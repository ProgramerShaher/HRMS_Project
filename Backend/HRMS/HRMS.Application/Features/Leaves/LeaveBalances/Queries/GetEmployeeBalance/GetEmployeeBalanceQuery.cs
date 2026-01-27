using AutoMapper;
using HRMS.Application.DTOs.Leaves;
using HRMS.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Leaves.LeaveBalances.Queries.GetEmployeeBalance;

// 1. Query
/// <summary>
/// استعلام جلب رصيد إجازات موظف لسنة معينة
/// </summary>
public record GetEmployeeBalanceQuery(int EmployeeId, short? Year) : IRequest<List<LeaveBalanceDto>>;

// 2. Handler
public class GetEmployeeBalanceQueryHandler : IRequestHandler<GetEmployeeBalanceQuery, List<LeaveBalanceDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetEmployeeBalanceQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<LeaveBalanceDto>> Handle(GetEmployeeBalanceQuery request, CancellationToken cancellationToken)
    {
        var targetYear = request.Year ?? (short)DateTime.Now.Year;

        var balances = await _context.LeaveBalances
            .AsNoTracking()
            .Include(b => b.LeaveType)
            .Include(b => b.Employee)
            .Where(b => b.EmployeeId == request.EmployeeId && b.Year == targetYear)
            .ToListAsync(cancellationToken);

        return _mapper.Map<List<LeaveBalanceDto>>(balances);
    }
}
