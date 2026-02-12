using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HRMS.Application.Features.Attendance.Queries.GetPendingSwapRequests
{
    /// <summary>
    /// استعلام جلب طلبات تبديل المناوبات المعلقة للمدراء
    /// </summary>
    public class GetPendingSwapRequestsQuery : IRequest<Result<List<PendingSwapRequestDto>>>
    {
    }

    /// <summary>
    /// البيانات المعادة لطلبات تبديل المناوبات
    /// </summary>
    public class PendingSwapRequestDto
    {
        public int RequestId { get; set; }
        public string RequesterName { get; set; } = string.Empty;
        public string TargetEmployeeName { get; set; } = string.Empty;
        public string CurrentShift { get; set; } = string.Empty;
        public string TargetShift { get; set; } = string.Empty;
        public DateTime RosterDate { get; set; }
    }

    /// <summary>
    /// معالج استعلام طلبات التبديل المعلقة
    /// </summary>
    public class GetPendingSwapRequestsQueryHandler : IRequestHandler<GetPendingSwapRequestsQuery, Result<List<PendingSwapRequestDto>>>
    {
        private readonly IApplicationDbContext _context;

        public GetPendingSwapRequestsQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<List<PendingSwapRequestDto>>> Handle(GetPendingSwapRequestsQuery request, CancellationToken cancellationToken)
        {
            // جلب طلبات التبديل المعلقة مع الموظفين
            var pendingRequests = await _context.ShiftSwapRequests
                .Include(x => x.Requester)
                .Include(x => x.TargetEmployee)
                .AsNoTracking()
                .Where(x => x.Status == "PENDING")
                .ToListAsync(cancellationToken);

            var result = new List<PendingSwapRequestDto>();

            foreach (var req in pendingRequests)
            {
                // جلب الـ Roster لكل موظف لمعرفة المناوبة
                var requesterRoster = await _context.EmployeeRosters
                    .Include(r => r.ShiftType)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(r => r.EmployeeId == req.RequesterId && r.RosterDate == req.RosterDate, cancellationToken);

                var targetRoster = await _context.EmployeeRosters
                    .Include(r => r.ShiftType)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(r => r.EmployeeId == req.TargetEmployeeId && r.RosterDate == req.RosterDate, cancellationToken);

                result.Add(new PendingSwapRequestDto
                {
                    RequestId = req.RequestId,
                    RequesterName = req.Requester.FullNameAr,
                    TargetEmployeeName = req.TargetEmployee.FullNameAr,
                    CurrentShift = requesterRoster?.ShiftType?.ShiftNameAr ?? (requesterRoster?.IsOffDay == 1 ? "يوم راحة" : "غير محدد"),
                    TargetShift = targetRoster?.ShiftType?.ShiftNameAr ?? (targetRoster?.IsOffDay == 1 ? "يوم راحة" : "غير محدد"),
                    RosterDate = req.RosterDate
                });
            }

            return Result<List<PendingSwapRequestDto>>.Success(result);
        }
    }
}
