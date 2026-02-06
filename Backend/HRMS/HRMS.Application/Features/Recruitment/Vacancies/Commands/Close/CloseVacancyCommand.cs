using FluentValidation;
using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Recruitment.Vacancies.Commands.Close;

/// <summary>
/// أمر إغلاق وظيفة شاغرة (إيقاف استقبال الطلبات)
/// </summary>
public class CloseVacancyCommand : IRequest<Result<bool>>
{
    public int VacancyId { get; set; }
}

public class CloseVacancyCommandHandler : IRequestHandler<CloseVacancyCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public CloseVacancyCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result<bool>> Handle(CloseVacancyCommand request, CancellationToken cancellationToken)
    {
        var vacancy = await _context.JobVacancies
            .FirstOrDefaultAsync(v => v.VacancyId == request.VacancyId, cancellationToken);

        if (vacancy == null)
            return Result<bool>.Failure("الوظيفة الشاغرة غير موجودة");

        // ✅ التحقق من أن الوظيفة ليست مغلقة بالفعل
        if (vacancy.Status == "CLOSED")
            return Result<bool>.Failure("الوظيفة الشاغرة مغلقة بالفعل");

        // إغلاق الوظيفة
        vacancy.Status = "CLOSED";
        vacancy.UpdatedBy = _currentUserService.UserId;
        vacancy.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true, "تم إغلاق الوظيفة الشاغرة بنجاح");
    }
}

public class CloseVacancyCommandValidator : AbstractValidator<CloseVacancyCommand>
{
    public CloseVacancyCommandValidator()
    {
        RuleFor(x => x.VacancyId)
            .GreaterThan(0).WithMessage("معرف الوظيفة الشاغرة مطلوب");
    }
}
