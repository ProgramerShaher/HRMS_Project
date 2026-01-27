using MediatR;
using HRMS.Application.Interfaces;
using HRMS.Core.Entities.Core;
using AutoMapper;

namespace HRMS.Application.Features.Core.Departments.Commands.CreateDepartment;

public class CreateDepartmentCommandHandler : IRequestHandler<CreateDepartmentCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public CreateDepartmentCommandHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<int> Handle(CreateDepartmentCommand request, CancellationToken cancellationToken)
    {
        var department = _mapper.Map<Department>(request);
        department.CreatedAt = DateTime.UtcNow;

        _context.Departments.Add(department);
        await _context.SaveChangesAsync(cancellationToken);

        return department.DeptId;
    }
}
