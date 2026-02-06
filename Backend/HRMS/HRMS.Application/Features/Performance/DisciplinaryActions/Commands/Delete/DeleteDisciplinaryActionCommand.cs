using FluentValidation;
using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Performance.DisciplinaryActions.Commands.Delete;

/// <summary>
/// أمر حذف إجراء تأديبي
/// </summary>
public class DeleteDisciplinaryActionCommand : IRequest<Result<bool>>
{
    public int ActionId { get; set; }
}

public class DeleteDisciplinaryActionCommandHandler : IRequestHandler<DeleteDisciplinaryActionCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;

    public DeleteDisciplinaryActionCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> Handle(DeleteDisciplinaryActionCommand request, CancellationToken cancellationToken)
    {
        var action = await _context.DisciplinaryActions
            .FirstOrDefaultAsync(a => a.ActionId == request.ActionId, cancellationToken);

        if (action == null)
            return Result<bool>.Failure("الإجراء التأديبي غير موجود");

        // ✅ التحقق من عدم وجود مخالفات مرتبطة
        var hasViolations = await _context.EmployeeViolations
            .AnyAsync(v => v.ActionId == request.ActionId, cancellationToken);

        if (hasViolations)
            return Result<bool>.Failure("لا يمكن حذف الإجراء التأديبي لأنه مستخدم في مخالفات موجودة");

        _context.DisciplinaryActions.Remove(action);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true, "تم حذف الإجراء التأديبي بنجاح");
    }
}

public class DeleteDisciplinaryActionCommandValidator : AbstractValidator<DeleteDisciplinaryActionCommand>
{
    public DeleteDisciplinaryActionCommandValidator()
    {
        RuleFor(x => x.ActionId)
            .GreaterThan(0).WithMessage("معرف الإجراء مطلوب");
    }
}
