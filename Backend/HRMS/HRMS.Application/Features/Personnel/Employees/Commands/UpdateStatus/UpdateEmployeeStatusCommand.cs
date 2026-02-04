using FluentValidation;
using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Personnel.Employees.Commands.UpdateStatus;

/// <summary>
/// أمر تحديث حالة الموظف (تفعيل/تعطيل/إنهاء خدمة)
/// Update Employee Status Command
/// </summary>
public class UpdateEmployeeStatusCommand : IRequest<Result<bool>>
{
    /// <summary>
    /// معرف الموظف
    /// </summary>
    public int EmployeeId { get; set; }

    /// <summary>
    /// الحالة الجديدة (Active/Inactive/Terminated)
    /// </summary>
    public string NewStatus { get; set; } = string.Empty;

    /// <summary>
    /// سبب التغيير
    /// </summary>
    public string Reason { get; set; } = string.Empty;
}

/// <summary>
/// معالج أمر تحديث حالة الموظف
/// </summary>
public class UpdateEmployeeStatusCommandHandler : IRequestHandler<UpdateEmployeeStatusCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;

    public UpdateEmployeeStatusCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> Handle(UpdateEmployeeStatusCommand request, CancellationToken cancellationToken)
    {
        // 1. جلب الموظف
        var employee = await _context.Employees
            .FirstOrDefaultAsync(e => e.EmployeeId == request.EmployeeId, cancellationToken);

        if (employee == null)
            return Result<bool>.Failure($"الموظف برقم {request.EmployeeId} غير موجود");

        // 2. تحديث الحالة
        switch (request.NewStatus.ToUpper())
        {
            case "ACTIVE":
                employee.IsActive = true;
                employee.TerminationDate = null;
                break;

            case "INACTIVE":
                employee.IsActive = false;
                employee.TerminationDate = null;
                break;

            case "TERMINATED":
                employee.IsActive = false;
                employee.TerminationDate = DateTime.Now;
                break;

            default:
                return Result<bool>.Failure($"الحالة '{request.NewStatus}' غير صحيحة. القيم المسموحة: Active, Inactive, Terminated");
        }

        // 3. حفظ التغييرات (سيتم تسجيلها تلقائياً في Audit Trail)
        await _context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(
            true,
            $"تم تحديث حالة الموظف إلى '{request.NewStatus}' بنجاح. السبب: {request.Reason}");
    }
}

/// <summary>
/// التحقق من صحة أمر تحديث حالة الموظف
/// </summary>
public class UpdateEmployeeStatusCommandValidator : AbstractValidator<UpdateEmployeeStatusCommand>
{
    private static readonly string[] AllowedStatuses = { "ACTIVE", "INACTIVE", "TERMINATED" };

    public UpdateEmployeeStatusCommandValidator()
    {
        RuleFor(x => x.EmployeeId)
            .GreaterThan(0)
            .WithMessage("معرف الموظف غير صحيح");

        RuleFor(x => x.NewStatus)
            .NotEmpty()
            .WithMessage("الحالة الجديدة مطلوبة")
            .Must(BeValidStatus)
            .WithMessage("الحالة يجب أن تكون إحدى القيم التالية: Active, Inactive, Terminated");

        RuleFor(x => x.Reason)
            .NotEmpty()
            .WithMessage("سبب التغيير مطلوب")
            .MinimumLength(10)
            .WithMessage("سبب التغيير يجب أن يكون على الأقل 10 أحرف")
            .MaximumLength(500)
            .WithMessage("سبب التغيير يجب ألا يتجاوز 500 حرف");
    }

    private bool BeValidStatus(string status)
    {
        return AllowedStatuses.Contains(status.ToUpper());
    }
}
