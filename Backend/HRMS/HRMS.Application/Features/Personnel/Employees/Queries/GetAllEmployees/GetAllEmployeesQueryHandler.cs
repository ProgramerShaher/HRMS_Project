using AutoMapper;
using HRMS.Application.Features.Personnel.Employees.DTOs;
using HRMS.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HRMS.Application.Features.Personnel.Employees.Queries.GetAllEmployees
{
    public class GetAllEmployeesQueryHandler : IRequestHandler<GetAllEmployeesQuery, List<EmployeeDto>>
    {
        private readonly HRMSDbContext _context;
        private readonly IMapper _mapper;

        public GetAllEmployeesQueryHandler(HRMSDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<EmployeeDto>> Handle(GetAllEmployeesQuery request, CancellationToken cancellationToken)
        {
            var employees = await _context.Employees
                .AsNoTracking()
                .Where(e => e.Status == "ACTIVE")
                .Include(e => e.Nationality)
                .Include(e => e.Job)
                .Include(e => e.Department)
                .OrderBy(e => e.EmployeeNumber)
                .ToListAsync(cancellationToken);

            return _mapper.Map<List<EmployeeDto>>(employees);
        }
    }
}
