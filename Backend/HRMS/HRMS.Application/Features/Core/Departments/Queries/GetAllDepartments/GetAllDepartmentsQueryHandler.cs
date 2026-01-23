using HRMS.Core.Entities.Core;
using HRMS.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HRMS.Application.Features.Core.Departments.Queries.GetAllDepartments
{
    public class GetAllDepartmentsQueryHandler : IRequestHandler<GetAllDepartmentsQuery, List<Department>>
    {
        private readonly HRMSDbContext _context;
        public GetAllDepartmentsQueryHandler(HRMSDbContext context) => _context = context;

        public async Task<List<Department>> Handle(GetAllDepartmentsQuery request, CancellationToken cancellationToken)
        {
            return await _context.Departments.AsNoTracking().ToListAsync(cancellationToken);
        }
    }
}
