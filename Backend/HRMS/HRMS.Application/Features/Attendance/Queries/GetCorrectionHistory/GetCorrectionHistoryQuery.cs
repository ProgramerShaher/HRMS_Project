using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using HRMS.Core.Entities.Identity;
using Microsoft.AspNetCore.Identity;

namespace HRMS.Application.Features.Attendance.Queries.GetCorrectionHistory;

public class GetCorrectionHistoryQuery : IRequest<Result<List<CorrectionHistoryDto>>>
{
    public int EmployeeId { get; set; }
}

public class CorrectionHistoryDto
{
    public DateTime ChangeDate { get; set; }
    public string FieldName { get; set; } = string.Empty;
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string ManagerName { get; set; } = string.Empty;
}

public class GetCorrectionHistoryQueryHandler(IApplicationDbContext context, UserManager<ApplicationUser> userManager) : IRequestHandler<GetCorrectionHistoryQuery, Result<List<CorrectionHistoryDto>>>
{
    public async Task<Result<List<CorrectionHistoryDto>>> Handle(GetCorrectionHistoryQuery request, CancellationToken cancellationToken)
    {
        var corrections = await context.AttendanceCorrections
            .Where(c => c.EmployeeId == request.EmployeeId)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync(cancellationToken);

        var dtos = new List<CorrectionHistoryDto>();

        foreach (var c in corrections)
        {
            var managerName = "System";
            if (!string.IsNullOrEmpty(c.CreatedBy))
            {
                var user = await userManager.FindByIdAsync(c.CreatedBy);
                managerName = user?.FullNameAr ?? user?.FullNameEn ?? c.CreatedBy;
            }

            dtos.Add(new CorrectionHistoryDto
            {
                ChangeDate = c.CreatedAt,
                FieldName = c.FieldName,
                OldValue = c.OldValue,
                NewValue = c.NewValue,
                Reason = c.AuditNote,
                ManagerName = managerName
            });
        }

        return Result<List<CorrectionHistoryDto>>.Success(dtos);
    }
}
