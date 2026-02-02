using FluentValidation;
using HRMS.Application.Interfaces;
using HRMS.Core.Entities.Leaves;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Leaves.LeaveTypes.Commands.CreateLeaveType;

// ═══════════════════════════════════════════════════════════════════════════
// 1. COMMAND - الأمر
// ═══════════════════════════════════════════════════════════════════════════

/// <summary>
/// أمر إنشاء نوع إجازة جديد
/// Command to create a new leave type.
/// </summary>
public record CreateLeaveTypeCommand : IRequest<Result<int>>
{
    /// <summary>
    /// اسم نوع الإجازة بالعربية
    /// Leave type name in Arabic
    /// </summary>
    public string LeaveTypeNameAr { get; init; } = string.Empty;

    /// <summary>
    /// عدد الأيام الافتراضية
    /// Default number of days
    /// </summary>
    public short DefaultDays { get; init; }

    /// <summary>
    /// هل يخصم من الرصيد السنوي (1=نعم، 0=لا)
    /// Is Deductible (1=Yes, 0=No)
    /// </summary>
    public byte IsDeductible { get; init; }

    /// <summary>
    /// هل يتطلب مرفق (1=نعم، 0=لا)
    /// Requires Attachment (1=Yes, 0=No)
    /// </summary>
    public byte RequiresAttachment { get; init; }
}

// ═══════════════════════════════════════════════════════════════════════════
// 2. VALIDATOR - المدقق
// ═══════════════════════════════════════════════════════════════════════════

/// <summary>
/// مدقق أمر إضافة نوع إجازة
/// Validator for CreateLeaveTypeCommand.
/// </summary>
public class CreateLeaveTypeCommandValidator : AbstractValidator<CreateLeaveTypeCommand>
{
    public CreateLeaveTypeCommandValidator()
    {
        // التحقق من الاسم العربي
        // Why: الاسم العربي هو الحقل الأساسي للتعريف
        RuleFor(x => x.LeaveTypeNameAr)
            .NotEmpty().WithMessage("اسم الإجازة بالعربية مطلوب")
            .MaximumLength(50).WithMessage("اسم الإجازة بالعربية يجب ألا يتجاوز 50 حرفاً");

        // التحقق من عدد الأيام الافتراضي
        // Why: يجب أن تكون الأيام منطقية (ليست سالبة ولا تتجاوز السنة)
        RuleFor(x => x.DefaultDays)
            .GreaterThan((short)0).WithMessage("عدد الأيام الافتراضي يجب أن يكون أكبر من 0")
            .LessThanOrEqualTo((short)365).WithMessage("عدد الأيام الافتراضي يجب ألا يتجاوز 365 يوماً");

        // التحقق من IsDeductible
        // Why: القيم المسموحة فقط 0 أو 1
        RuleFor(x => x.IsDeductible)
            .InclusiveBetween((byte)0, (byte)1).WithMessage("قيمة IsDeductible يجب أن تكون 0 أو 1");

        // التحقق من RequiresAttachment
        // Why: القيم المسموحة فقط 0 أو 1
        RuleFor(x => x.RequiresAttachment)
            .InclusiveBetween((byte)0, (byte)1).WithMessage("قيمة RequiresAttachment يجب أن تكون 0 أو 1");
    }
}

// ═══════════════════════════════════════════════════════════════════════════
// 3. HANDLER - معالج الأمر
// ═══════════════════════════════════════════════════════════════════════════

/// <summary>
/// معالج أمر إضافة نوع إجازة جديد
/// Handler for creating a new leave type with strict validation.
/// </summary>
public class CreateLeaveTypeCommandHandler : IRequestHandler<CreateLeaveTypeCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;

    public CreateLeaveTypeCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<int>> Handle(CreateLeaveTypeCommand request, CancellationToken cancellationToken)
    {
        // ═══════════════════════════════════════════════════════════════════════════
        // الخطوة 1: التحقق الدفاعي (Defensive Programming)
        // Step 1: Manual Defensive Validation
        // ═══════════════════════════════════════════════════════════════════════════

        // نقوم بتنظيف الاسم من المسافات الزائدة
        // Why: لتجنب الأخطاء الناتجة عن المسافات غير المقصودة
        var trimmedName = request.LeaveTypeNameAr?.Trim();

        // 1. التحقق من أن الاسم ليس فارغاً بعد التنظيف
        // Why: لضمان عدم إدخال أسماء فارغة أو تحتوي فقط على مسافات
        if (string.IsNullOrEmpty(trimmedName))
        {
            return Result<int>.Failure("اسم نوع الإجازة لا يمكن أن يكون فارغاً", 400);
        }

        // 2. التحقق من أن عدد الأيام أكبر من الصفر
        // Why: لضمان منطقية البيانات (Zero Value Check)
        if (request.DefaultDays <= 0)
        {
            return Result<int>.Failure("عدد الأيام الافتراضي يجب أن يكون أكبر من صفر", 400);
        }

        // 3. التحقق القوي من التكرار (Robust Duplicate Check)
        // Why: لمنع تكرار أنواع الإجازات بنفس الاسم (Masatara Compliance)
        var isDuplicate = await _context.LeaveTypes
            .AnyAsync(lt => lt.LeaveNameAr == trimmedName && lt.IsDeleted == 0, cancellationToken);

        if (isDuplicate)
        {
            return Result<int>.Failure($"نوع الإجازة '{trimmedName}' موجود بالفعل", 409);
        }

        // ═══════════════════════════════════════════════════════════════════════════
        // الخطوة 2: إنشاء الكيان وحفظه
        // Step 2: Create and Save Entity
        // ═══════════════════════════════════════════════════════════════════════════

        var leaveType = new LeaveType
        {
            LeaveNameAr = trimmedName,
            DefaultDays = request.DefaultDays,
            IsDeductible = request.IsDeductible,
            RequiresAttachment = request.RequiresAttachment
        };

        _context.LeaveTypes.Add(leaveType);
        await _context.SaveChangesAsync(cancellationToken);

        // ═══════════════════════════════════════════════════════════════════════════
        // الخطوة 3: إرجاع النتيجة
        // Step 3: Return Result
        // ═══════════════════════════════════════════════════════════════════════════

        return Result<int>.Success(leaveType.LeaveTypeId, "تم إضافة نوع الإجازة بنجاح");
    }
}
