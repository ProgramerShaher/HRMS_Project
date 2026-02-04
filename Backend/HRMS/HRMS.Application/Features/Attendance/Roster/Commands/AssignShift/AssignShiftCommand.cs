using HRMS.Application.Interfaces;
using HRMS.Core.Entities.Attendance;
using HRMS.Core.Utilities; // For Result<T>
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Attendance.Roster.Commands.AssignShift;

public class AssignShiftCommand : IRequest<Result<int>>
{
    public int EmployeeId { get; set; }
    public int ShiftId { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
    public List<int>? OffDays { get; set; } // Optional: e.g. [5, 6] for Fri/Sat
}

public class AssignShiftCommandHandler : IRequestHandler<AssignShiftCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;

    public AssignShiftCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<int>> Handle(AssignShiftCommand request, CancellationToken cancellationToken)
    {
        // 1. Validate Shift Exists
        var shift = await _context.ShiftTypes.FindAsync(new object[] { request.ShiftId }, cancellationToken);
        if (shift == null) return Result<int>.Failure("المناوبة غير موجودة");

        // 2. Define Date Range
        var startDate = new DateTime(request.Year, request.Month, 1);
        var endDate = startDate.AddMonths(1).AddDays(-1);

        // 3. Remove Existing Roster for this period (to allow re-assignment)
        var existingRoster = await _context.EmployeeRosters
            .Where(r => r.EmployeeId == request.EmployeeId 
                     && r.RosterDate >= startDate && r.RosterDate <= endDate)
            .ToListAsync(cancellationToken);
        
        _context.EmployeeRosters.RemoveRange(existingRoster);

        // 4. Generate New Roster
        var rosterEntries = new List<EmployeeRoster>();
        for (var date = startDate; date <= endDate; date = date.AddDays(1))
        {
            // Determine if Off Day (By DayOfWeek enum value: Sun=0, Sat=6)
            bool isOffDay = request.OffDays != null && request.OffDays.Contains((int)date.DayOfWeek);

            rosterEntries.Add(new EmployeeRoster
            {
                EmployeeId = request.EmployeeId,
                RosterDate = date,
                ShiftId = isOffDay ? null : request.ShiftId,
                IsOffDay = (byte)(isOffDay ? 1 : 0),
                Status = "Scheduled"
            });
        }

        await _context.EmployeeRosters.AddRangeAsync(rosterEntries, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<int>.Success(rosterEntries.Count, $"تم جدولة المناوبة لـ {rosterEntries.Count} يوم");
    }
}
