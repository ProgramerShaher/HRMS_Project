using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;

namespace HRMS.Application.Features.Attendance.Configuration.UpdateAttendancePolicy;

/// <summary>
/// Handler for updating attendance policy.
/// Updates policy configuration and invalidates cache.
/// </summary>
public class UpdateAttendancePolicyCommandHandler : IRequestHandler<UpdateAttendancePolicyCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;
    private readonly IAttendancePolicyService _policyService;

    public UpdateAttendancePolicyCommandHandler(
        IApplicationDbContext context,
        IAttendancePolicyService policyService)
    {
        _context = context;
        _policyService = policyService;
    }

    public async Task<Result<bool>> Handle(
        UpdateAttendancePolicyCommand request, 
        CancellationToken cancellationToken)
    {
        // ═══════════════════════════════════════════════════════════
        // جلب السياسة المراد تعديلها
        // Fetch policy to update
        // ═══════════════════════════════════════════════════════════
        
        var policy = await _context.AttendancePolicies
            .FindAsync(new object[] { request.PolicyId }, cancellationToken);

        if (policy == null)
            return Result<bool>.Failure("السياسة غير موجودة");

        // ═══════════════════════════════════════════════════════════
        // تحديث البيانات
        // Update policy data
        // ═══════════════════════════════════════════════════════════
        
        policy.PolicyNameAr = request.PolicyNameAr;
        policy.DeptId = request.DeptId;
        policy.JobId = request.JobId;
        policy.LateGraceMins = request.LateGraceMins;
        policy.OvertimeMultiplier = request.OvertimeMultiplier;
        policy.WeekendOtMultiplier = request.WeekendOtMultiplier;

        await _context.SaveChangesAsync(cancellationToken);

        // ═══════════════════════════════════════════════════════════
        // مسح الـ Cache بعد التعديل
        // Clear cache to ensure updated policy is fetched next time
        // ═══════════════════════════════════════════════════════════
        
        _policyService.ClearCache();

        return Result<bool>.Success(true, "تم تعديل سياسة الحضور بنجاح");
    }
}
