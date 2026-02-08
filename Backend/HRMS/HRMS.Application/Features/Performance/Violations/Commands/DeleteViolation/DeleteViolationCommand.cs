using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Performance.Violations.Commands.DeleteViolation;

public class DeleteViolationCommand : IRequest<Result<int>>
{
    public int ViolationId { get; set; }

    public DeleteViolationCommand(int id)
    {
        ViolationId = id;
    }
}

public class DeleteViolationCommandHandler : IRequestHandler<DeleteViolationCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public DeleteViolationCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result<int>> Handle(DeleteViolationCommand request, CancellationToken cancellationToken)
    {
        var violation = await _context.EmployeeViolations
            .FirstOrDefaultAsync(v => v.ViolationId == request.ViolationId && v.IsDeleted == 0, cancellationToken);

        if (violation == null)
            return Result<int>.Failure("المخالفة غير موجودة");

        if (violation.Status == "APPROVED")
            return Result<int>.Failure("لا يمكن حذف مخالفة معتمدة");

        violation.IsDeleted = 1;
        violation.UpdatedBy = _currentUserService.UserId;
        violation.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return Result<int>.Success(violation.ViolationId, "تم حذف المخالفة بنجاح");
    }
}
