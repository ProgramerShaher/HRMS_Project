using FluentValidation;
using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Performance.ViolationTypes.Commands.Delete;

/// <summary>
/// أمر حذف نوع مخالفة
/// </summary>
public class DeleteViolationTypeCommand : IRequest<Result<bool>>
{
    /// <summary>
    /// معرف نوع المخالفة المراد حذفه
    /// </summary>
    public int ViolationTypeId { get; set; }
}

/// <summary>
/// معالج أمر الحذف
/// </summary>
public class DeleteViolationTypeCommandHandler : IRequestHandler<DeleteViolationTypeCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;

    public DeleteViolationTypeCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> Handle(DeleteViolationTypeCommand request, CancellationToken cancellationToken)
    {
        // البحث عن النوع
        var violationType = await _context.ViolationTypes
            .FirstOrDefaultAsync(v => v.ViolationTypeId == request.ViolationTypeId, cancellationToken);

        if (violationType == null)
            return Result<bool>.Failure("نوع المخالفة غير موجود");

        // ✅ التحقق من عدم وجود مخالفات مرتبطة
        var hasViolations = await _context.EmployeeViolations
            .AnyAsync(v => v.ViolationTypeId == request.ViolationTypeId, cancellationToken);

        if (hasViolations)
            return Result<bool>.Failure("لا يمكن حذف نوع المخالفة لأنه مستخدم في سجلات موجودة");

        // الحذف
        _context.ViolationTypes.Remove(violationType);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true, "تم حذف نوع المخالفة بنجاح");
    }
}

/// <summary>
/// قواعد التحقق
/// </summary>
public class DeleteViolationTypeCommandValidator : AbstractValidator<DeleteViolationTypeCommand>
{
    public DeleteViolationTypeCommandValidator()
    {
        RuleFor(x => x.ViolationTypeId)
            .GreaterThan(0).WithMessage("معرف نوع المخالفة مطلوب");
    }
}
