using AutoMapper;
using HRMS.Application.DTOs.Personnel;
using HRMS.Application.Interfaces;
using HRMS.Core.Entities.Personnel;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Personnel.Contracts.Commands.CreateContract;

public record CreateContractCommand(CreateContractDto Data) : IRequest<int>;

public class CreateContractCommandHandler : IRequestHandler<CreateContractCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public CreateContractCommandHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<int> Handle(CreateContractCommand request, CancellationToken cancellationToken)
    {
        // 0. Pre-save Validation: Check if Employee Exists
        var employeeExists = await _context.Employees.AnyAsync(e => e.EmployeeId == request.Data.EmployeeId, cancellationToken);
        if (!employeeExists)
        {
            throw new Exceptions.NotFoundException($"Employee {request.Data.EmployeeId} not found.");
        }

        // 1. Create Contract
        var contract = _mapper.Map<Contract>(request.Data);
        _context.Contracts.Add(contract);

        // 2. Auto-Sync Compensation
        var compensation = await _context.EmployeeCompensations
            .FirstOrDefaultAsync(c => c.EmployeeId == request.Data.EmployeeId, cancellationToken);

        if (compensation == null)
        {
            compensation = new EmployeeCompensation
            {
                EmployeeId = request.Data.EmployeeId,
                BasicSalary = request.Data.BasicSalary,
                HousingAllowance = request.Data.HousingAllowance,
                TransportAllowance = request.Data.TransportAllowance,
                OtherAllowances = request.Data.OtherAllowances
            };
            _context.EmployeeCompensations.Add(compensation);
        }
        else
        {
            compensation.BasicSalary = request.Data.BasicSalary;
            compensation.HousingAllowance = request.Data.HousingAllowance;
            compensation.TransportAllowance = request.Data.TransportAllowance;
            compensation.OtherAllowances = request.Data.OtherAllowances;
            // Existing compensation updated
        }

        await _context.SaveChangesAsync(cancellationToken);

        return contract.ContractId;
    }
}
