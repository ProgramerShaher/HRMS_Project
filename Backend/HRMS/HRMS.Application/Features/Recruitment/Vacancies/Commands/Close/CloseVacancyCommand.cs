using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Recruitment.Vacancies.Commands.Close;

public class CloseVacancyCommand : IRequest<Result<bool>>
{
    public int VacancyId { get; set; }
}

public class CloseVacancyCommandHandler : IRequestHandler<CloseVacancyCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public CloseVacancyCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result<bool>> Handle(CloseVacancyCommand request, CancellationToken cancellationToken)
    {
        var vacancy = await _context.JobVacancies
            .FirstOrDefaultAsync(v => v.VacancyId == request.VacancyId && v.IsDeleted == 0, cancellationToken);

        if (vacancy == null)
            return Result<bool>.Failure("الوظيفة الشاغرة غير موجودة");

        vacancy.Status = "CLOSED";
        vacancy.UpdatedBy = _currentUserService.UserId;
        vacancy.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true, "تم إغلاق الوظيفة الشاغرة بنجاح");
    }
}
