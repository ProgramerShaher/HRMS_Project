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
                .Include(t => t.Employee)
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

            var result = await query
                .OrderByDescending(t => t.TransactionDate)
                .Select(t => new LeaveTransactionDto
                {
                    TransactionId = (int)t.TransactionId,
                    EmployeeId = t.EmployeeId,
                    EmployeeName = t.Employee.FirstNameAr + " " + t.Employee.LastNameAr,
                    LeaveTypeId = t.LeaveTypeId,
                    LeaveTypeName = t.LeaveType.LeaveNameAr,
                    TransactionType = t.TransactionType,
                    Days = t.Days,
                    TransactionDate = t.TransactionDate,
                    Notes = t.Notes,
                    ReferenceId = t.ReferenceId
                })
                .ToListAsync(cancellationToken);

            return Result<List<LeaveTransactionDto>>.Success(result);
        }
    }
}
