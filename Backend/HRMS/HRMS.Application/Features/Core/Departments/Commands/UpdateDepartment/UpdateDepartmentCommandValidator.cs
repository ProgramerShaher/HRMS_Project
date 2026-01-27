using FluentValidation;
using HRMS.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Core.Departments.Commands.UpdateDepartment;

public class UpdateDepartmentCommandValidator : AbstractValidator<UpdateDepartmentCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateDepartmentCommandValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(x => x.DeptId)
            .GreaterThan(0).WithMessage("معرف القسم غير صحيح");

        RuleFor(x => x.DeptNameAr)
            .NotEmpty().WithMessage("اسم القسم بالعربية مطلوب")
            .MaximumLength(100).WithMessage("اسم القسم لا يمكن أن يتجاوز 100 حرف")
            .MustAsync(BeUniqueName).WithMessage("اسم القسم موجود مسبقاً");

        RuleFor(x => x.ParentDeptId)
            .MustAsync(ParentExists).When(x => x.ParentDeptId.HasValue)
            .WithMessage("القسم الأب غير موجود")
            .MustAsync(NotCircular).When(x => x.ParentDeptId.HasValue)
            .WithMessage("لا يمكن جعل القسم أب لنفسه");
    }

    private async Task<bool> BeUniqueName(UpdateDepartmentCommand command, string nameAr, CancellationToken cancellationToken)
    {
        return !await _context.Departments.AnyAsync(
            d => d.DeptNameAr == nameAr && d.DeptId != command.DeptId && d.IsDeleted == 0, 
            cancellationToken);
    }

    private async Task<bool> ParentExists(int? parentId, CancellationToken cancellationToken)
    {
        if (!parentId.HasValue) return true;
        return await _context.Departments.AnyAsync(d => d.DeptId == parentId.Value && d.IsDeleted == 0, cancellationToken);
    }

    private async Task<bool> NotCircular(UpdateDepartmentCommand command, int? parentId, CancellationToken cancellationToken)
    {
        if (!parentId.HasValue) return true;
        return parentId.Value != command.DeptId;
    }
}
