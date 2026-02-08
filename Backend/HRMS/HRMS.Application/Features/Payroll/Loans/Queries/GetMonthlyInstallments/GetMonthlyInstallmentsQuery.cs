using HRMS.Application.DTOs.Payroll;
using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Payroll.Loans.Queries.GetMonthlyInstallments;

public class GetMonthlyInstallmentsQuery : IRequest<Result<List<LoanInstallmentDto>>>
{
    public int Month { get; set; }
    public int Year { get; set; }
    public int? EmployeeId { get; set; }
}

public class GetMonthlyInstallmentsQueryHandler : IRequestHandler<GetMonthlyInstallmentsQuery, Result<List<LoanInstallmentDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetMonthlyInstallmentsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<LoanInstallmentDto>>> Handle(GetMonthlyInstallmentsQuery request, CancellationToken cancellationToken)
    {
        var startDate = new DateTime(request.Year, request.Month, 1);
        var endDate = startDate.AddMonths(1).AddDays(-1);

        var query = _context.LoanInstallments
            .Include(i => i.Loan)
            .ThenInclude(l => l.Employee)
            .Where(i => i.DueDate >= startDate && i.DueDate <= endDate && i.IsPaid == 0) // Only get pending/due
            .AsNoTracking();

        if (request.EmployeeId.HasValue)
        {
            query = query.Where(i => i.Loan.EmployeeId == request.EmployeeId);
        }

        var installments = await query.ToListAsync(cancellationToken);

        var dtos = installments.Select(i => new LoanInstallmentDto
        {
            InstallmentId = (int)i.InstallmentId,
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
