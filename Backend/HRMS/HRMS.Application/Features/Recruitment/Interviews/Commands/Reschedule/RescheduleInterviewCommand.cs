using FluentValidation;
using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Recruitment.Interviews.Commands.Reschedule;

/// <summary>
/// Reschedule Interview Command
/// </summary>
public class RescheduleInterviewCommand : IRequest<Result<bool>>
{
    public int InterviewId { get; set; }
    public DateTime NewScheduledTime { get; set; }
    public int? NewInterviewerId { get; set; }
}

/// <summary>
/// Reschedule Interview Command Validator
/// </summary>
public class RescheduleInterviewCommandValidator : AbstractValidator<RescheduleInterviewCommand>
{
    public RescheduleInterviewCommandValidator()
    {
        RuleFor(x => x.InterviewId).GreaterThan(0);
        RuleFor(x => x.NewScheduledTime).GreaterThan(DateTime.Now)
            .WithMessage("يجب أن يكون موعد المقابلة في المستقبل");
    }
}

/// <summary>
/// Reschedule Interview Command Handler
/// </summary>
public class RescheduleInterviewCommandHandler : IRequestHandler<RescheduleInterviewCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public RescheduleInterviewCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result<bool>> Handle(RescheduleInterviewCommand request, CancellationToken cancellationToken)
    {
        // البحث عن المقابلة
        var interview = await _context.Interviews
            .FirstOrDefaultAsync(i => i.InterviewId == request.InterviewId && i.IsDeleted == 0, cancellationToken);

        if (interview == null)
            return Result<bool>.Failure("المقابلة غير موجودة");

        // Business Rule: Cannot reschedule completed interviews
        if (interview.Status == "COMPLETED")
            return Result<bool>.Failure("لا يمكن إعادة جدولة مقابلة مكتملة");

        if (interview.Status == "CANCELLED")
            return Result<bool>.Failure("لا يمكن إعادة جدولة مقابلة ملغاة");

        // التحقق من وجود المقابل الجديد إذا تم تحديده
        if (request.NewInterviewerId.HasValue)
        {
            var interviewerExists = await _context.Employees
                .AnyAsync(e => e.EmployeeId == request.NewInterviewerId.Value && e.IsDeleted == 0, cancellationToken);

            if (!interviewerExists)
                return Result<bool>.Failure("المقابل المحدد غير موجود");

            interview.InterviewerId = request.NewInterviewerId.Value;
        }

        // تحديث موعد المقابلة
        interview.ScheduledTime = request.NewScheduledTime;
        interview.UpdatedBy = _currentUserService.UserId;
        interview.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true, "تم إعادة جدولة المقابلة بنجاح");
    }
}
