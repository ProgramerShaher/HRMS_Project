using FluentValidation;
using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Recruitment.Vacancies.Commands.Update;

/// <summary>
/// أمر تحديث بيانات وظيفة شاغرة
/// </summary>
public class UpdateVacancyCommand : IRequest<Result<bool>>
{
    public int VacancyId { get; set; }
    public int JobId { get; set; }
    public int DepartmentId { get; set; }
    public int NumberOfPositions { get; set; }
    public string? Description { get; set; }
    public string? Requirements { get; set; }
    public DateTime? ClosingDate { get; set; }
}

public class UpdateVacancyCommandHandler : IRequestHandler<UpdateVacancyCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public UpdateVacancyCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result<bool>> Handle(UpdateVacancyCommand request, CancellationToken cancellationToken)
    {
        var vacancy = await _context.JobVacancies
            .FirstOrDefaultAsync(v => v.VacancyId == request.VacancyId, cancellationToken);

        if (vacancy == null)
            return Result<bool>.Failure("الوظيفة الشاغرة غير موجودة");

        // ✅ لا يمكن تعديل وظيفة مغلقة
        if (vacancy.Status == "CLOSED")
            return Result<bool>.Failure("لا يمكن تعديل وظيفة شاغرة مغلقة");

        // التحقق من وجود الوظيفة الجديدة
        var jobExists = await _context.Jobs
            .AnyAsync(j => j.JobId == request.JobId, cancellationToken);

        if (!jobExists)
            return Result<bool>.Failure("المسمى الوظيفي غير موجود");

        // التحديث
        vacancy.JobId = request.JobId;
        vacancy.DeptId = request.DepartmentId;
        // vacancy.NumberOfPositions = request.NumberOfPositions;
        // vacancy.Description = request.Description;
        vacancy.Requirements = request.Requirements;
        vacancy.ClosingDate = request.ClosingDate;
        vacancy.UpdatedBy = _currentUserService.UserId;
        vacancy.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true, "تم تحديث الوظيفة الشاغرة بنجاح");
    }
}

public class UpdateVacancyCommandValidator : AbstractValidator<UpdateVacancyCommand>
{
    public UpdateVacancyCommandValidator()
    {
        RuleFor(x => x.VacancyId)
            .GreaterThan(0).WithMessage("معرف الوظيفة الشاغرة مطلوب");

        RuleFor(x => x.JobId)
            .GreaterThan(0).WithMessage("معرف الوظيفة مطلوب");

        RuleFor(x => x.DepartmentId)
            .GreaterThan(0).WithMessage("معرف القسم مطلوب");

        RuleFor(x => x.NumberOfPositions)
            .GreaterThan(0).WithMessage("عدد المناصب يجب أن يكون أكبر من صفر");

        RuleFor(x => x.ClosingDate)
            .GreaterThan(DateTime.Today).WithMessage("تاريخ الإغلاق يجب أن يكون في المستقبل")
            .When(x => x.ClosingDate.HasValue);
    }
}
