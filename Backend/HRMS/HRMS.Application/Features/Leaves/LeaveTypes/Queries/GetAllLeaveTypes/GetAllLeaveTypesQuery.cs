using HRMS.Application.DTOs.Leaves;
using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Leaves.LeaveTypes.Queries.GetAllLeaveTypes;

// ═══════════════════════════════════════════════════════════════════════════
// 1. QUERY - الاستعلام
// ═══════════════════════════════════════════════════════════════════════════

/// <summary>
/// Query to get all leave types.
/// Returns list of leave types wrapped in Result pattern.
/// </summary>
public record GetAllLeaveTypesQuery : IRequest<Result<List<LeaveTypeDto>>>;

// ═══════════════════════════════════════════════════════════════════════════
// 2. HANDLER - معالج الاستعلام
// ═══════════════════════════════════════════════════════════════════════════

/// <summary>
/// Handler for getting all leave types.
/// Uses AsNoTracking for performance and direct DTO projection.
/// </summary>
public class GetAllLeaveTypesQueryHandler : IRequestHandler<GetAllLeaveTypesQuery, Result<List<LeaveTypeDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetAllLeaveTypesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<LeaveTypeDto>>> Handle(GetAllLeaveTypesQuery request, CancellationToken cancellationToken)
    {
        // ═══════════════════════════════════════════════════════════════════════════
        // الخطوة 1: جلب جميع أنواع الإجازات النشطة
        // Step 1: Get all active leave types
        // ═══════════════════════════════════════════════════════════════════════════

        // نستخدم AsNoTracking لتحسين الأداء (قراءة فقط)
        // نستخدم Direct DTO Projection لتقليل استهلاك الذاكرة
        var leaveTypes = await _context.LeaveTypes
            .AsNoTracking()
            .Where(lt => lt.IsDeleted == 0)
            .OrderBy(lt => lt.LeaveNameAr)
            .Select(lt => new LeaveTypeDto
            {
                LeaveTypeId = lt.LeaveTypeId,
                LeaveTypeNameAr = lt.LeaveNameAr,
                DefaultDays = lt.DefaultDays,
                IsDeductible = lt.IsDeductible,
                RequiresAttachment = lt.RequiresAttachment
            })
            .ToListAsync(cancellationToken);

        // ═══════════════════════════════════════════════════════════════════════════
        // الخطوة 2: إرجاع النتيجة
        // Step 2: Return result
        // ═══════════════════════════════════════════════════════════════════════════

        var message = leaveTypes.Count > 0
            ? $"تم استرجاع {leaveTypes.Count} نوع إجازة"
            : "لا توجد أنواع إجازات مسجلة";

        return Result<List<LeaveTypeDto>>.Success(leaveTypes, message);
    }
}
