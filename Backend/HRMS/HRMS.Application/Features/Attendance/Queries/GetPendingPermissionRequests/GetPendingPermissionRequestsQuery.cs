using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HRMS.Application.Features.Attendance.Queries.GetPendingPermissionRequests
{
    /// <summary>
    /// استعلام جلب طلبات الاستئذان المعلقة للمدراء
    /// </summary>
    public class GetPendingPermissionRequestsQuery : IRequest<Result<List<PendingPermissionRequestDto>>>
    {
    }

    /// <summary>
    /// البيانات المعادة لطلبات الاستئذان
    /// </summary>
    public class PendingPermissionRequestDto
    {
        public int RequestId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public string PermissionType { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public decimal Hours { get; set; }
    }

    /// <summary>
    /// معالج استعلام طلبات الاستئذان المعلقة
    /// </summary>
    public class GetPendingPermissionRequestsQueryHandler : IRequestHandler<GetPendingPermissionRequestsQuery, Result<List<PendingPermissionRequestDto>>>
    {
        private readonly IApplicationDbContext _context;

        public GetPendingPermissionRequestsQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<List<PendingPermissionRequestDto>>> Handle(GetPendingPermissionRequestsQuery request, CancellationToken cancellationToken)
        {
            var requests = await _context.PermissionRequests
                .Include(x => x.Employee)
                .AsNoTracking()
                .Where(x => x.Status == "Pending") // ملاحظة: PermissionRequest يستخدم 'Pending' بحروف صغيرة/مختلطة في الكود السابق
                .OrderByDescending(x => x.PermissionDate)
                .Select(x => new PendingPermissionRequestDto
                {
                    RequestId = x.PermissionRequestId,
                    EmployeeName = x.Employee.FullNameAr,
                    PermissionType = x.PermissionType,
                    Date = x.PermissionDate,
                    Hours = x.Hours
                })
                .ToListAsync(cancellationToken);

            return Result<List<PendingPermissionRequestDto>>.Success(requests);
        }
    }
}
