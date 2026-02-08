using FluentValidation;
using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Recruitment.Candidates.Commands.Update;

public class UpdateCandidateCommand : IRequest<Result<int>>
{
    public int CandidateId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? ResumeUrl { get; set; }
}

public class UpdateCandidateCommandValidator : AbstractValidator<UpdateCandidateCommand>
{
    public UpdateCandidateCommandValidator()
    {
        RuleFor(x => x.CandidateId).GreaterThan(0);
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(50);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Phone).NotEmpty().MaximumLength(20);
    }
}

public class UpdateCandidateCommandHandler : IRequestHandler<UpdateCandidateCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public UpdateCandidateCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result<int>> Handle(UpdateCandidateCommand request, CancellationToken cancellationToken)
    {
        var candidate = await _context.Candidates
            .FirstOrDefaultAsync(c => c.CandidateId == request.CandidateId && c.IsDeleted == 0, cancellationToken);

        if (candidate == null)
            return Result<int>.Failure("المرشح غير موجود");

        candidate.FirstNameAr = request.FirstName;
        candidate.Email = request.Email;
        candidate.Phone = request.Phone;
        if (!string.IsNullOrEmpty(request.ResumeUrl)) candidate.CvFilePath = request.ResumeUrl;
        
        candidate.UpdatedBy = _currentUserService.UserId;
        candidate.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return Result<int>.Success(candidate.CandidateId, "تم تحديث بيانات المرشح بنجاح");
    }
}
