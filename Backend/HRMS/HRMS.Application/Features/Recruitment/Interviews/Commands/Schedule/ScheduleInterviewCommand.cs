using FluentValidation;
using HRMS.Application.Interfaces;
using HRMS.Core.Entities.Recruitment;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Recruitment.Interviews.Commands.Schedule;

/// <summary>
/// أمر جدولة مقابلة توظيف
/// </summary>
public class ScheduleInterviewCommand : IRequest<Result<int>>
{
    /// <summary>
    /// معرف طلب التوظيف
    /// </summary>
    public int AppId { get; set; }

    /// <summary>
    /// معرف الموظف المُقابِل
    /// </summary>
    public int? InterviewerId { get; set; }

    /// <summary>
    /// وقت المقابلة المجدولة
    /// </summary>
    public DateTime ScheduledTime { get; set; }

    /// <summary>
    /// نوع المقابلة (IN_PERSON, ONLINE, PHONE)
    /// </summary>
    public string InterviewType { get; set; } = "IN_PERSON";
}

public class ScheduleInterviewCommandHandler : IRequestHandler<ScheduleInterviewCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public ScheduleInterviewCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result<int>> Handle(ScheduleInterviewCommand request, CancellationToken cancellationToken)
    {
        // التحقق من وجود الطلب
        var application = await _context.JobApplications
            .FirstOrDefaultAsync(a => a.AppId == request.AppId, cancellationToken);

        if (application == null)
            return Result<int>.Failure("طلب التوظيف غير موجود");

        // ✅ يجب أن يكون الطلب في حالة SHORTLISTED
        if (application.Status != "SHORTLISTED")
            return Result<int>.Failure("الطلب يجب أن يكون في قائمة المرشحين (SHORTLISTED) لجدولة مقابلة");

        // التحقق من المُقابِل إذا تم تحديده
        if (request.InterviewerId.HasValue)
        {
            var interviewerExists = await _context.Employees
                .AnyAsync(e => e.EmployeeId == request.InterviewerId.Value, cancellationToken);

            if (!interviewerExists)
                return Result<int>.Failure("الموظف المُقابِل غير موجود");
        }

        // إنشاء المقابلة
        var interview = new Interview
        {
            AppId = request.AppId,
            InterviewerId = request.InterviewerId,
            ScheduledTime = request.ScheduledTime,
            InterviewType = request.InterviewType,
            CreatedBy = _currentUserService.UserId,
            CreatedAt = DateTime.UtcNow
        };

        _context.Interviews.Add(interview);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<int>.Success(interview.InterviewId, "تم جدولة المقابلة بنجاح");
    }
}

public class ScheduleInterviewCommandValidator : AbstractValidator<ScheduleInterviewCommand>
{
    public ScheduleInterviewCommandValidator()
    {
        RuleFor(x => x.AppId)
            .GreaterThan(0).WithMessage("معرف الطلب مطلوب");

        RuleFor(x => x.ScheduledTime)
            .GreaterThan(DateTime.Now).WithMessage("وقت المقابلة يجب أن يكون في المستقبل");

        RuleFor(x => x.InterviewType)
            .NotEmpty().WithMessage("نوع المقابلة مطلوب")
            .Must(t => new[] { "IN_PERSON", "ONLINE", "PHONE" }.Contains(t))
            .WithMessage("نوع المقابلة يجب أن يكون: IN_PERSON, ONLINE, PHONE");
    }
}
