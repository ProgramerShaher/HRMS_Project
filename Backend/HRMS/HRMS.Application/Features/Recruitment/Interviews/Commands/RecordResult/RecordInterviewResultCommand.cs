using FluentValidation;
using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Recruitment.Interviews.Commands.RecordResult;

/// <summary>
/// أمر تسجيل نتيجة مقابلة التوظيف
/// </summary>
public class RecordInterviewResultCommand : IRequest<Result<bool>>
{
    /// <summary>
    /// معرف المقابلة
    /// </summary>
    public int InterviewId { get; set; }

    /// <summary>
    /// نتيجة المقابلة (PASS, FAIL)
    /// </summary>
    public string Result { get; set; } = string.Empty;

    /// <summary>
    /// التقييم (1-5)
    /// </summary>
    public byte? Rating { get; set; }

    /// <summary>
    /// الملاحظات
    /// </summary>
    public string? Feedback { get; set; }
}

public class RecordInterviewResultCommandHandler : IRequestHandler<RecordInterviewResultCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public RecordInterviewResultCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result<bool>> Handle(RecordInterviewResultCommand request, CancellationToken cancellationToken)
    {
        var interview = await _context.Interviews
            .Include(i => i.Application)
            .FirstOrDefaultAsync(i => i.InterviewId == request.InterviewId, cancellationToken);

        if (interview == null)
            return Result<bool>.Failure("المقابلة غير موجودة");

        // تحديث نتيجة المقابلة
        interview.Result = request.Result;
        interview.Rating = request.Rating;
        interview.Feedback = request.Feedback;
        interview.UpdatedBy = _currentUserService.UserId;
        interview.UpdatedAt = DateTime.UtcNow;

        // ✅ إذا كانت النتيجة FAIL، تحديث حالة الطلب إلى REJECTED تلقائياً
        if (request.Result == "FAIL")
        {
            interview.Application.Status = "REJECTED";
            interview.Application.RejectionReason = "رفض بعد المقابلة";
            interview.Application.UpdatedBy = _currentUserService.UserId;
            interview.Application.UpdatedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync(cancellationToken);

        var message = request.Result == "PASS" 
            ? "تم تسجيل نتيجة المقابلة بنجاح - المرشح اجتاز المقابلة"
            : "تم تسجيل نتيجة المقابلة بنجاح - تم رفض المرشح تلقائياً";

        return Result<bool>.Success(true, message);
    }
}

public class RecordInterviewResultCommandValidator : AbstractValidator<RecordInterviewResultCommand>
{
    public RecordInterviewResultCommandValidator()
    {
        RuleFor(x => x.InterviewId)
            .GreaterThan(0).WithMessage("معرف المقابلة مطلوب");

        RuleFor(x => x.Result)
            .NotEmpty().WithMessage("نتيجة المقابلة مطلوبة")
            .Must(r => new[] { "PASS", "FAIL" }.Contains(r))
            .WithMessage("نتيجة المقابلة يجب أن تكون: PASS أو FAIL");

        RuleFor(x => x.Rating)
            .InclusiveBetween((byte)1, (byte)5).WithMessage("التقييم يجب أن يكون بين 1 و 5")
            .When(x => x.Rating.HasValue);

        RuleFor(x => x.Feedback)
            .MaximumLength(1000).WithMessage("الملاحظات لا يمكن أن تتجاوز 1000 حرف")
            .When(x => !string.IsNullOrEmpty(x.Feedback));
    }
}
