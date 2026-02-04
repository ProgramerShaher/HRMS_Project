using FluentValidation;
using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Attendance.Requests.Permissions.Commands.ApproveRejectPermissionRequest;

public class ApproveRejectPermissionRequestCommand : IRequest<Result<bool>>
{
    public int PermissionRequestId { get; set; }
    public string Action { get; set; } = string.Empty; // Approve, Reject
    public string? RejectionReason { get; set; }
    public int ApproverId { get; set; } // Set from Controller
}

public class ApproveRejectPermissionRequestCommandHandler : IRequestHandler<ApproveRejectPermissionRequestCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;

    public ApproveRejectPermissionRequestCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> Handle(ApproveRejectPermissionRequestCommand request, CancellationToken cancellationToken)
    {
        var permission = await _context.PermissionRequests
            .FirstOrDefaultAsync(x => x.PermissionRequestId == request.PermissionRequestId, cancellationToken);

        if (permission == null)
            return Result<bool>.Failure("Permission Request not found.");

        if (permission.Status != "Pending")
            return Result<bool>.Failure($"Request is already {permission.Status}");

        if (request.Action == "Approve")
        {
            permission.Status = "Approved";
            permission.ApprovedBy = request.ApproverId;
            permission.ApprovedAt = DateTime.UtcNow;
        }
        else if (request.Action == "Reject")
        {
            permission.Status = "Rejected";
            permission.RejectionReason = request.RejectionReason;
        }

        await _context.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true, $"Request {request.Action}ed successfully.");
    }
}

public class ApproveRejectPermissionRequestValidator : AbstractValidator<ApproveRejectPermissionRequestCommand>
{
    public ApproveRejectPermissionRequestValidator()
    {
        RuleFor(x => x.PermissionRequestId).GreaterThan(0);
        RuleFor(x => x.Action).Must(x => x == "Approve" || x == "Reject").WithMessage("Action must be Approve or Reject");
        RuleFor(x => x.RejectionReason).NotEmpty().When(x => x.Action == "Reject");
    }
}
