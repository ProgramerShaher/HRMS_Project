using HRMS.Application.DTOs.Leaves;
using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Leaves.LeaveBalances.Queries.GetEmployeesBalances;

public record GetEmployeesBalancesQuery : IRequest<Result<List<EmployeeLeaveTypeBalanceDto>>>
{
    public short? Year { get; init; }
    public int? DepartmentId { get; init; }
    public int? EmployeeId { get; init; }
    public string? Search { get; init; }
}

public class GetEmployeesBalancesQueryHandler : IRequestHandler<GetEmployeesBalancesQuery, Result<List<EmployeeLeaveTypeBalanceDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetEmployeesBalancesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<EmployeeLeaveTypeBalanceDto>>> Handle(GetEmployeesBalancesQuery request, CancellationToken cancellationToken)
    {
        var year = request.Year ?? (short)DateTime.Today.Year;
        var yearStart = new DateTime(year, 1, 1);
        var yearEnd = yearStart.AddYears(1);

        var employeesQuery = _context.Employees
            .AsNoTracking()
            .Include(e => e.Department)
            .Where(e => e.IsDeleted == 0 && e.IsActive);

        if (request.DepartmentId.HasValue)
            employeesQuery = employeesQuery.Where(e => e.DepartmentId == request.DepartmentId.Value);

        if (request.EmployeeId.HasValue)
            employeesQuery = employeesQuery.Where(e => e.EmployeeId == request.EmployeeId.Value);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var s = request.Search.Trim();
            employeesQuery = employeesQuery.Where(e =>
                e.EmployeeNumber.Contains(s) ||
                e.FirstNameAr.Contains(s) ||
                e.SecondNameAr.Contains(s) ||
                e.ThirdNameAr.Contains(s) ||
                e.LastNameAr.Contains(s));
        }

        // Fetch balances for target year.
        var balancesQuery =
            from b in _context.EmployeeLeaveBalances.AsNoTracking()
            join e in employeesQuery on b.EmployeeId equals e.EmployeeId
            join lt in _context.LeaveTypes.AsNoTracking() on b.LeaveTypeId equals lt.LeaveTypeId
            where b.IsDeleted == 0 && lt.IsDeleted == 0 && b.Year == year
            select new
            {
                b.EmployeeId,
                EmployeeNumber = e.EmployeeNumber,
                EmployeeNameAr = (e.FirstNameAr + " " + e.SecondNameAr + " " + e.ThirdNameAr + " " + e.LastNameAr).Trim(),
                DepartmentId = e.DepartmentId,
                DepartmentNameAr = e.Department.DeptNameAr,
                b.LeaveTypeId,
                LeaveTypeNameAr = lt.LeaveNameAr,
                b.Year,
                RemainingDays = b.CurrentBalance
            };

        var balances = await balancesQuery.ToListAsync(cancellationToken);

        if (balances.Count == 0)
        {
            return Result<List<EmployeeLeaveTypeBalanceDto>>.Success(
                new List<EmployeeLeaveTypeBalanceDto>(),
                "لا توجد أرصدة إجازات مهيأة للموظفين في السنة المحددة");
        }

        var employeeIds = balances.Select(b => b.EmployeeId).Distinct().ToList();
        var leaveTypeIds = balances.Select(b => b.LeaveTypeId).Distinct().ToList();

        // Net consumption in the year based on transactions:
        // DEDUCTION is negative days, CANCELLATION is positive days (reversal).
        // Net consumed = -(sum(Days)) for these two types.
        var tx = await _context.LeaveTransactions
            .AsNoTracking()
            .Where(t => t.IsDeleted == 0)
            .Where(t => employeeIds.Contains(t.EmployeeId))
            .Where(t => leaveTypeIds.Contains(t.LeaveTypeId))
            .Where(t => t.TransactionDate >= yearStart && t.TransactionDate < yearEnd)
            .Where(t => t.TransactionType == "DEDUCTION" || t.TransactionType == "CANCELLATION")
            .GroupBy(t => new { t.EmployeeId, t.LeaveTypeId })
            .Select(g => new
            {
                g.Key.EmployeeId,
                g.Key.LeaveTypeId,
                SumDays = g.Sum(x => x.Days)
            })
            .ToListAsync(cancellationToken);

        var consumedMap = tx.ToDictionary(
            x => (x.EmployeeId, x.LeaveTypeId),
            x => Math.Max(0, -x.SumDays));

        var result = balances.Select(b =>
        {
            var consumed = consumedMap.TryGetValue((b.EmployeeId, b.LeaveTypeId), out var c) ? c : 0m;
            return new EmployeeLeaveTypeBalanceDto
            {
                EmployeeId = b.EmployeeId,
                EmployeeNumber = b.EmployeeNumber,
                EmployeeNameAr = b.EmployeeNameAr,
                DepartmentId = b.DepartmentId,
                DepartmentNameAr = b.DepartmentNameAr,
                LeaveTypeId = b.LeaveTypeId,
                LeaveTypeNameAr = b.LeaveTypeNameAr,
                Year = b.Year,
                RemainingDays = b.RemainingDays,
                ConsumedDays = consumed,
                EntitlementDays = b.RemainingDays + consumed
            };
        }).OrderBy(x => x.DepartmentNameAr).ThenBy(x => x.EmployeeNumber).ThenBy(x => x.LeaveTypeNameAr).ToList();

        return Result<List<EmployeeLeaveTypeBalanceDto>>.Success(result);
    }
}
