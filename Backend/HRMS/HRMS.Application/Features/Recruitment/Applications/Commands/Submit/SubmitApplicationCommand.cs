using FluentValidation;
using HRMS.Application.Interfaces;
using HRMS.Core.Entities.Recruitment;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Recruitment.Applications.Commands.Submit;

/// <summary>
/// أمر تقديم طلب توظيف (ربط مرشح بوظيفة شاغرة)
/// </summary>
public class SubmitApplicationCommand : IRequest<Result<int>>
{
    /// <summary>
    /// معرف الوظيفة الشاغرة
    /// </summary>
    public int VacancyId { get; set; }

    /// <summary>
    /// معرف المرشح
    /// </summary>
    public int CandidateId { get; set; }

    /// <summary>
    /// مصدر الطلب (LinkedIn, Website, Referral, إلخ)
    /// </summary>
    public string? Source { get; set; }
}

public class SubmitApplicationCommandHandler : IRequestHandler<SubmitApplicationCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public SubmitApplicationCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result<int>> Handle(SubmitApplicationCommand request, CancellationToken cancellationToken)
    {
        // ✅ التحقق من أن الوظيفة الشاغرة موجودة ونشطة
        var vacancy = await _context.JobVacancies
            .FirstOrDefaultAsync(v => v.VacancyId == request.VacancyId, cancellationToken);

        if (vacancy == null)
            return Result<int>.Failure("الوظيفة الشاغرة غير موجودة");

        if (vacancy.Status != "ACTIVE")
            return Result<int>.Failure("الوظيفة الشاغرة غير متاحة للتقديم");

        // التحقق من تاريخ الإغلاق
        if (vacancy.ClosingDate.HasValue && vacancy.ClosingDate.Value < DateTime.Today)
            return Result<int>.Failure("انتهى موعد التقديم على هذه الوظيفة");

        // ✅ التحقق من وجود المرشح
        var candidateExists = await _context.Candidates
            .AnyAsync(c => c.CandidateId == request.CandidateId, cancellationToken);

        if (!candidateExists)
            return Result<int>.Failure("المرشح غير موجود");

        // ✅ التحقق من عدم وجود طلب مكرر (نفس المرشح + نفس الوظيفة)
        var duplicateExists = await _context.JobApplications
            .AnyAsync(a => a.CandidateId == request.CandidateId && a.VacancyId == request.VacancyId, 
                cancellationToken);

        if (duplicateExists)
            return Result<int>.Failure("تم تقديم طلب سابق لهذه الوظيفة");

        // إنشاء الطلب
        var application = new JobApplication
        {
            VacancyId = request.VacancyId,
            CandidateId = request.CandidateId,
            ApplicationDate = DateTime.UtcNow,
            Status = "APPLIED",
            Source = request.Source,
            CreatedBy = _currentUserService.UserId,
            CreatedAt = DateTime.UtcNow
        };

        _context.JobApplications.Add(application);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<int>.Success(application.AppId, "تم تقديم الطلب بنجاح");
    }
}

public class SubmitApplicationCommandValidator : AbstractValidator<SubmitApplicationCommand>
{
    public SubmitApplicationCommandValidator()
    {
        RuleFor(x => x.VacancyId)
            .GreaterThan(0).WithMessage("معرف الوظيفة الشاغرة مطلوب");

        RuleFor(x => x.CandidateId)
            .GreaterThan(0).WithMessage("معرف المرشح مطلوب");

        RuleFor(x => x.Source)
            .MaximumLength(50).WithMessage("مصدر الطلب لا يمكن أن يتجاوز 50 حرف")
            .When(x => !string.IsNullOrEmpty(x.Source));
    }
}
