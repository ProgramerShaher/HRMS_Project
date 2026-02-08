using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Recruitment.Candidates.Commands.Delete;

public class DeleteCandidateCommand : IRequest<Result<int>>
{
    public int CandidateId { get; set; }
}

public class DeleteCandidateCommandHandler : IRequestHandler<DeleteCandidateCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public DeleteCandidateCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result<int>> Handle(DeleteCandidateCommand request, CancellationToken cancellationToken)
    {
        var candidate = await _context.Candidates
            .FirstOrDefaultAsync(c => c.CandidateId == request.CandidateId && c.IsDeleted == 0, cancellationToken);

        if (candidate == null)
            return Result<int>.Failure("المرشح غير موجود");

        candidate.IsDeleted = 1;
        candidate.UpdatedBy = _currentUserService.UserId;
        candidate.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return Result<int>.Success(candidate.CandidateId, "تم حذف المرشح بنجاح");
    }
}
