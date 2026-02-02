using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;

namespace HRMS.Application.Features.Attendance.Configuration.DeleteAttendancePolicy;

/// <summary>
/// Handler for deleting attendance policy.
/// Performs soft delete and invalidates cache.
/// </summary>
public class DeleteAttendancePolicyCommandHandler : IRequestHandler<DeleteAttendancePolicyCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;
    private readonly IAttendancePolicyService _policyService;

    public DeleteAttendancePolicyCommandHandler(
        IApplicationDbContext context,
        IAttendancePolicyService policyService)
    {
        _context = context;
        _policyService = policyService;
    }

    public async Task<Result<bool>> Handle(
        DeleteAttendancePolicyCommand request, 
        CancellationToken cancellationToken)
    {
        // ═══════════════════════════════════════════════════════════
        // جلب السياسة المراد حذفها
        // Fetch policy to delete
        // ═══════════════════════════════════════════════════════════
        
        var policy = await _context.AttendancePolicies
            .FindAsync(new object[] { request.PolicyId }, cancellationToken);

        if (policy == null)
            return Result<bool>.Failure("السياسة غير موجودة");

        // ═══════════════════════════════════════════════════════════
        // حذف ناعم (Soft Delete)
        // Soft delete - mark as deleted without removing from database
        // هذا يحافظ على البيانات التاريخية ويمنع فقدان المراجع
        // This preserves historical data and prevents broken references
        // ═══════════════════════════════════════════════════════════
        
        policy.IsDeleted = 1;
        await _context.SaveChangesAsync(cancellationToken);

        // ═══════════════════════════════════════════════════════════
        // مسح الـ Cache بعد الحذف
        // Clear cache after deletion
        // ═══════════════════════════════════════════════════════════
        
        _policyService.ClearCache();

        return Result<bool>.Success(true, "تم حذف سياسة الحضور بنجاح");
    }
}
