using MediatR;
using Microsoft.EntityFrameworkCore;
using HRMS.Application.Interfaces;

namespace HRMS.Application.Features.Core.Departments.Commands.DeleteDepartment;

public class DeleteDepartmentCommandHandler : IRequestHandler<DeleteDepartmentCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public DeleteDepartmentCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeleteDepartmentCommand request, CancellationToken cancellationToken)
    {
        var department = await _context.Departments
            .Include(d => d.SubDepartments)
            .Include(d => d.Jobs)
            .FirstOrDefaultAsync(d => d.DeptId == request.DeptId && d.IsDeleted == 0, cancellationToken);

        if (department == null)
            throw new KeyNotFoundException($"القسم برقم {request.DeptId} غير موجود");

        // منع الحذف إذا كان لديه أقسام فرعية
        if (department.SubDepartments.Any(sd => sd.IsDeleted == 0))
            throw new InvalidOperationException("لا يمكن حذف القسم لأنه يحتوي على أقسام فرعية");

        // منع الحذف إذا كان لديه وظائف
        if (department.Jobs.Any(j => j.IsDeleted == 0))
            throw new InvalidOperationException("لا يمكن حذف القسم لأنه يحتوي على وظائف");

        // Soft Delete
        department.IsDeleted = 1;
        department.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
