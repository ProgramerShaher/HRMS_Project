using FluentValidation;
using HRMS.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Core.Departments.Commands.CreateDepartment;

public class CreateDepartmentCommandValidator : AbstractValidator<CreateDepartmentCommand>
{
    private readonly IApplicationDbContext _context;

    public CreateDepartmentCommandValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(x => x.DeptNameAr)
            .NotEmpty().WithMessage("اسم القسم بالعربية مطلوب")
            .MaximumLength(100).WithMessage("اسم القسم لا يمكن أن يتجاوز 100 حرف")
            .MustAsync(BeUniqueName).WithMessage("اسم القسم موجود مسبقاً");

        RuleFor(x => x.ParentDeptId)
            .MustAsync(ParentExists).When(x => x.ParentDeptId.HasValue)
            .WithMessage("القسم الأب غير موجود");
    }

    private async Task<bool> BeUniqueName(string nameAr, CancellationToken cancellationToken)
    {
        return !await _context.Departments.AnyAsync(d => d.DeptNameAr == nameAr, cancellationToken);
    }

    private async Task<bool> ParentExists(int? parentId, CancellationToken cancellationToken)
    {
        if (!parentId.HasValue) return true;
        return await _context.Departments.AnyAsync(d => d.DeptId == parentId.Value, cancellationToken);
    }
}
