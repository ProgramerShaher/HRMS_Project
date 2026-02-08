using AutoMapper;
using HRMS.Application.DTOs.Payroll;
using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Payroll.Loans.Queries.GetEmployeeLoans;

/// <summary>
/// Get Employee Loans Query
/// </summary>
public class GetEmployeeLoansQuery : IRequest<Result<List<LoanDto>>>
{
    public int EmployeeId { get; set; }
    public string? Status { get; set; }
}

/// <summary>
/// Get Employee Loans Query Handler
/// </summary>
public class GetEmployeeLoansQueryHandler : IRequestHandler<GetEmployeeLoansQuery, Result<List<LoanDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetEmployeeLoansQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<List<LoanDto>>> Handle(GetEmployeeLoansQuery request, CancellationToken cancellationToken)
    {
        // بناء الاستعلام
        var query = _context.Loans
            .Include(l => l.Employee)
            .Include(l => l.Installments)
            .Where(l => l.EmployeeId == request.EmployeeId && l.IsDeleted == 0);

        // تصفية حسب الحالة إن وجدت
        if (!string.IsNullOrEmpty(request.Status))
        {
            query = query.Where(l => l.Status == request.Status);
        }

        var loans = await query
            .OrderByDescending(l => l.RequestDate)
            .ToListAsync(cancellationToken);

        var dtos = _mapper.Map<List<LoanDto>>(loans);

        return Result<List<LoanDto>>.Success(dtos);
    }
}
