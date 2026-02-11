using HRMS.Application.DTOs.Attendance;
using HRMS.Application.Exceptions;
using HRMS.Application.Interfaces;
using HRMS.Core.Entities.Payroll;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Attendance.Commands.ProcessMonthlyClosing;

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
		// 1. التحقق من وجود الفترة
		var period = await _context.Database
			.SqlQuery<RosterPeriodDto>($@"
                SELECT PERIOD_ID as PeriodId, IS_LOCKED as IsLocked, START_DATE as StartDate, END_DATE as EndDate
                FROM [HR_ATTENDANCE].[ROSTER_PERIODS]
                WHERE YEAR = {request.Year} AND MONTH = {request.Month}")
			.FirstOrDefaultAsync(cancellationToken);

		if (period == null)
			throw new NotFoundException($"فترة الحضور {request.Month}/{request.Year} غير موجودة.");

		if (period.IsLocked == 1)
			throw new InvalidOperationException($"الفترة {request.Month}/{request.Year} مغلقة بالفعل.");

		using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

		try
		{
			var result = new MonthlyClosingResultDto { LockedPeriodId = period.PeriodId, ClosedAt = DateTime.Now };

			// 2. جلب سجلات الحضور
			// ملاحظة: قمنا بإزالة .ThenInclude(e => e.Salaries) لأنها تسببت في خطأ
			var attendanceRecords = await _context.DailyAttendances
				.Include(a => a.Employee)
				.Where(a => a.AttendanceDate >= period.StartDate && a.AttendanceDate <= period.EndDate)
				.ToListAsync(cancellationToken);

			// 3. جلب الرواتب الأساسية للموظفين الموجودين في الحضور (استعلام مباشر لضمان الدقة)
			// هذا يحل مشكلة عدم وجود Navigation Property
			var employeeIds = attendanceRecords.Select(x => x.EmployeeId).Distinct().ToList();

			var salariesDict = new Dictionary<int, decimal>();

			if (employeeIds.Any())
			{
				var empIdsStr = string.Join(",", employeeIds);
				// نجلب أحدث راتب أساسي لكل موظف
				var salariesDto = await _context.Database
					.SqlQuery<EmployeeSalaryDto>($@"
                        SELECT t.EMPLOYEE_ID as EmployeeId, t.BASIC_SALARY as BasicSalary
                        FROM [HR_PAYROLL].[SALARIES] t
                        INNER JOIN (
                            SELECT EMPLOYEE_ID, MAX(SALARY_ID) as MaxId
                            FROM [HR_PAYROLL].[SALARIES]
                            WHERE EMPLOYEE_ID IN ({empIdsStr})
                            GROUP BY EMPLOYEE_ID
                        ) tm ON t.EMPLOYEE_ID = tm.EMPLOYEE_ID AND t.SALARY_ID = tm.MaxId
                    ")
					.ToListAsync(cancellationToken);

				salariesDict = salariesDto.ToDictionary(k => k.EmployeeId, v => v.BasicSalary);
			}

			// تجميع السجلات حسب الموظف
			var employeeGroups = attendanceRecords.GroupBy(a => new { a.EmployeeId, a.Employee.DepartmentId });

			var financialAdjustments = new List<PayrollAdjustment>();

			foreach (var group in employeeGroups)
			{
				var employeeId = group.Key.EmployeeId;
				var deptId = group.Key.DepartmentId;

				// حساب سعر الدقيقة من الراتب الذي جلبناه يدوياً
				decimal basicSalary = salariesDict.ContainsKey(employeeId) ? salariesDict[employeeId] : 0;

				// المعادلة: الراتب / 30 يوم / 8 ساعات / 60 دقيقة
				decimal minuteRate = (basicSalary > 0) ? (basicSalary / 30 / 8 / 60) : 0;

				// جلب السياسة
				var policy = await GetDepartmentPolicy(deptId, cancellationToken) ?? await GetDefaultPolicy(cancellationToken);
				if (policy == null) continue;

				int totalLateMinutes = 0;
				int totalOvertimeMinutes = 0;

				foreach (var record in group)
				{
					if (record.LateMinutes > policy.LateGraceMins)
						totalLateMinutes += record.LateMinutes;

					if (record.OvertimeMinutes > 0)
					{
						var isWeekend = record.Status == "OFF" || record.Status == "WEEKEND";
						var multiplier = isWeekend ? policy.WeekendOtMultiplier : policy.OvertimeMultiplier;
						totalOvertimeMinutes += (int)(record.OvertimeMinutes * multiplier);
					}
				}

				// =========================================================
				// التكامل المالي (تم تصحيح أسماء الحقول)
				// =========================================================

				// 1. خصم التأخير
				if (totalLateMinutes > 0 && minuteRate > 0)
				{
					var deductionAmount = totalLateMinutes * minuteRate;

					financialAdjustments.Add(new PayrollAdjustment
					{
						EmployeeId = employeeId,
						AdjustmentType = "DEDUCTION",
						Amount = Math.Round(deductionAmount, 2),
						Reason = $"Late: {totalLateMinutes} mins ({request.Month}/{request.Year})",
						// تم التصحيح: استخدام CreatedAt بدلاً من AdjustmentDate
						CreatedAt = DateTime.Now,
						CreatedBy = request.ClosedByUserId.ToString()
					});
				}

				// 2. استحقاق الإضافي
				if (totalOvertimeMinutes > 0 && minuteRate > 0)
				{
					var earningAmount = totalOvertimeMinutes * minuteRate;

					financialAdjustments.Add(new PayrollAdjustment
					{
						EmployeeId = employeeId,
						AdjustmentType = "EARNING",
						Amount = Math.Round(earningAmount, 2),
						Reason = $"Overtime: {totalOvertimeMinutes} paid mins ({request.Month}/{request.Year})",
						// تم التصحيح: استخدام CreatedAt بدلاً من AdjustmentDate
						CreatedAt = DateTime.Now,
						CreatedBy = request.ClosedByUserId.ToString()
					});
				}

				result.TotalLateMinutesCharged += totalLateMinutes;
				result.TotalOvertimeMinutes += totalOvertimeMinutes;
			}

			if (financialAdjustments.Any())
			{
				await _context.PayrollAdjustments.AddRangeAsync(financialAdjustments, cancellationToken);
			}

			await LockPeriod(period.PeriodId, request.ClosedByUserId, cancellationToken);
			await _context.SaveChangesAsync(cancellationToken);
			await transaction.CommitAsync(cancellationToken);

			result.TotalEmployeesProcessed = employeeGroups.Count();
			return result;
		}
		catch
		{
			await transaction.RollbackAsync(cancellationToken);
			throw;
		}
	}

	// --- Helpers ---

	private async Task<AttendancePolicyDto?> GetDepartmentPolicy(int? deptId, CancellationToken cancellationToken)
	{
		if (deptId == null) return null;
		return await _context.Database.SqlQuery<AttendancePolicyDto>($@"
            SELECT TOP 1 POLICY_ID as PolicyId, LATE_GRACE_MINS as LateGraceMins, 
                         OVERTIME_MULTIPLIER as OvertimeMultiplier, WEEKEND_OT_MULTIPLIER as WeekendOtMultiplier
            FROM [HR_ATTENDANCE].[ATTENDANCE_POLICIES] WHERE DEPT_ID = {deptId} ORDER BY POLICY_ID DESC")
		   .FirstOrDefaultAsync(cancellationToken);
	}

	private async Task<AttendancePolicyDto?> GetDefaultPolicy(CancellationToken cancellationToken)
	{
		return await _context.Database.SqlQuery<AttendancePolicyDto>($@"
            SELECT TOP 1 POLICY_ID as PolicyId, LATE_GRACE_MINS as LateGraceMins, 
                         OVERTIME_MULTIPLIER as OvertimeMultiplier, WEEKEND_OT_MULTIPLIER as WeekendOtMultiplier
            FROM [HR_ATTENDANCE].[ATTENDANCE_POLICIES] WHERE DEPT_ID IS NULL ORDER BY POLICY_ID DESC")
			.FirstOrDefaultAsync(cancellationToken);
	}

	private async Task LockPeriod(int periodId, int userId, CancellationToken ct)
	{
		await _context.Database.ExecuteSqlRawAsync(
			$"UPDATE [HR_ATTENDANCE].[ROSTER_PERIODS] SET IS_LOCKED = 1, LOCKED_BY = {userId}, UPDATED_AT = GETDATE() WHERE PERIOD_ID = {periodId}", ct);
	}

	internal class RosterPeriodDto { public int PeriodId { get; set; } public byte IsLocked { get; set; } public DateTime StartDate { get; set; } public DateTime EndDate { get; set; } }
	internal class AttendancePolicyDto { public int PolicyId { get; set; } public short LateGraceMins { get; set; } public decimal OvertimeMultiplier { get; set; } public decimal WeekendOtMultiplier { get; set; } }
	// كلاس مساعد لجلب الراتب
	internal class EmployeeSalaryDto { public int EmployeeId { get; set; } public decimal BasicSalary { get; set; } }
}