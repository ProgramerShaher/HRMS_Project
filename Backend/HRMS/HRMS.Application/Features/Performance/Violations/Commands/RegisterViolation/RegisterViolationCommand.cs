using FluentValidation;
using HRMS.Application.Interfaces;
using HRMS.Core.Entities.Performance;
using HRMS.Core.Entities.Payroll;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Performance.Violations.Commands.RegisterViolation;

/// <summary>
/// أمر تسجيل مخالفة إدارية مع حساب الخصم المالي تلقائياً
/// </summary>
/// <remarks>
/// المنطق (ERP Integration):
/// 1. حفظ المخالفة في جدول EMPLOYEE_VIOLATIONS
/// 2. جلب الإجراء التأديبي المرتبط (DisciplinaryAction)
/// 3. إذا كان DeductionDays > 0:
///    - جلب الراتب الأساسي من EMPLOYEE_SALARY_STRUCTURE (باستخدام IsBasic flag)
///    - حساب المبلغ: Amount = (BasicSalary / 30) × DeductionDays
///    - إدراج تلقائي في PAYROLL_ADJUSTMENTS
///    - تحديث IsExecuted = 1
/// 4. كل العمليات في Transaction واحدة (Atomicity)
/// </remarks>
public class RegisterViolationCommand : IRequest<Result<int>>
{
    /// <summary>
    /// معرف الموظف
    /// </summary>
    public int EmployeeId { get; set; }

    /// <summary>
    /// نوع المخالفة
    /// </summary>
    public int ViolationTypeId { get; set; }

    /// <summary>
    /// الإجراء التأديبي المطبق
    /// </summary>
    public int? ActionId { get; set; }

    /// <summary>
    /// وصف المخالفة
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// تاريخ المخالفة
    /// </summary>
    public DateTime ViolationDate { get; set; } = DateTime.Now;
}

/// <summary>
/// التحقق من صحة بيانات المخالفة
/// </summary>
public class RegisterViolationCommandValidator : AbstractValidator<RegisterViolationCommand>
{
    public RegisterViolationCommandValidator()
    {
        RuleFor(x => x.EmployeeId)
            .GreaterThan(0)
            .WithMessage("معرف الموظف مطلوب");

        RuleFor(x => x.ViolationTypeId)
            .GreaterThan(0)
            .WithMessage("نوع المخالفة مطلوب");

        RuleFor(x => x.ViolationDate)
            .NotEmpty()
            .WithMessage("تاريخ المخالفة مطلوب")
            .LessThanOrEqualTo(DateTime.Now)
            .WithMessage("تاريخ المخالفة لا يمكن أن يكون في المستقبل");

        RuleFor(x => x.Description)
            .MaximumLength(500)
            .WithMessage("الوصف لا يمكن أن يتجاوز 500 حرف");
    }
}

/// <summary>
/// معالج أمر تسجيل المخالفة مع التكامل المالي
/// </summary>
public class RegisterViolationCommandHandler : IRequestHandler<RegisterViolationCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public RegisterViolationCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result<int>> Handle(RegisterViolationCommand request, CancellationToken cancellationToken)
    {
        // استخدام Transaction لضمان Atomicity
        using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            // ✅ 1. التحقق من وجود الموظف
            var employeeExists = await _context.Employees
                .AnyAsync(e => e.EmployeeId == request.EmployeeId && e.IsDeleted == 0, cancellationToken);

            if (!employeeExists)
                return Result<int>.Failure("الموظف المحدد غير موجود");

            // ✅ 2. التحقق من نوع المخالفة
            var violationType = await _context.ViolationTypes
                .FirstOrDefaultAsync(v => v.ViolationTypeId == request.ViolationTypeId && v.IsDeleted == 0, cancellationToken);

            if (violationType == null)
                return Result<int>.Failure("نوع المخالفة المحدد غير موجود");

            // ✅ 3. إنشاء سجل المخالفة
            var violation = new EmployeeViolation
            {
                EmployeeId = request.EmployeeId,
                ViolationTypeId = request.ViolationTypeId,
                ActionId = request.ActionId,
                ViolationDate = request.ViolationDate,
                Description = request.Description,
                Status = "PENDING", // ✅ Default status is now PENDING waiting for approval
                IsExecuted = 0,
                CreatedBy = _currentUserService.UserId,
                CreatedAt = DateTime.UtcNow
            };

            _context.EmployeeViolations.Add(violation);
            await _context.SaveChangesAsync(cancellationToken);

            // ⚠️ Financial Deduction Logic Removed
            // It has been moved to ApproveViolationCommand to enforce the approval workflow.

            // ✅ 5. Commit Transaction
            await transaction.CommitAsync(cancellationToken);

            return Result<int>.Success(
                violation.ViolationId,
                "تم تسجيل المخالفة بنجاح، وهي في انتظار الاعتماد (Pending Approval)");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            return Result<int>.Failure($"حدث خطأ أثناء تسجيل المخالفة: {ex.Message}");
        }
    }
}
