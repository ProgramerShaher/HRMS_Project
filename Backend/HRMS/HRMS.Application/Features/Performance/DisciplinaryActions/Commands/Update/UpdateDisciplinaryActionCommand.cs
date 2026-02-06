using FluentValidation;
using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Performance.DisciplinaryActions.Commands.Update;

/// <summary>
/// أمر تحديث إجراء تأديبي
/// </summary>
public class UpdateDisciplinaryActionCommand : IRequest<Result<bool>>
{
    public int ActionId { get; set; }
    public string ActionNameAr { get; set; } = string.Empty;
    public decimal DeductionDays { get; set; }
    public byte IsTermination { get; set; } = 0;
}

public class UpdateDisciplinaryActionCommandHandler : IRequestHandler<UpdateDisciplinaryActionCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public UpdateDisciplinaryActionCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result<bool>> Handle(UpdateDisciplinaryActionCommand request, CancellationToken cancellationToken)
    {
        var action = await _context.DisciplinaryActions
            .FirstOrDefaultAsync(a => a.ActionId == request.ActionId, cancellationToken);

        if (action == null)
            return Result<bool>.Failure("الإجراء التأديبي غير موجود");

        // التحقق من عدم تكرار الاسم
        var duplicateExists = await _context.DisciplinaryActions
            .AnyAsync(a => a.ActionNameAr == request.ActionNameAr 
                        && a.ActionId != request.ActionId, cancellationToken);

        if (duplicateExists)
            return Result<bool>.Failure("يوجد إجراء تأديبي آخر بنفس الاسم");

        // تحديث البيانات
        action.ActionNameAr = request.ActionNameAr;
        action.DeductionDays = request.DeductionDays;
        action.IsTermination = request.IsTermination;
        action.UpdatedBy = _currentUserService.UserId;
        action.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true, "تم تحديث الإجراء التأديبي بنجاح");
    }
}

public class UpdateDisciplinaryActionCommandValidator : AbstractValidator<UpdateDisciplinaryActionCommand>
{
    public UpdateDisciplinaryActionCommandValidator()
    {
        RuleFor(x => x.ActionId)
            .GreaterThan(0).WithMessage("معرف الإجراء مطلوب");

        RuleFor(x => x.ActionNameAr)
            .NotEmpty().WithMessage("اسم الإجراء بالعربية مطلوب")
            .MaximumLength(200).WithMessage("اسم الإجراء لا يمكن أن يتجاوز 200 حرف");

        RuleFor(x => x.DeductionDays)
            .GreaterThanOrEqualTo(0).WithMessage("عدد أيام الخصم يجب أن يكون 0 أو أكثر");
    }
}
