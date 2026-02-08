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
    /// وصف المخالفة
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// مستوى الخطورة: 1=بسيط، 2=متوسط، 3=جسيم
    /// </summary>
    public byte SeverityLevel { get; set; } = 1;
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
            Description = request.Description,
            SeverityLevel = request.SeverityLevel,
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

        RuleFor(x => x.SeverityLevel)
            .InclusiveBetween((byte)1, (byte)3)
            .WithMessage("مستوى الخطورة يجب أن يكون 1 (بسيط)، 2 (متوسط)، أو 3 (جسيم)");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("الوصف لا يمكن أن يتجاوز 500 حرف")
            .When(x => !string.IsNullOrEmpty(x.Description));
    }
}
