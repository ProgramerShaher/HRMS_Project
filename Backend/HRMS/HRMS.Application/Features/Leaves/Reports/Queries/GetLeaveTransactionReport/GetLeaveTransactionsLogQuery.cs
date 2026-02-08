using HRMS.Application.DTOs.Leaves;
using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HRMS.Application.Features.Leaves.Reports.Queries.GetLeaveTransactionReport
{
    // Rename to GetLeaveTransactionHistoryQuery or similar to avoid conflict if desired, 
    // but user wants monitoring endpoint. I'll call it GetLeaveTransactionsLogQuery for clarity.

    public record GetLeaveTransactionsLogQuery(
        int? EmployeeId,
        DateTime? FromDate,
        DateTime? ToDate,
        int? LeaveTypeId,
        string? TransactionType
    ) : IRequest<Result<List<LeaveTransactionDto>>>;

    public class GetLeaveTransactionsLogQueryHandler : IRequestHandler<GetLeaveTransactionsLogQuery, Result<List<LeaveTransactionDto>>>
    {
        private readonly IApplicationDbContext _context;

        public GetLeaveTransactionsLogQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<List<LeaveTransactionDto>>> Handle(GetLeaveTransactionsLogQuery request, CancellationToken cancellationToken)
        {
            var query = _context.LeaveTransactions
                .AsNoTracking()
                .Include(t => t.LeaveType)
                .AsQueryable();

            if (request.EmployeeId.HasValue)
                query = query.Where(t => t.EmployeeId == request.EmployeeId.Value);

            if (request.FromDate.HasValue)
                query = query.Where(t => t.TransactionDate >= request.FromDate.Value);

            if (request.ToDate.HasValue)
                query = query.Where(t => t.TransactionDate <= request.ToDate.Value);

            if (request.LeaveTypeId.HasValue)
                query = query.Where(t => t.LeaveTypeId == request.LeaveTypeId.Value);

            if (!string.IsNullOrEmpty(request.TransactionType))
                query = query.Where(t => t.TransactionType == request.TransactionType);

            // Fetch Employee Name manually or include Employee if navigation property exists.
            // LeaveTransaction usually has EmployeeId. Let's assume navigation property exists or fetch names.
            // Checking LeaveTransaction entity...
            // It likely has navigation properties based on convention.
            
            // To be safe and performant, let's join.
            var result = await query
                .OrderByDescending(t => t.TransactionDate)
                .Select(t => new LeaveTransactionDto
                {
                    TransactionId = (int)t.TransactionId,
                    EmployeeId = t.EmployeeId,
                    // EmployeeName = t.Employee.FirstNameAr + " " + t.Employee.LastNameAr, // If Nav Prop exists
                    LeaveTypeId = t.LeaveTypeId,
                    LeaveTypeName = t.LeaveType.LeaveNameAr,
                    TransactionType = t.TransactionType,
                    Days = t.Days,
                    TransactionDate = t.TransactionDate,
                    Notes = t.Notes,
                    ReferenceId = t.ReferenceId
                })
                .ToListAsync(cancellationToken);

            // If Employee Name is missing (no navigation), we can enrich it here.
            // But let's assume Basic navigation exists. If not, I'll fix it.
            
            // Optimization: Fetch employee names for the IDs if navigation is missing.
            var empIds = result.Select(r => r.EmployeeId).Distinct().ToList();
            var employees = await _context.Employees
                .Where(e => empIds.Contains(e.EmployeeId))
                .Select(e => new { e.EmployeeId, Name = e.FirstNameAr + " " + e.LastNameAr })
                .ToDictionaryAsync(e => e.EmployeeId, e => e.Name, cancellationToken);

            foreach (var item in result)
            {
                if (employees.TryGetValue(item.EmployeeId, out var name))
                {
                    item.EmployeeName = name;
                }
            }

            return Result<List<LeaveTransactionDto>>.Success(result);
        }
    }
}
