using AutoMapper;
using HRMS.Application.Interfaces;
using HRMS.Core.Entities.Personnel;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Personnel.Employees.Commands.CreateEmployee;

public class CreateEmployeeCommandHandler : IRequestHandler<CreateEmployeeCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public CreateEmployeeCommandHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<int> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
    {
        // Execute in Transaction to ensure Sub-Entities consistency
        using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        try
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

            var employeeNumber = $"{year}{nextSequence:D4}"; // e.g., 20260001 (4 digits year + 4 digits sequence)

            // 2. Map Employee Core Data
            var employee = _mapper.Map<Employee>(request.Data);
            employee.EmployeeNumber = employeeNumber;
            employee.CreatedAt = DateTime.UtcNow;
            
            // 3. Map Financial Data (Sub-Entity)
            var compensation = _mapper.Map<EmployeeCompensation>(request.Data);
            compensation.CreatedAt = DateTime.UtcNow;
            
            // Link One-to-One
            employee.Compensation = compensation;

            // 4. Save Aggregate Root
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync(cancellationToken);

            // Commit Transaction
            await transaction.CommitAsync(cancellationToken);

            return employee.EmployeeId;
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
