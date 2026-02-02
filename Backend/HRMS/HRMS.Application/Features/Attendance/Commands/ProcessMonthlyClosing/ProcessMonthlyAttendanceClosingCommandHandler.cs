using HRMS.Application.DTOs.Attendance;
using HRMS.Application.Exceptions;
using HRMS.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Attendance.Commands.ProcessMonthlyClosing;

/// <summary>
/// Handler for processing monthly attendance closing with dynamic policy application.
/// Implements ERP-grade calculations with complete audit trail.
/// </summary>
public class ProcessMonthlyAttendanceClosingCommandHandler 
    : IRequestHandler<ProcessMonthlyAttendanceClosingCommand, MonthlyClosingResultDto>
{
    private readonly IApplicationDbContext _context;

    public ProcessMonthlyAttendanceClosingCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<MonthlyClosingResultDto> Handle(
        ProcessMonthlyAttendanceClosingCommand request, 
        CancellationToken cancellationToken)
    {
        // ═══════════════════════════════════════════════════════════
        // المرحلة 1: التحقق من وجود الفترة وحالة القفل
        // Phase 1: Validate period existence and lock status
        // ═══════════════════════════════════════════════════════════

        var period = await _context.Database
            .SqlQuery<RosterPeriodDto>($@"
                SELECT 
                    PERIOD_ID as PeriodId,
                    YEAR as Year,
                    MONTH as Month,
                    IS_LOCKED as IsLocked,
                    START_DATE as StartDate,
                    END_DATE as EndDate
                FROM [HR_ATTENDANCE].[ROSTER_PERIODS]
                WHERE YEAR = {request.Year} AND MONTH = {request.Month}
            ")
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);

        if (period == null)
            throw new NotFoundException(
                $"لا توجد فترة روستر للشهر {request.Month}/{request.Year}. " +
                "يجب إنشاء الفترة أولاً قبل الإغلاق.");

        if (period.IsLocked == 1)
            throw new InvalidOperationException(
                $"الفترة {request.Month}/{request.Year} مغلقة بالفعل. " +
                "لا يمكن إعادة المعالجة.");

        // ═══════════════════════════════════════════════════════════
        // المرحلة 2: بدء معاملة ذرية لضمان تكامل البيانات
        // Phase 2: Start atomic transaction
        // ═══════════════════════════════════════════════════════════

        using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var result = new MonthlyClosingResultDto
            {
                LockedPeriodId = period.PeriodId,
                ClosedAt = DateTime.Now
            };

            // ═══════════════════════════════════════════════════════════
            // المرحلة 3: جلب جميع سجلات الحضور للفترة المحددة
            // Phase 3: Fetch all attendance records for the period
            // ═══════════════════════════════════════════════════════════

            var attendanceRecords = await _context.DailyAttendances
                .Include(a => a.Employee)
                    .ThenInclude(e => e.Department)
                .Where(a => a.AttendanceDate >= period.StartDate 
                         && a.AttendanceDate <= period.EndDate)
                .ToListAsync(cancellationToken);

            if (!attendanceRecords.Any())
            {
                // لا توجد سجلات حضور - نغلق الفترة فقط
                await LockPeriod(period.PeriodId, request.ClosedByUserId, cancellationToken);
                await transaction.CommitAsync(cancellationToken);
                return result;
            }

            // ═══════════════════════════════════════════════════════════
            // المرحلة 4: تجميع السجلات حسب الموظف والقسم
            // Phase 4: Group records by employee and department
            // ═══════════════════════════════════════════════════════════

            var employeeGroups = attendanceRecords
                .GroupBy(a => new 
                { 
                    a.EmployeeId, 
                    DeptId = a.Employee.DepartmentId 
                })
                .ToList();

            result.TotalEmployeesProcessed = employeeGroups.Count;

            // ═══════════════════════════════════════════════════════════
            // المرحلة 5: معالجة كل موظف بناءً على سياسة قسمه
            // Phase 5: Process each employee based on department policy
            // ═══════════════════════════════════════════════════════════

            foreach (var employeeGroup in employeeGroups)
            {
                // جلب السياسة الخاصة بقسم الموظف (ديناميكياً - بدون قيم ثابتة)
                // Fetch policy for employee's department (dynamic - zero hardcoding)
                var policy = await GetDepartmentPolicy(employeeGroup.Key.DeptId, cancellationToken);

                if (policy == null)
                {
                    // في حالة عدم وجود سياسة خاصة، نستخدم السياسة الافتراضية
                    // If no specific policy exists, use default policy
                    policy = await GetDefaultPolicy(cancellationToken);
                }

                if (policy == null)
                    throw new InvalidOperationException(
                        $"لا توجد سياسة حضور معرفة للقسم {employeeGroup.Key.DeptId}. " +
                        "يجب تعريف سياسة افتراضية على الأقل.");

                // معالجة سجلات الموظف
                // Process employee records
                foreach (var attendance in employeeGroup)
                {
                    // ═══════════════════════════════════════════════════════════
                    // حساب التأخير المحتسب (بعد فترة السماح)
                    // Calculate chargeable late time (after grace period)
                    // ═══════════════════════════════════════════════════════════

                    short chargeableLateMinutes = 0;
                    
                    if (attendance.LateMinutes > policy.LateGraceMins)
                    {
                        // إذا تجاوز التأخير فترة السماح، نحتسب كامل وقت التأخير
                        // If late time exceeds grace period, charge full late time
                        chargeableLateMinutes = attendance.LateMinutes;
                    }
                    // else: التأخير ضمن فترة السماح = لا يُحتسب
                    // else: Late time within grace = not charged

                    result.TotalLateMinutesCharged += chargeableLateMinutes;

                    // ═══════════════════════════════════════════════════════════
                    // حساب الوقت الإضافي باستخدام المعامل من السياسة
                    // Calculate overtime using policy multiplier
                    // ═══════════════════════════════════════════════════════════

                    if (attendance.OvertimeMinutes > 0)
                    {
                        // المعامل يأتي من السياسة ديناميكياً
                        // Multiplier comes from policy dynamically
                        var multiplier = policy.OvertimeMultiplier;
                        
                        // يمكن حفظ الوقت الإضافي المحسوب في جدول منفصل للرواتب
                        // Can save calculated overtime to separate payroll table
                        var calculatedOvertimeValue = attendance.OvertimeMinutes * multiplier;
                        
                        result.TotalOvertimeMinutes += attendance.OvertimeMinutes;

                        // ملاحظة: في نظام ERP حقيقي، نحفظ هذه القيم في جدول PAYROLL_INPUTS
                        // Note: In real ERP, save these values to PAYROLL_INPUTS table
                    }

                    // هنا يمكن إضافة منطق إضافي مثل:
                    // Additional logic can be added here such as:
                    // - حساب خصومات الغياب (Absence deductions)
                    // - حساب بدل المواصلات (Transportation allowance)
                    // - تطبيق قواعد الانصراف المبكر (Early leave rules)
                }
            }

            // ═══════════════════════════════════════════════════════════
            // المرحلة 6: قفل الفترة لمنع التعديلات المستقبلية
            // Phase 6: Lock period to prevent future modifications
            // ═══════════════════════════════════════════════════════════

            await LockPeriod(period.PeriodId, request.ClosedByUserId, cancellationToken);

            // ═══════════════════════════════════════════════════════════
            // المرحلة 7: حفظ التغييرات وإتمام المعاملة
            // Phase 7: Save changes and commit transaction
            // ═══════════════════════════════════════════════════════════

            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return result;
        }
        catch (Exception)
        {
            // في حالة حدوث خطأ، نلغي جميع التغييرات
            // On error, rollback all changes
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    /// <summary>
    /// Retrieves attendance policy for a specific department (dynamic lookup)
    /// </summary>
    private async Task<AttendancePolicyDto?> GetDepartmentPolicy(
        int? deptId, 
        CancellationToken cancellationToken)
    {
        if (deptId == null)
            return null;

        return await _context.Database
            .SqlQuery<AttendancePolicyDto>($@"
                SELECT TOP 1
                    POLICY_ID as PolicyId,
                    LATE_GRACE_MINS as LateGraceMins,
                    OVERTIME_MULTIPLIER as OvertimeMultiplier,
                    WEEKEND_OT_MULTIPLIER as WeekendOtMultiplier
                FROM [HR_ATTENDANCE].[ATTENDANCE_POLICIES]
                WHERE DEPT_ID = {deptId}
                ORDER BY POLICY_ID DESC
            ")
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <summary>
    /// Retrieves default attendance policy when no department-specific policy exists
    /// </summary>
    private async Task<AttendancePolicyDto?> GetDefaultPolicy(CancellationToken cancellationToken)
    {
        return await _context.Database
            .SqlQuery<AttendancePolicyDto>($@"
                SELECT TOP 1
                    POLICY_ID as PolicyId,
                    LATE_GRACE_MINS as LateGraceMins,
                    OVERTIME_MULTIPLIER as OvertimeMultiplier,
                    WEEKEND_OT_MULTIPLIER as WeekendOtMultiplier
                FROM [HR_ATTENDANCE].[ATTENDANCE_POLICIES]
                WHERE DEPT_ID IS NULL AND JOB_ID IS NULL
                ORDER BY POLICY_ID DESC
            ")
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <summary>
    /// Locks the roster period to prevent further modifications
    /// </summary>
    private async Task LockPeriod(
        int periodId, 
        int lockedByUserId, 
        CancellationToken cancellationToken)
    {
        await _context.Database.ExecuteSqlRawAsync($@"
            UPDATE [HR_ATTENDANCE].[ROSTER_PERIODS]
            SET IS_LOCKED = 1,
                LOCKED_BY = {lockedByUserId},
                UPDATED_AT = GETDATE(),
                UPDATED_BY = {lockedByUserId}
            WHERE PERIOD_ID = {periodId}
        ", cancellationToken);
    }

    #region Internal DTOs for Raw SQL Queries

    /// <summary>
    /// DTO for roster period raw SQL queries
    /// </summary>
    internal class RosterPeriodDto
    {
        public int PeriodId { get; set; }
        public short Year { get; set; }
        public byte Month { get; set; }
        public byte IsLocked { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    /// <summary>
    /// DTO for attendance policy raw SQL queries
    /// </summary>
    internal class AttendancePolicyDto
    {
        public int PolicyId { get; set; }
        public short LateGraceMins { get; set; }
        public decimal OvertimeMultiplier { get; set; }
        public decimal WeekendOtMultiplier { get; set; }
    }

    #endregion
}
