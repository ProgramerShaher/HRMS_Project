using HRMS.Application.DTOs.Payroll;
using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Payroll.Loans.Queries.GetPendingLoans;

/// <summary>
/// الحصول على جميع السلف المعلقة للموافقة
/// Get all pending loans for approval
/// </summary>
public class GetPendingLoansQuery : IRequest<Result<List<LoanDto>>>
{
    public int? DepartmentId { get; set; }
}

public class GetPendingLoansQueryHandler : IRequestHandler<GetPendingLoansQuery, Result<List<LoanDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetPendingLoansQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<LoanDto>>> Handle(GetPendingLoansQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // استعلام السلف المعلقة
            // Query pending loans
            var query = _context.Loans
                .Include(l => l.Employee)
                    .ThenInclude(e => e.Department)
                .Include(l => l.Installments)
                .Where(l => l.IsDeleted == 0 && l.Status == "PENDING")
                .AsQueryable();

            // تطبيق فلتر القسم
            // Apply department filter
            if (request.DepartmentId.HasValue)
            {
                query = query.Where(l => l.Employee.DepartmentId == request.DepartmentId.Value);
            }

            // تنفيذ الاستعلام
            // Execute query
            var loans = await query
                .OrderBy(l => l.RequestDate)
                .ToListAsync(cancellationToken);

            // تحويل إلى DTO
            // Map to DTO
            var result = loans.Select(l => new LoanDto
            {
                LoanId = l.LoanId,
                EmployeeId = l.EmployeeId,
                EmployeeName = l.Employee.FullNameAr,
                LoanAmount = l.LoanAmount,
                InstallmentCount = l.InstallmentCount,
                Status = l.Status,
                RequestDate = l.RequestDate,
                ApprovalDate = l.ApprovalDate
            }).ToList();

            return Result<List<LoanDto>>.Success(result);
        }
        catch (Exception ex)
        {
            return Result<List<LoanDto>>.Failure($"Error retrieving pending loans: {ex.Message}");
        }
    }
}
