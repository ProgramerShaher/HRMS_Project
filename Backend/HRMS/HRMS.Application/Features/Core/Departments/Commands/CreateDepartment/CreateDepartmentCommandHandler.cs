using HRMS.Core.Entities.Core;
using HRMS.Infrastructure.Data;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace HRMS.Application.Features.Core.Departments.Commands.CreateDepartment
{
    public class CreateDepartmentCommandHandler : IRequestHandler<CreateDepartmentCommand, int>
    {
        private readonly HRMSDbContext _context;
        public CreateDepartmentCommandHandler(HRMSDbContext context) => _context = context;

        public async Task<int> Handle(CreateDepartmentCommand request, CancellationToken cancellationToken)
        {
            var dept = new Department
            {
                DeptNameAr = request.DeptNameAr,
                DeptNameEn = request.DeptNameEn,
                ParentDeptId = request.ParentDeptId,
                ManagerId = request.ManagerId,
                CreatedBy = "API_USER",
                CreatedAt = DateTime.Now
            };

            _context.Departments.Add(dept);
            await _context.SaveChangesAsync(cancellationToken);
            return dept.DeptId;
        }
    }
}
