using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Recruitment.Interviews.Commands.Cancel;

/// <summary>
/// Cancel Interview Command
/// </summary>
public class CancelInterviewCommand : IRequest<Result<bool>>
{
    public int InterviewId { get; set; }
    public string? CancellationReason { get; set; }
}

/// <summary>
/// Cancel Interview Command Handler
/// </summary>
public class CancelInterviewCommandHandler : IRequestHandler<CancelInterviewCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public CancelInterviewCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result<bool>> Handle(CancelInterviewCommand request, CancellationToken cancellationToken)
    {
        // البحث عن المقابلة
        var interview = await _context.Interviews
            .FirstOrDefaultAsync(i => i.InterviewId == request.InterviewId && i.IsDeleted == 0, cancellationToken);

        if (interview == null)
            return Result<bool>.Failure("المقابلة غير موجودة");

        // Business Rule: Cannot cancel completed interviews
        if (interview.Status == "COMPLETED")
            return Result<bool>.Failure("لا يمكن إلغاء مقابلة مكتملة");

        if (interview.Status == "CANCELLED")
            return Result<bool>.Failure("المقابلة ملغاة بالفعل");

        // تحديث حالة المقابلة
        interview.Status = "CANCELLED";
        if (!string.IsNullOrEmpty(request.CancellationReason))
        {
            interview.ResultNotes = request.CancellationReason;
        }
        interview.UpdatedBy = _currentUserService.UserId;
        interview.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true, "تم إلغاء المقابلة بنجاح");
    }
}
