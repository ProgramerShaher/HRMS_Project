using FluentValidation;
using HRMS.Application.Interfaces;
using HRMS.Core.Entities.Payroll;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Performance.Violations.Commands.ApproveViolation;

public class ApproveViolationCommand : IRequest<Result<int>>
{
    public int ViolationId { get; set; }
}

public class ApproveViolationCommandValidator : AbstractValidator<ApproveViolationCommand>
{
    public ApproveViolationCommandValidator()
    {
        RuleFor(x => x.ViolationId)
            .GreaterThan(0)
            .WithMessage("معرف المخالفة مطلوب");
    }
}

public class ApproveViolationCommandHandler : IRequestHandler<ApproveViolationCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public ApproveViolationCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result<int>> Handle(ApproveViolationCommand request, CancellationToken cancellationToken)
    {
        // استخدام Transaction لضمان Atomicity
        using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            // 1. جلب المخالفة مع البيانات المرتبطة
            var violation = await _context.EmployeeViolations
                .Include(v => v.ViolationType)
                .Include(v => v.Action)
                .FirstOrDefaultAsync(v => v.ViolationId == request.ViolationId && v.IsDeleted == 0, cancellationToken);

            if (violation == null)
                return Result<int>.Failure("المخالفة المحددة غير موجودة");

            if (violation.Status == "APPROVED")
                return Result<int>.Failure("هذه المخالفة معتمدة مسبقاً");

            if (violation.IsExecuted == 1)
                return Result<int>.Failure("تم تنفيذ الخصم المالي لهذه المخالفة مسبقاً");

            // 2. معالجة الخصم المالي (ERP Magic)
            if (violation.ActionId.HasValue && violation.Action != null && violation.Action.DeductionDays > 0)
            {
                // 🔍 جلب الراتب الأساسي
                var basicSalaryStructure = await _context.SalaryStructures
                    .Include(s => s.SalaryElement)
                    .FirstOrDefaultAsync(s => s.EmployeeId == violation.EmployeeId 
                                            && s.IsActive == 1 
                                            && s.SalaryElement.IsBasic == 1, 
                                            cancellationToken);

                if (basicSalaryStructure == null || basicSalaryStructure.Amount <= 0)
                {
                    // يمكن السماح بالاعتماد بدون خصم مع تحذير، أو منع الاعتماد. سنمنع الاعتماد لضمان التكامل.
                    return Result<int>.Failure("لا يمكن اعتماد المخالفة: لا يوجد راتب أساسي مسجل للموظف لحساب الخصم");
                }

                // 💰 حساب مبلغ الخصم: (BasicSalary / 30) × DeductionDays
                decimal deductionAmount = Math.Round((basicSalaryStructure.Amount / 30m) * (decimal)violation.Action.DeductionDays, 2);

                // 📝 إنشاء سجل Payroll Adjustment
                var adjustment = new PayrollAdjustment
                {
                    EmployeeId = violation.EmployeeId,
                    AdjustmentType = "DEDUCTION",
                    Amount = deductionAmount,
                    Reason = $"مخالفة إدارية: {violation.ViolationType.ViolationNameAr} - {violation.Action.ActionNameAr} (Ref: {violation.ViolationId})",
                    ApprovedBy = int.Parse(_currentUserService.UserId ?? "0"),
                    CreatedBy = _currentUserService.UserId,
                    CreatedAt = DateTime.UtcNow
                };

                _context.PayrollAdjustments.Add(adjustment);
                
                // تحديث حالة التنفيذ
                violation.IsExecuted = 1;
            }

            // 3. تحديث حالة المخالفة
            violation.Status = "APPROVED";
            violation.UpdatedBy = _currentUserService.UserId;
            violation.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return Result<int>.Success(violation.ViolationId, "تم اعتماد المخالفة وتنفيذ الخصم المالي (إن وجد) بنجاح");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            return Result<int>.Failure($"حدث خطأ أثناء اعتماد المخالفة: {ex.Message}");
        }
    }
}
