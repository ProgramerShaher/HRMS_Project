using FluentValidation;
using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Performance.Violations.Commands.UpdateViolation;

public class UpdateViolationCommand : IRequest<Result<int>>
{
    public int ViolationId { get; set; }
    public int ViolationTypeId { get; set; }
    public int? ActionId { get; set; }
    public string? Description { get; set; }
    public DateTime ViolationDate { get; set; }
}

public class UpdateViolationCommandValidator : AbstractValidator<UpdateViolationCommand>
{
    public UpdateViolationCommandValidator()
    {
        RuleFor(x => x.ViolationId).GreaterThan(0);
        RuleFor(x => x.ViolationTypeId).GreaterThan(0);
        RuleFor(x => x.Description).MaximumLength(500);
        RuleFor(x => x.ViolationDate).LessThanOrEqualTo(DateTime.Now);
    }
}

public class UpdateViolationCommandHandler : IRequestHandler<UpdateViolationCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public UpdateViolationCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result<int>> Handle(UpdateViolationCommand request, CancellationToken cancellationToken)
    {
        var violation = await _context.EmployeeViolations
            .FirstOrDefaultAsync(v => v.ViolationId == request.ViolationId && v.IsDeleted == 0, cancellationToken);

        if (violation == null)
            return Result<int>.Failure("المخالفة غير موجودة");

        if (violation.Status == "APPROVED")
            return Result<int>.Failure("لا يمكن تعديل مخالفة معتمدة");

        violation.ViolationTypeId = request.ViolationTypeId;
        violation.ActionId = request.ActionId;
        violation.Description = request.Description;
        violation.ViolationDate = request.ViolationDate;
        violation.UpdatedBy = _currentUserService.UserId;
        violation.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return Result<int>.Success(violation.ViolationId, "تم تحديث بيانات المخالفة بنجاح");
    }
}
