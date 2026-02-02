using FluentValidation;
using HRMS.Application.DTOs.Leaves;
using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Leaves.LeaveBalances.Queries.GetEmployeeBalance;

// ═══════════════════════════════════════════════════════════════════════════
// 1. QUERY - الاستعلام
// ═══════════════════════════════════════════════════════════════════════════

/// <summary>
/// Query to get an employee's leave balances for a specific year.
/// Returns list of balances wrapped in Result pattern.
/// </summary>
public record GetEmployeeBalanceQuery : IRequest<Result<List<LeaveBalanceDto>>>
{
    /// <summary>
    /// معرف الموظف
    /// Employee ID
    /// </summary>
    public int EmployeeId { get; init; }

    /// <summary>
    /// السنة (اختياري - السنة الحالية افتراضياً)
    /// Year (optional - defaults to current year)
    /// </summary>
    public short? Year { get; init; }
}

// ═══════════════════════════════════════════════════════════════════════════
// 2. VALIDATOR - المدقق
// ═══════════════════════════════════════════════════════════════════════════

/// <summary>
/// Validator for GetEmployeeBalanceQuery.
/// Ensures valid employee ID and year.
/// </summary>
public class GetEmployeeBalanceQueryValidator : AbstractValidator<GetEmployeeBalanceQuery>
{
    public GetEmployeeBalanceQueryValidator()
    {
        // التحقق من معرف الموظف
        // Validate employee ID
        RuleFor(x => x.EmployeeId)
            .GreaterThan(0).WithMessage("معرف الموظف غير صحيح");

        // التحقق من السنة (إذا تم توفيرها)
        // Validate year (if provided)
        RuleFor(x => x.Year)
            .InclusiveBetween((short)2000, (short)2100)
            .When(x => x.Year.HasValue)
            .WithMessage("السنة يجب أن تكون بين 2000 و 2100");
    }
}

// ═══════════════════════════════════════════════════════════════════════════
// 3. HANDLER - معالج الاستعلام
// ═══════════════════════════════════════════════════════════════════════════

/// <summary>
/// Handler for getting employee leave balances.
/// Uses AsNoTracking and direct DTO projection for performance.
/// </summary>
public class GetEmployeeBalanceQueryHandler : IRequestHandler<GetEmployeeBalanceQuery, Result<List<LeaveBalanceDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetEmployeeBalanceQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<LeaveBalanceDto>>> Handle(GetEmployeeBalanceQuery request, CancellationToken cancellationToken)
    {
        // ═══════════════════════════════════════════════════════════════════════════
        // الخطوة 1: التحقق من وجود الموظف
        // Step 1: Verify employee exists
        // ═══════════════════════════════════════════════════════════════════════════

        var employeeExists = await _context.Employees
            .AnyAsync(e => e.EmployeeId == request.EmployeeId && e.IsDeleted == 0, cancellationToken);

        if (!employeeExists)
        {
            return Result<List<LeaveBalanceDto>>.Failure("الموظف غير موجود", 404);
        }

        // ═══════════════════════════════════════════════════════════════════════════
        // الخطوة 2: تحديد السنة المستهدفة
        // Step 2: Determine target year
        // ═══════════════════════════════════════════════════════════════════════════

        var targetYear = request.Year ?? (short)DateTime.Now.Year;

        // ═══════════════════════════════════════════════════════════════════════════
        // الخطوة 3: جلب أرصدة الموظف
        // Step 3: Get employee balances
        // ═══════════════════════════════════════════════════════════════════════════

        // نستخدم AsNoTracking لتحسين الأداء (قراءة فقط)
        // نستخدم Include لتحميل بيانات نوع الإجازة
        // نستخدم Direct DTO Projection لتقليل استهلاك الذاكرة
        var balances = await _context.EmployeeLeaveBalances
            .AsNoTracking()
            .Include(b => b.LeaveType) // تحميل بيانات نوع الإجازة
            .Where(b => b.EmployeeId == request.EmployeeId 
                     && b.Year == targetYear
                     && b.IsDeleted == 0)
            .Select(b => new LeaveBalanceDto
            {
                BalanceId = b.BalanceId,
                EmployeeId = b.EmployeeId,
                LeaveTypeId = b.LeaveTypeId,
                LeaveTypeName = b.LeaveType.LeaveNameAr, // اسم نوع الإجازة بالعربية
                LeaveTypeNameAr = b.LeaveType.LeaveNameAr, // للتوافق مع DTO
                Year = b.Year,
                CurrentBalance = b.CurrentBalance
            })
            .OrderBy(b => b.LeaveTypeName)
            .ToListAsync(cancellationToken);

        // ═══════════════════════════════════════════════════════════════════════════
        // الخطوة 4: إرجاع النتيجة
        // Step 4: Return result
        // ═══════════════════════════════════════════════════════════════════════════

        var message = balances.Count > 0
            ? $"تم استرجاع {balances.Count} رصيد إجازة للموظف للسنة {targetYear}"
            : $"لا توجد أرصدة إجازات مسجلة للموظف للسنة {targetYear}";

        return Result<List<LeaveBalanceDto>>.Success(balances, message);
    }
}
