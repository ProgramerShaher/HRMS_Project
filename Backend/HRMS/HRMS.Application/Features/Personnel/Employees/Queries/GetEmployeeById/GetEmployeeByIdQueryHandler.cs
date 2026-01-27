using AutoMapper;
using HRMS.Application.Features.Personnel.Employees.DTOs;
using HRMS.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace HRMS.Application.Features.Personnel.Employees.Queries.GetEmployeeById
{
    public class GetEmployeeByIdQueryHandler : IRequestHandler<GetEmployeeByIdQuery, EmployeeDto>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public GetEmployeeByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<EmployeeDto> Handle(GetEmployeeByIdQuery request, CancellationToken cancellationToken)
        {
            var employee = await _context.Employees
                .AsNoTracking()
                .Include(e => e.Country)
                .Include(e => e.Job)
                .Include(e => e.Department)
                .FirstOrDefaultAsync(e => e.EmployeeId == request.Id, cancellationToken);

            if (employee == null)
                return null;

            return _mapper.Map<EmployeeDto>(employee);
        }
    }
}
