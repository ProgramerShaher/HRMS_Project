using FluentValidation;
using HRMS.Application.Features.Payroll.Loans.Services;
using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Payroll.Loans.Commands.ChangeStatus;

/// <summary>
/// Change Loan Status Command
/// </summary>
public class ChangeLoanStatusCommand : IRequest<Result<bool>>
{
    public int LoanId { get; set; }
    public string NewStatus { get; set; } = null!;
}

/// <summary>
/// Change Loan Status Command Validator
/// </summary>
public class ChangeLoanStatusCommandValidator : AbstractValidator<ChangeLoanStatusCommand>
{
    public ChangeLoanStatusCommandValidator()
    {
        RuleFor(x => x.LoanId).GreaterThan(0);
        RuleFor(x => x.NewStatus)
            .NotEmpty()
            .Must(s => new[] { "PENDING", "APPROVED", "ACTIVE", "SETTLED", "CLOSED" }.Contains(s))
            .WithMessage("الحالة يجب أن تكون: PENDING, APPROVED, ACTIVE, SETTLED, CLOSED");
    }
}

/// <summary>
/// Change Loan Status Command Handler
/// </summary>
public class ChangeLoanStatusCommandHandler : IRequestHandler<ChangeLoanStatusCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly GenerateInstallmentsService _installmentsService;

    public ChangeLoanStatusCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService,
        GenerateInstallmentsService installmentsService)
    {
        _context = context;
        _currentUserService = currentUserService;
        _installmentsService = installmentsService;
    }

    public async Task<Result<bool>> Handle(ChangeLoanStatusCommand request, CancellationToken cancellationToken)
    {
        // جلب القرض مع بيانات الموظف
        var loan = await _context.Loans
            .Include(l => l.Employee)
            .FirstOrDefaultAsync(l => l.LoanId == request.LoanId && l.IsDeleted == 0, cancellationToken);

        if (loan == null)
            return Result<bool>.Failure("القرض غير موجود");

        // Business Rule: التحقق من حالة الموظف عند التفعيل
        if (request.NewStatus == "ACTIVE")
        {
            // التحقق من أن الموظف نشط
            if (loan.Employee.IsActive != true)
                return Result<bool>.Failure("لا يمكن تفعيل القرض لموظف غير نشط");

            // جلب الراتب الأساسي للموظف من هيكل الراتب
            // نبحث عن عنصر الراتب الأساسي (BASIC_SALARY) في هيكل راتب الموظف
            var basicSalaryElement = await _context.SalaryStructures
                .Include(s => s.SalaryElement)
                .Where(s => s.EmployeeId == loan.EmployeeId 
                    && s.IsDeleted == 0 
                    && s.IsActive == 1
                    && s.SalaryElement.ElementType == "BASIC_SALARY")
                .FirstOrDefaultAsync(cancellationToken);

            // التحقق من وجود راتب أساسي
            if (basicSalaryElement == null)
                return Result<bool>.Failure("لا يمكن تفعيل القرض. الموظف لا يملك راتب أساسي محدد");

            decimal basicSalary = basicSalaryElement.Amount;

            // حساب القسط الشهري
            decimal monthlyInstallment = loan.LoanAmount / loan.InstallmentCount;

            // Business Rule: القسط الشهري يجب ألا يتجاوز 30% من الراتب الأساسي
            decimal maxAllowedDeduction = basicSalary * 0.30m;

            if (monthlyInstallment > maxAllowedDeduction)
            {
                return Result<bool>.Failure(
                    $"القسط الشهري ({monthlyInstallment:N2}) يتجاوز الحد المسموح (30% من الراتب الأساسي = {maxAllowedDeduction:N2})");
            }

            // تسجيل بيانات الموافقة
            loan.ApprovalDate = DateTime.UtcNow;
            // تحويل UserId من string إلى int (نفترض أن UserId يحتوي على رقم صحيح)
            if (int.TryParse(_currentUserService.UserId, out int approvedById))
            {
                loan.ApprovedBy = approvedById;
            }

            // توليد الأقساط تلقائياً
            var installmentsResult = await _installmentsService.GenerateInstallmentsAsync(loan.LoanId, cancellationToken);
            if (!installmentsResult.Succeeded)
                return Result<bool>.Failure($"فشل توليد الأقساط: {installmentsResult.Message}");
        }

        // تحديث حالة القرض
        loan.Status = request.NewStatus;
        
        // تحويل UserId من string إلى int للتحديث
        if (int.TryParse(_currentUserService.UserId, out int updatedById))
        {
            loan.UpdatedBy = updatedById.ToString();
        }
        loan.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true, $"تم تغيير حالة القرض إلى {request.NewStatus} بنجاح");
    }
}
