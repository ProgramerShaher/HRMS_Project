using HRMS.Application.DTOs.Payroll;
using HRMS.Application.Interfaces;
using HRMS.Core.Entities.Payroll;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Payroll.Loans.Queries.GetAllLoans;

/// <summary>
/// الحصول على جميع السلف (للإدارة)
/// Get all loans (for admin)
/// </summary>
public class GetAllLoansQuery : IRequest<Result<List<LoanDto>>>
{
    public string? Status { get; set; }
    public int? EmployeeId { get; set; }
    public int? DepartmentId { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
}

public class GetAllLoansQueryHandler : IRequestHandler<GetAllLoansQuery, Result<List<LoanDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetAllLoansQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<LoanDto>>> Handle(GetAllLoansQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Loans
            .Include(l => l.Employee)
                .ThenInclude(e => e.Department)
            .Include(l => l.Installments)
            .AsNoTracking();

        // تطبيق الفلاتر
        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            query = query.Where(l => l.Status == request.Status);
        }

        if (request.EmployeeId.HasValue)
        {
            query = query.Where(l => l.EmployeeId == request.EmployeeId.Value);
        }

        if (request.DepartmentId.HasValue)
        {
            query = query.Where(l => l.Employee.DepartmentId == request.DepartmentId.Value);
        }

        if (request.DateFrom.HasValue)
        {
            query = query.Where(l => l.RequestDate >= request.DateFrom.Value);
        }

        if (request.DateTo.HasValue)
        {
            query = query.Where(l => l.RequestDate <= request.DateTo.Value);
        }

        var loans = await query
            .OrderByDescending(l => l.LoanId)
            .ToListAsync(cancellationToken);

        var result = loans.Select(l =>
        {
            // حساب المبلغ المدفوع والمتبقي من الأقساط
            var paidInstallments = l.Installments?.Where(i => i.IsPaid == 1).ToList() ?? new List<LoanInstallment>();
            var paidAmount = paidInstallments.Sum(i => i.Amount);
            var remainingAmount = l.LoanAmount - paidAmount;

            return new LoanDto
            {
                LoanId = l.LoanId,
                EmployeeId = l.EmployeeId,
                EmployeeName = l.Employee.FullNameAr,
                LoanAmount = l.LoanAmount,
                RequestDate = l.RequestDate,
                InstallmentCount = l.InstallmentCount,
                Status = l.Status,
                ApprovalDate = l.ApprovalDate,
                ApprovedBy = l.ApprovedBy,
                SettlementDate = l.SettlementDate,
                SettlementNotes = l.SettlementNotes,
                RemainingAmount = remainingAmount,
                PaidAmount = paidAmount,
                Installments = l.Installments?.Select(i => new LoanInstallmentDto
                {
                    InstallmentId = i.InstallmentId,
                    LoanId = i.LoanId,
                    EmployeeId = l.EmployeeId, // من الـ Loan وليس من الـ Installment
                    InstallmentNumber = i.InstallmentNumber,
                    Amount = i.Amount,
                    DueDate = i.DueDate,
                    IsPaid = i.IsPaid == 1,
                    PaidDate = i.PaidDate,
                    PaidInPayrollRun = i.PaidInPayrollRun
                }).ToList()
            };
        }).ToList();

        return Result<List<LoanDto>>.Success(result);
    }
}
