using FluentValidation;
using HRMS.Application.Interfaces;
using HRMS.Core.Entities.Performance;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Performance.DisciplinaryActions.Commands.Create;

/// <summary>
/// أمر إنشاء إجراء تأديبي جديد
/// </summary>
public class CreateDisciplinaryActionCommand : IRequest<Result<int>>
{
    /// <summary>
    /// اسم الإجراء بالعربية
    /// </summary>
    public string ActionNameAr { get; set; } = string.Empty;

    /// <summary>
    /// عدد أيام الخصم (0 إذا لم يكن هناك خصم)
    /// </summary>
    public decimal DeductionDays { get; set; } = 0;

    /// <summary>
    /// هل هذا الإجراء يؤدي لإنهاء الخدمة؟
    /// </summary>
    public byte IsTermination { get; set; } = 0;
}

/// <summary>
/// معالج أمر الإنشاء
/// </summary>
public class CreateDisciplinaryActionCommandHandler : IRequestHandler<CreateDisciplinaryActionCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public CreateDisciplinaryActionCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result<int>> Handle(CreateDisciplinaryActionCommand request, CancellationToken cancellationToken)
    {
        // التحقق من عدم تكرار الاسم
        var exists = await _context.DisciplinaryActions
            .AnyAsync(a => a.ActionNameAr == request.ActionNameAr, cancellationToken);

        if (exists)
            return Result<int>.Failure("يوجد إجراء تأديبي بنفس الاسم مسبقاً");

        // إنشاء الإجراء
        var action = new DisciplinaryAction
        {
            ActionNameAr = request.ActionNameAr,
            DeductionDays = request.DeductionDays,
            IsTermination = request.IsTermination,
            CreatedBy = _currentUserService.UserId,
            CreatedAt = DateTime.UtcNow
        };

        _context.DisciplinaryActions.Add(action);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<int>.Success(action.ActionId, "تم إنشاء الإجراء التأديبي بنجاح");
    }
}

/// <summary>
/// قواعد التحقق
/// </summary>
public class CreateDisciplinaryActionCommandValidator : AbstractValidator<CreateDisciplinaryActionCommand>
{
    public CreateDisciplinaryActionCommandValidator()
    {
        RuleFor(x => x.ActionNameAr)
            .NotEmpty().WithMessage("اسم الإجراء بالعربية مطلوب")
            .MaximumLength(200).WithMessage("اسم الإجراء لا يمكن أن يتجاوز 200 حرف");

        RuleFor(x => x.DeductionDays)
            .GreaterThanOrEqualTo(0).WithMessage("عدد أيام الخصم يجب أن يكون 0 أو أكثر");

        RuleFor(x => x.IsTermination)
            .Must(v => v == 0 || v == 1).WithMessage("قيمة IsTermination يجب أن تكون 0 أو 1");
    }
}
