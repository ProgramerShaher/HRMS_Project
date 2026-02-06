using FluentValidation;
using HRMS.Application.Interfaces;
using HRMS.Core.Entities.Recruitment;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Recruitment.Vacancies.Commands.Create;

/// <summary>
/// أمر إنشاء وظيفة شاغرة جديدة
/// </summary>
public class CreateVacancyCommand : IRequest<Result<int>>
{
    /// <summary>
    /// معرف المسمى الوظيفي
    /// </summary>
    public int JobId { get; set; }

    /// <summary>
    /// معرف القسم المطلوب
    /// </summary>
    public int DepartmentId { get; set; }

    /// <summary>
    /// عدد المناصب المطلوبة
    /// </summary>
    public int NumberOfPositions { get; set; }

    /// <summary>
    /// وصف الوظيفة
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// المتطلبات والمؤهلات
    /// </summary>
    public string? Requirements { get; set; }

    /// <summary>
    /// تاريخ إغلاق التقديم
    /// </summary>
    public DateTime? ClosingDate { get; set; }
}

/// <summary>
/// معالج إنشاء وظيفة شاغرة
/// </summary>
public class CreateVacancyCommandHandler : IRequestHandler<CreateVacancyCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public CreateVacancyCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result<int>> Handle(CreateVacancyCommand request, CancellationToken cancellationToken)
    {
        // ✅ التحقق من وجود الوظيفة
        var jobExists = await _context.Jobs
            .AnyAsync(j => j.JobId == request.JobId, cancellationToken);

        if (!jobExists)
            return Result<int>.Failure("المسمى الوظيفي غير موجود");

        // ✅ التحقق من وجود القسم
        var deptExists = await _context.Departments
            .AnyAsync(d => d.DeptId == request.DepartmentId, cancellationToken);

        if (!deptExists)
            return Result<int>.Failure("القسم غير موجود");

        // إنشاء الوظيفة الشاغرة
        var vacancy = new JobVacancy
        {
            JobId = request.JobId,
            DeptId = request.DepartmentId,
            // Description = request.Description,
            Requirements = request.Requirements,
            // PostedDate = DateTime.UtcNow,
            ClosingDate = request.ClosingDate,
            Status = "ACTIVE",
            CreatedBy = _currentUserService.UserId,
            CreatedAt = DateTime.UtcNow
        };

        _context.JobVacancies.Add(vacancy);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<int>.Success(vacancy.VacancyId, "تم إنشاء الوظيفة الشاغرة بنجاح");
    }
}

/// <summary>
/// مدقق صحة بيانات إنشاء الوظيفة الشاغرة
/// </summary>
public class CreateVacancyCommandValidator : AbstractValidator<CreateVacancyCommand>
{
    public CreateVacancyCommandValidator()
    {
        RuleFor(x => x.JobId)
            .GreaterThan(0).WithMessage("معرف الوظيفة مطلوب");

        RuleFor(x => x.DepartmentId)
            .GreaterThan(0).WithMessage("معرف القسم مطلوب");

        RuleFor(x => x.NumberOfPositions)
            .GreaterThan(0).WithMessage("عدد المناصب يجب أن يكون أكبر من صفر")
            .LessThanOrEqualTo(100).WithMessage("عدد المناصب لا يمكن أن يتجاوز 100");

        RuleFor(x => x.ClosingDate)
            .GreaterThan(DateTime.Today).WithMessage("تاريخ الإغلاق يجب أن يكون في المستقبل")
            .When(x => x.ClosingDate.HasValue);

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("الوصف لا يمكن أن يتجاوز 1000 حرف")
            .When(x => !string.IsNullOrEmpty(x.Description));
    }
}
