using FluentValidation;
using HRMS.Application.Interfaces;
using HRMS.Core.Entities.Attendance;
using HRMS.Core.Utilities;
using MediatR;

namespace HRMS.Application.Features.Attendance.Requests.Permissions.Commands.CreatePermissionRequest;

public class CreatePermissionRequestCommand : IRequest<Result<int>>
{
    public int EmployeeId { get; set; } // Will be set from CurrentUser in Controller
    public DateTime PermissionDate { get; set; }
    public string PermissionType { get; set; } = string.Empty; // LateEntry, EarlyExit
    public decimal Hours { get; set; }
    public string Reason { get; set; } = string.Empty;
}

public class CreatePermissionRequestCommandHandler : IRequestHandler<CreatePermissionRequestCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;

    public CreatePermissionRequestCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<int>> Handle(CreatePermissionRequestCommand request, CancellationToken cancellationToken)
    {
        // Check duplication
        // (Optional: Access Policy to check limit)

        var permission = new PermissionRequest
        {
            EmployeeId = request.EmployeeId,
            PermissionDate = request.PermissionDate,
            PermissionType = request.PermissionType,
            Hours = request.Hours,
            Reason = request.Reason,
            Status = "Pending"
        };

        _context.PermissionRequests.Add(permission);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<int>.Success(permission.PermissionRequestId, "Permission request submitted successfully.");
    }
}

public class CreatePermissionRequestValidator : AbstractValidator<CreatePermissionRequestCommand>
{
    public CreatePermissionRequestValidator()
    {
        RuleFor(x => x.EmployeeId).GreaterThan(0);
        RuleFor(x => x.PermissionDate).NotEmpty();
        RuleFor(x => x.PermissionType).Must(x => x == "LateEntry" || x == "EarlyExit")
            .WithMessage("Type must be 'LateEntry' or 'EarlyExit'");
        RuleFor(x => x.Hours).GreaterThan(0);
        RuleFor(x => x.Reason).NotEmpty();
    }
}
