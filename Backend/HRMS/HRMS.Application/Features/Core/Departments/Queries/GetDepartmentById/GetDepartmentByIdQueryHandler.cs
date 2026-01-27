using MediatR;
using Microsoft.EntityFrameworkCore;
using HRMS.Application.Interfaces;
using HRMS.Application.DTOs.Core;
using AutoMapper;
using AutoMapper.QueryableExtensions;

namespace HRMS.Application.Features.Core.Departments.Queries.GetDepartmentById;

public class GetDepartmentByIdQueryHandler : IRequestHandler<GetDepartmentByIdQuery, DepartmentDto?>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetDepartmentByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<DepartmentDto?> Handle(GetDepartmentByIdQuery request, CancellationToken cancellationToken)
    {
        return await _context.Departments
            .Where(d => d.DeptId == request.DeptId && d.IsDeleted == 0)
            .ProjectTo<DepartmentDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
