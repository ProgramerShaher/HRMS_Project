using FluentValidation;
using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Recruitment.Interviews.Commands.RecordResult;

public class RecordInterviewResultCommand : IRequest<Result<bool>>
{
    public int InterviewId { get; set; }
    public string Result { get; set; } = string.Empty; // PASSED, FAILED, NO_SHOW
    public string? Notes { get; set; }
    public int? Rating { get; set; } // 1-5
}

public class RecordInterviewResultCommandValidator : AbstractValidator<RecordInterviewResultCommand>
{
    public RecordInterviewResultCommandValidator()
    {
        RuleFor(x => x.InterviewId).GreaterThan(0);
        RuleFor(x => x.Result).NotEmpty().Must(s => new[] { "PASSED", "FAILED", "NO_SHOW" }.Contains(s));
        RuleFor(x => x.Rating).InclusiveBetween(1, 5).When(x => x.Rating.HasValue);
    }
}

public class RecordInterviewResultCommandHandler : IRequestHandler<RecordInterviewResultCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public RecordInterviewResultCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result<bool>> Handle(RecordInterviewResultCommand request, CancellationToken cancellationToken)
    {
        var interview = await _context.Interviews
            .FirstOrDefaultAsync(i => i.InterviewId == request.InterviewId && i.IsDeleted == 0, cancellationToken);

        if (interview == null)
            return Result<bool>.Failure("المعذرة، المقابلة غير موجودة");

        interview.Status = "COMPLETED";
        interview.Result = request.Result;
        interview.ResultNotes = request.Notes;
        interview.Rating = (byte?)request.Rating;
        interview.UpdatedBy = _currentUserService.UserId;
        interview.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true, "تم تسجيل نتيجة المقابلة بنجاح");
    }
}
