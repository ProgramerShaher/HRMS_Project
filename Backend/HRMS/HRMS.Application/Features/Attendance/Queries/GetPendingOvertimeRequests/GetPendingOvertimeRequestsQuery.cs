using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HRMS.Application.Features.Attendance.Queries.GetPendingOvertimeRequests
{
    /// <summary>
    /// استعلام جلب طلبات العمل الإضافي المعلقة للمدراء
    /// </summary>
    public class GetPendingOvertimeRequestsQuery : IRequest<Result<List<PendingOvertimeRequestDto>>>
    {
    }

    /// <summary>
    /// البيانات المعادة لطلبات العمل الإضافي
    /// </summary>
    public class PendingOvertimeRequestDto
    {
        public int RequestId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public DateTime RequestDate { get; set; }
        public decimal HoursRequested { get; set; }
        public string Reason { get; set; } = string.Empty;
    }

    /// <summary>
    /// معالج استعلام طلبات العمل الإضافي المعلقة
    /// </summary>
    public class GetPendingOvertimeRequestsQueryHandler : IRequestHandler<GetPendingOvertimeRequestsQuery, Result<List<PendingOvertimeRequestDto>>>
    {
        private readonly IApplicationDbContext _context;

        public GetPendingOvertimeRequestsQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<List<PendingOvertimeRequestDto>>> Handle(GetPendingOvertimeRequestsQuery request, CancellationToken cancellationToken)
        {
            var requests = await _context.OvertimeRequests
                .Include(x => x.Employee)
                .AsNoTracking()
                .Where(x => x.Status == "PENDING")
                .OrderByDescending(x => x.RequestDate)
                .Select(x => new PendingOvertimeRequestDto
                {
                    RequestId = x.OtRequestId,
                    EmployeeName = x.Employee.FullNameAr,
                    RequestDate = x.RequestDate,
                    HoursRequested = x.HoursRequested,
                    Reason = x.Reason ?? string.Empty
                })
                .ToListAsync(cancellationToken);

            return Result<List<PendingOvertimeRequestDto>>.Success(requests);
        }
    }
}
