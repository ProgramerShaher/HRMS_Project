using FluentValidation;
using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Recruitment.Candidates.Commands.Update;

public class UpdateCandidateCommand : IRequest<Result<int>>
{
    public int CandidateId { get; set; }
    public string FullNameEn { get; set; } = string.Empty;
    public string? FirstNameAr { get; set; }
    public string? FamilyNameAr { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public int? NationalityId { get; set; }
    public string? CvFilePath { get; set; }
    public string? LinkedinProfile { get; set; }
}

public class UpdateCandidateCommandValidator : AbstractValidator<UpdateCandidateCommand>
{
    public UpdateCandidateCommandValidator()
    {
        RuleFor(x => x.CandidateId).GreaterThan(0);
        RuleFor(x => x.FullNameEn).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
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

        candidate.FullNameEn = request.FullNameEn;
        candidate.FirstNameAr = request.FirstNameAr;
        candidate.FamilyNameAr = request.FamilyNameAr;
        candidate.Email = request.Email;
        candidate.Phone = request.Phone;
        candidate.NationalityId = request.NationalityId;
        candidate.CvFilePath = request.CvFilePath ?? candidate.CvFilePath;
        candidate.LinkedinProfile = request.LinkedinProfile;
        
        candidate.UpdatedBy = _currentUserService.UserId;
        candidate.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return Result<int>.Success(candidate.CandidateId, "تم تحديث بيانات المرشح بنجاح");
    }
}
