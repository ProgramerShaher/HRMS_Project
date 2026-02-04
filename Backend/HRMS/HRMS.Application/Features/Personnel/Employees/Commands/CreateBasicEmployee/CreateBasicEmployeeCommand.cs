using AutoMapper;
using HRMS.Application.DTOs.Personnel;
using HRMS.Application.Interfaces;
using HRMS.Core.Entities.Personnel;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Personnel.Employees.Commands.CreateBasicEmployee;

public class CreateBasicEmployeeCommand : IRequest<int>
{
    public CreateBasicEmployeeDto Data { get; set; }

    public CreateBasicEmployeeCommand(CreateBasicEmployeeDto data)
    {
        Data = data;
    }
}

public class CreateBasicEmployeeCommandHandler : IRequestHandler<CreateBasicEmployeeCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public CreateBasicEmployeeCommandHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<int> Handle(CreateBasicEmployeeCommand request, CancellationToken cancellationToken)
    {
        // 1. Generate Auto-Number (Year + Sequence)
        var year = DateTime.Now.Year;
        // Get last number for this year to increment
        var lastEmployee = await _context.Employees
            .Where(e => e.EmployeeNumber.StartsWith(year.ToString()))
            .OrderByDescending(e => e.EmployeeNumber)
            .FirstOrDefaultAsync(cancellationToken);

        int nextSequence = 1;
        if (lastEmployee != null && lastEmployee.EmployeeNumber.Length > 4)
        {
            if (int.TryParse(lastEmployee.EmployeeNumber.Substring(4), out int lastSeq))
                nextSequence = lastSeq + 1;
        }

        var employeeNumber = $"{year}{nextSequence:D4}"; // e.g., 20260001

        // 2. Map Data
        var employee = _mapper.Map<Employee>(request.Data);
        employee.EmployeeNumber = employeeNumber;
        employee.CreatedAt = DateTime.UtcNow;

        // 3. Save
        _context.Employees.Add(employee);
        await _context.SaveChangesAsync(cancellationToken);

        return employee.EmployeeId;
    }
}
