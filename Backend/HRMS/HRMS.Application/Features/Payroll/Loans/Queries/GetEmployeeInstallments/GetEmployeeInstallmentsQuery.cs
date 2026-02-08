using HRMS.Application.DTOs.Payroll;
using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Payroll.Loans.Queries.GetEmployeeInstallments;

public class GetEmployeeInstallmentsQuery : IRequest<Result<List<LoanInstallmentDto>>>
{
    public int EmployeeId { get; set; }
}

public class GetEmployeeInstallmentsQueryHandler : IRequestHandler<GetEmployeeInstallmentsQuery, Result<List<LoanInstallmentDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetEmployeeInstallmentsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<LoanInstallmentDto>>> Handle(GetEmployeeInstallmentsQuery request, CancellationToken cancellationToken)
    {
        var installments = await _context.LoanInstallments
            .Include(i => i.Loan)
            .ThenInclude(l => l.Employee)
            .Where(i => i.Loan.EmployeeId == request.EmployeeId)
            .OrderBy(i => i.DueDate)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var dtos = installments.Select(i => new LoanInstallmentDto
        {
            InstallmentId =(int)i.InstallmentId,
            LoanId = i.LoanId,
            EmployeeId = i.Loan.EmployeeId,
            EmployeeName = i.Loan.Employee.FullNameAr,
            TotalLoanAmount = i.Loan.LoanAmount,
            InstallmentNumber = i.InstallmentNumber,
            InstallmentAmount = i.Amount,
            DueDate = i.DueDate,
            IsPaid = i.IsPaid == 1
        }).ToList();

        return Result<List<LoanInstallmentDto>>.Success(dtos);
    }
}
