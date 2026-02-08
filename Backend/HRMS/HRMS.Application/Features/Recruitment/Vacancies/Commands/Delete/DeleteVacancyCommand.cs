using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Recruitment.Vacancies.Commands.Delete;

/// <summary>
/// Delete Job Vacancy Command
/// </summary>
public class DeleteVacancyCommand : IRequest<Result<int>>
{
    public int VacancyId { get; set; }
}

/// <summary>
/// Delete Vacancy Command Handler
/// </summary>
public class DeleteVacancyCommandHandler : IRequestHandler<DeleteVacancyCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public DeleteVacancyCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result<int>> Handle(DeleteVacancyCommand request, CancellationToken cancellationToken)
    {
        // البحث عن الوظيفة الشاغرة
        var vacancy = await _context.JobVacancies
            .FirstOrDefaultAsync(v => v.VacancyId == request.VacancyId && v.IsDeleted == 0, cancellationToken);

        if (vacancy == null)
            return Result<int>.Failure("الوظيفة الشاغرة غير موجودة");

        // التحقق من عدم وجود طلبات توظيف نشطة
        // Business Rule: Cannot delete vacancy if active applications exist
        var hasActiveApplications = await _context.JobApplications
            .AnyAsync(a => a.VacancyId == request.VacancyId 
                          && a.IsDeleted == 0 
                          && a.Status != "REJECTED" 
                          && a.Status != "WITHDRAWN", 
                      cancellationToken);

        if (hasActiveApplications)
            return Result<int>.Failure("لا يمكن حذف الوظيفة لوجود طلبات توظيف نشطة. يرجى إغلاق الوظيفة بدلاً من حذفها");

        // تنفيذ الحذف الناعم
        vacancy.IsDeleted = 1;
        vacancy.UpdatedBy = _currentUserService.UserId;
        vacancy.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return Result<int>.Success(vacancy.VacancyId, "تم حذف الوظيفة الشاغرة بنجاح");
    }
}
