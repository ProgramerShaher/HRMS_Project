using FluentValidation;
using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Performance.ViolationTypes.Commands.Update;

/// <summary>
/// أمر تحديث نوع مخالفة
/// </summary>
public class UpdateViolationTypeCommand : IRequest<Result<bool>>
{
    /// <summary>
    /// معرف نوع المخالفة
    /// </summary>
    public int ViolationTypeId { get; set; }

    /// <summary>
    /// اسم المخالفة بالعربية
    /// </summary>
    public string ViolationNameAr { get; set; } = string.Empty;

    /// <summary>
    /// الوصف
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// مستوى الخطورة
    /// </summary>
    public byte SeverityLevel { get; set; } = 1;
}

/// <summary>
/// معالج أمر تحديث نوع المخالفة
/// </summary>
public class UpdateViolationTypeCommandHandler : IRequestHandler<UpdateViolationTypeCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public UpdateViolationTypeCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result<bool>> Handle(UpdateViolationTypeCommand request, CancellationToken cancellationToken)
    {
        // البحث عن النوع
        var violationType = await _context.ViolationTypes
            .FirstOrDefaultAsync(v => v.ViolationTypeId == request.ViolationTypeId, cancellationToken);

        if (violationType == null)
            return Result<bool>.Failure("نوع المخالفة غير موجود");

        // التحقق من عدم تكرار الاسم (مع استثناء السجل الحالي)
        var duplicateExists = await _context.ViolationTypes
            .AnyAsync(v => v.ViolationNameAr == request.ViolationNameAr 
                        && v.ViolationTypeId != request.ViolationTypeId, cancellationToken);

        if (duplicateExists)
            return Result<bool>.Failure("يوجد نوع مخالفة آخر بنفس الاسم");

        // تحديث البيانات
        violationType.ViolationNameAr = request.ViolationNameAr;
        violationType.Description = request.Description;
        violationType.SeverityLevel = request.SeverityLevel;
        violationType.UpdatedBy = _currentUserService.UserId;
        violationType.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true, "تم تحديث نوع المخالفة بنجاح");
    }
}

/// <summary>
/// قواعد التحقق
/// </summary>
public class UpdateViolationTypeCommandValidator : AbstractValidator<UpdateViolationTypeCommand>
{
    public UpdateViolationTypeCommandValidator()
    {
        RuleFor(x => x.ViolationTypeId)
            .GreaterThan(0).WithMessage("معرف نوع المخالفة مطلوب");

        RuleFor(x => x.ViolationNameAr)
            .NotEmpty().WithMessage("اسم المخالفة بالعربية مطلوب")
            .MaximumLength(200).WithMessage("اسم المخالفة لا يمكن أن يتجاوز 200 حرف");

        RuleFor(x => x.SeverityLevel)
            .InclusiveBetween((byte)1, (byte)3)
            .WithMessage("مستوى الخطورة يجب أن يكون 1 (بسيط)، 2 (متوسط)، أو 3 (جسيم)");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("الوصف لا يمكن أن يتجاوز 500 حرف")
            .When(x => !string.IsNullOrEmpty(x.Description));
    }
}
