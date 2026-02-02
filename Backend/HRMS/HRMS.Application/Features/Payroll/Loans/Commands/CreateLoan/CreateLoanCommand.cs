using HRMS.Application.Interfaces;
using HRMS.Core.Entities.Payroll;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Payroll.Loans.Commands.CreateLoan;

public class CreateLoanCommand : IRequest<Result<int>>
{
    public int EmployeeId { get; set; }
    public decimal LoanAmount { get; set; }
    public short InstallmentCount { get; set; }
    public DateTime StartDate { get; set; }
}

public class CreateLoanCommandHandler : IRequestHandler<CreateLoanCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;

    public CreateLoanCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<int>> Handle(CreateLoanCommand request, CancellationToken cancellationToken)
    {
        if (request.LoanAmount <= 0) return Result<int>.Failure("مبلغ السلفة يجب أن يكون أكبر من صفر");
        if (request.InstallmentCount <= 0) return Result<int>.Failure("عدد الأقساط يجب أن يكون 1 على الأقل");

        // 1. Calculate Monthly Installment
        decimal monthlyInstallment = Math.Round(request.LoanAmount / request.InstallmentCount, 2);

        // 2. Fetch Employee's Basic Salary (using IsBasic flag)
        var basicSalaryStructure = await _context.SalaryStructures
            .Include(s => s.SalaryElement)
            .Where(s => s.EmployeeId == request.EmployeeId && s.IsActive == 1 && s.SalaryElement.IsBasic == 1)
            .FirstOrDefaultAsync(cancellationToken);

        if (basicSalaryStructure == null)
            return Result<int>.Failure("لا يمكن منح سلفة للموظف لعدم وجود راتب أساسي معرف.");

        decimal basicSalary = basicSalaryStructure.Amount;
        
        // 3. Validation: 30% Rule
        decimal maxAllowedInstallment = basicSalary * 0.30m;

        if (monthlyInstallment > maxAllowedInstallment)
        {
            return Result<int>.Failure($"قيمة القسط الشهري ({monthlyInstallment}) تتجاوز 30% من الراتب الأساسي ({maxAllowedInstallment})");
        }

        // 4. Create Loan
        var loan = new Loan
        {
            EmployeeId = request.EmployeeId,
            LoanAmount = request.LoanAmount,
            InstallmentCount = request.InstallmentCount,
            RequestDate = DateTime.Now,
            Status = "ACTIVE"
        };

        // 5. Generate Installments
        var installments = new List<LoanInstallment>();
        decimal remainingAmount = request.LoanAmount;

        for (int i = 0; i < request.InstallmentCount; i++)
        {
            decimal currentAmount = monthlyInstallment;
            
            // Adjust last installment for rounding differences
            if (i == request.InstallmentCount - 1)
            {
                currentAmount = remainingAmount;
            }

            installments.Add(new LoanInstallment
            {
                InstallmentNumber = (short)(i + 1),
                DueDate = request.StartDate.AddMonths(i),
                Amount = currentAmount,
                IsPaid = 0
            });
            
            remainingAmount -= currentAmount;
        }

        loan.Installments = installments;
        _context.Loans.Add(loan);

        await _context.SaveChangesAsync(cancellationToken);

        return Result<int>.Success(loan.LoanId, "تم إنشاء السلفة وجدولة الأقساط بنجاح");
    }
}
