using HRMS.Application.Interfaces;
using HRMS.Core.Entities.Attendance;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Attendance.Configuration.CreateAttendancePolicy;

/// <summary>
/// Handler for creating attendance policy.
/// Implements duplicate prevention for default policies and cache invalidation.
/// </summary>
public class CreateAttendancePolicyCommandHandler : IRequestHandler<CreateAttendancePolicyCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;
    private readonly IAttendancePolicyService _policyService;

    public CreateAttendancePolicyCommandHandler(
        IApplicationDbContext context,
        IAttendancePolicyService policyService)
    {
        _context = context;
        _policyService = policyService;
    }

    public async Task<Result<int>> Handle(
        CreateAttendancePolicyCommand request, 
        CancellationToken cancellationToken)
    {
        // ═══════════════════════════════════════════════════════════
        // التحقق من عدم وجود سياسة افتراضية مكررة
        // Prevent duplicate default policy
        // ═══════════════════════════════════════════════════════════
        
        // السياسة الافتراضية هي التي لا تحتوي على قسم أو وظيفة محددة
        // Default policy is one without specific department or job
        if (request.DeptId == null && request.JobId == null)
        {
            var existingDefault = await _context.AttendancePolicies
                .Where(p => p.DeptId == null && p.JobId == null && p.IsDeleted == 0)
                .AnyAsync(cancellationToken);

            if (existingDefault)
                return Result<int>.Failure("يوجد بالفعل سياسة افتراضية. يمكنك تعديلها أو حذفها أولاً.");
        }

        // ═══════════════════════════════════════════════════════════
        // إنشاء السياسة الجديدة
        // Create new policy
        // ═══════════════════════════════════════════════════════════
        
        var policy = new AttendancePolicy
        {
            PolicyNameAr = request.PolicyNameAr,
            DeptId = request.DeptId,
            JobId = request.JobId,
            LateGraceMins = request.LateGraceMins,
            OvertimeMultiplier = request.OvertimeMultiplier,
            WeekendOtMultiplier = request.WeekendOtMultiplier
        };

        _context.AttendancePolicies.Add(policy);
        await _context.SaveChangesAsync(cancellationToken);

        // ═══════════════════════════════════════════════════════════
        // مسح الـ Cache بعد الإضافة
        // Clear cache after adding to ensure fresh data on next retrieval
        // ═══════════════════════════════════════════════════════════
        
        _policyService.ClearCache();

        return Result<int>.Success(policy.PolicyId, "تم إضافة سياسة الحضور بنجاح");
    }
}
