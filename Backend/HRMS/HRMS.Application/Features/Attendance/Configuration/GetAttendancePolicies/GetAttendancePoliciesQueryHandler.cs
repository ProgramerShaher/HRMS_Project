using HRMS.Application.DTOs.Attendance;
using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Attendance.Configuration.GetAttendancePolicies;

/// <summary>
/// Handler for retrieving all attendance policies.
/// Returns only active (non-deleted) policies ordered by ID.
/// </summary>
public class GetAttendancePoliciesQueryHandler : IRequestHandler<GetAttendancePoliciesQuery, Result<List<AttendancePolicyDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetAttendancePoliciesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<AttendancePolicyDto>>> Handle(
        GetAttendancePoliciesQuery request, 
        CancellationToken cancellationToken)
    {
        // ═══════════════════════════════════════════════════════════
        // جلب جميع السياسات النشطة
        // Fetch all active policies
        // نستخدم AsNoTracking لتحسين الأداء لأننا نقرأ فقط
        // Use AsNoTracking for better performance since we're only reading
        // ═══════════════════════════════════════════════════════════
        
        var policies = await _context.AttendancePolicies
            .AsNoTracking()
            .Where(p => p.IsDeleted == 0)
            .OrderBy(p => p.PolicyId)
            .Select(p => new AttendancePolicyDto
            {
                PolicyId = p.PolicyId,
                PolicyNameAr = p.PolicyNameAr,
                DeptId = p.DeptId,
                JobId = p.JobId,
                LateGraceMins = p.LateGraceMins,
                OvertimeMultiplier = p.OvertimeMultiplier,
                WeekendOtMultiplier = p.WeekendOtMultiplier
            })
            .ToListAsync(cancellationToken);

        return Result<List<AttendancePolicyDto>>.Success(policies);
    }
}
