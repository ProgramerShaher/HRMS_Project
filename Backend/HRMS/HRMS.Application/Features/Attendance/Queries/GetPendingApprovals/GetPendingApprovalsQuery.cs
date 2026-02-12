using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HRMS.Application.Features.Attendance.Queries.PendingApprovals
{
    /// <summary>
    /// استعلام جلب كافة الطلبات المعلقة (عمل إضافي، تبديل، استئذان) للمدراء
    /// </summary>
    public class GetPendingApprovalsQuery : IRequest<Result<List<PendingApprovalDto>>>
    {
    }

    /// <summary>
    /// البيانات الموحدة للطلبات المعلقة
    /// </summary>
    public class PendingApprovalDto
    {
        public int Id { get; set; }
        public string RequestType { get; set; } = string.Empty; // 'Overtime', 'Swap', 'Permission'
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public DateTime RequestDate { get; set; }
        public DateTime TargetDate { get; set; }
        public string Details { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }

    /// <summary>
    /// معالج استعلام كافة الطلبات المعلقة
    /// </summary>
    public class GetPendingApprovalsQueryHandler : IRequestHandler<GetPendingApprovalsQuery, Result<List<PendingApprovalDto>>>
    {
        private readonly IApplicationDbContext _context;

        public GetPendingApprovalsQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<List<PendingApprovalDto>>> Handle(GetPendingApprovalsQuery request, CancellationToken cancellationToken)
        {
            var result = new List<PendingApprovalDto>();

            // 1. Fetch pending Overtime requests
            var overtimeRequests = await _context.OvertimeRequests
                .Include(x => x.Employee)
                .AsNoTracking()
                .Where(x => x.Status == "PENDING")
                .Select(x => new PendingApprovalDto
                {
                    Id = x.OtRequestId,
                    RequestType = "Overtime",
                    EmployeeId = x.EmployeeId,
                    EmployeeName = x.Employee.FullNameAr,
                    RequestDate = x.RequestDate,
                    TargetDate = x.WorkDate,
                    Details = $"{x.HoursRequested} ساعات | {x.Reason}",
                    Status = x.Status
                })
                .ToListAsync(cancellationToken);

            result.AddRange(overtimeRequests);

            // 2. Fetch pending Shift Swap requests
            var swapRequests = await _context.ShiftSwapRequests
                .Include(x => x.Requester)
                .AsNoTracking()
                .Where(x => x.Status == "PENDING")
                .Select(x => new PendingApprovalDto
                {
                    Id = x.RequestId,
                    RequestType = "Swap",
                    EmployeeId = x.RequesterId,
                    EmployeeName = x.Requester.FullNameAr,
                    RequestDate = x.CreatedAt,
                    TargetDate = x.RosterDate,
                    Details = $"تبديل مناوبة",
                    Status = x.Status
                })
                .ToListAsync(cancellationToken);

            result.AddRange(swapRequests);

            // 3. Fetch pending Permission requests
            var permissionRequests = await _context.PermissionRequests
                .Include(x => x.Employee)
                .AsNoTracking()
                .Where(x => x.Status == "Pending") // Uses 'Pending' 
                .Select(x => new PendingApprovalDto
                {
                    Id = x.PermissionRequestId,
                    RequestType = "Permission",
                    EmployeeId = x.EmployeeId,
                    EmployeeName = x.Employee.FullNameAr,
                    RequestDate = x.CreatedAt,
                    TargetDate = x.PermissionDate,
                    Details = $"{x.PermissionType} | {x.Hours} ساعات | {x.Reason}",
                    Status = x.Status
                })
                .ToListAsync(cancellationToken);

            result.AddRange(permissionRequests);

            // Sort by RequestDate descending
            var sortedResult = result.OrderByDescending(x => x.RequestDate).ToList();

            return Result<List<PendingApprovalDto>>.Success(sortedResult);
        }
    }
}
