using MediatR;
using Microsoft.EntityFrameworkCore;
using HRMS.Application.Interfaces;

namespace HRMS.Application.Features.Core.Departments.Commands.UpdateDepartment;

public class UpdateDepartmentCommandHandler : IRequestHandler<UpdateDepartmentCommand, int>
{
    private readonly IApplicationDbContext _context;

    public UpdateDepartmentCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(UpdateDepartmentCommand request, CancellationToken cancellationToken)
    {
        var department = await _context.Departments
            .FirstOrDefaultAsync(d => d.DeptId == request.DeptId && d.IsDeleted == 0, cancellationToken);

        if (department == null)
            throw new KeyNotFoundException($"القسم برقم {request.DeptId} غير موجود");

        department.DeptNameAr = request.DeptNameAr;
        department.DeptNameEn = request.DeptNameEn;
        department.ParentDeptId = request.ParentDeptId;
        department.CostCenterCode = request.CostCenterCode;
        department.ManagerId = request.ManagerId;
        department.IsActive = request.IsActive;
        department.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return department.DeptId;
    }
}
