using FluentValidation;
using HRMS.Application.Interfaces;
using HRMS.Core.Entities.Performance;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Performance.ViolationTypes.Commands.Create;

/// <summary>
/// أمر إنشاء نوع مخالفة جديد
/// </summary>
public class CreateViolationTypeCommand : IRequest<Result<int>>
{
    /// <summary>
    /// اسم المخالفة بالعربية (مطلوب)
    /// </summary>
    public string ViolationNameAr { get; set; } = string.Empty;

    /// <summary>
    /// اسم المخالفة بالإنجليزية (اختياري)
    /// </summary>
    public string? ViolationNameEn { get; set; }

    /// <summary>
    /// وصف المخالفة
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// مستوى الخطورة (LOW, MEDIUM, HIGH, CRITICAL)
    /// </summary>
    public string Severity { get; set; } = "MEDIUM";
}

/// <summary>
/// معالج أمر إنشاء نوع مخالفة
/// </summary>
public class CreateViolationTypeCommandHandler : IRequestHandler<CreateViolationTypeCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public CreateViolationTypeCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result<int>> Handle(CreateViolationTypeCommand request, CancellationToken cancellationToken)
    {
        // التحقق من عدم تكرار الاسم
        var exists = await _context.ViolationTypes
            .AnyAsync(v => v.ViolationNameAr == request.ViolationNameAr, cancellationToken);

        if (exists)
            return Result<int>.Failure("يوجد نوع مخالفة بنفس الاسم مسبقاً");

        // إنشاء النوع الجديد
        var violationType = new ViolationType
        {
            ViolationNameAr = request.ViolationNameAr,
            CreatedBy = _currentUserService.UserId,
            CreatedAt = DateTime.UtcNow
        };

        _context.ViolationTypes.Add(violationType);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<int>.Success(violationType.ViolationTypeId, "تم إنشاء نوع المخالفة بنجاح");
    }
}

/// <summary>
/// قواعد التحقق من صحة البيانات
/// </summary>
public class CreateViolationTypeCommandValidator : AbstractValidator<CreateViolationTypeCommand>
{
    public CreateViolationTypeCommandValidator()
    {
        RuleFor(x => x.ViolationNameAr)
            .NotEmpty().WithMessage("اسم المخالفة بالعربية مطلوب")
            .MaximumLength(200).WithMessage("اسم المخالفة لا يمكن أن يتجاوز 200 حرف");

        RuleFor(x => x.ViolationNameEn)
            .MaximumLength(200).WithMessage("الاسم الإنجليزي لا يمكن أن يتجاوز 200 حرف")
            .When(x => !string.IsNullOrEmpty(x.ViolationNameEn));

        RuleFor(x => x.Severity)
            .NotEmpty().WithMessage("مستوى الخطورة مطلوب")
            .Must(s => new[] { "LOW", "MEDIUM", "HIGH", "CRITICAL" }.Contains(s))
            .WithMessage("مستوى الخطورة يجب أن يكون أحد القيم: LOW, MEDIUM, HIGH, CRITICAL");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("الوصف لا يمكن أن يتجاوز 500 حرف")
            .When(x => !string.IsNullOrEmpty(x.Description));
    }
}
