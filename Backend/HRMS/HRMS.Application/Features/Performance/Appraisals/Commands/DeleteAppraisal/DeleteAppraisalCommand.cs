using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Performance.Appraisals.Commands.DeleteAppraisal;

public class DeleteAppraisalCommand : IRequest<Result<int>>
{
    public int AppraisalId { get; set; }

    public DeleteAppraisalCommand(int id)
    {
        AppraisalId = id;
    }
}

public class DeleteAppraisalCommandHandler : IRequestHandler<DeleteAppraisalCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public DeleteAppraisalCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result<int>> Handle(DeleteAppraisalCommand request, CancellationToken cancellationToken)
    {
        var appraisal = await _context.EmployeeAppraisals
            .FirstOrDefaultAsync(a => a.AppraisalId == request.AppraisalId && a.IsDeleted == 0, cancellationToken);

        if (appraisal == null)
            return Result<int>.Failure("التقييم غير موجود");

        appraisal.IsDeleted = 1;
        appraisal.UpdatedBy = _currentUserService.UserId;
        appraisal.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return Result<int>.Success(appraisal.AppraisalId, "تم حذف التقييم بنجاح");
    }
}
