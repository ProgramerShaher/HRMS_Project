using AutoMapper;
using HRMS.Application.Interfaces;
using HRMS.Core.Entities.Personnel;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Personnel.Employees.Commands.UpdateEmployee;

public class UpdateEmployeeCommandHandler : IRequestHandler<UpdateEmployeeCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public UpdateEmployeeCommandHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<int> Handle(UpdateEmployeeCommand request, CancellationToken cancellationToken)
    {
        // 1. Fetch Aggregate Root with All Sub-Entities
        var employee = await _context.Employees
            .Include(e => e.Compensation)
            .Include(e => e.Qualifications)
            .Include(e => e.Experiences)
            .Include(e => e.EmergencyContacts)
            .Include(e => e.Contracts)
            .Include(e => e.Certifications)
            .Include(e => e.BankAccounts)
            .Include(e => e.Dependents)
            .Include(e => e.Addresses)
            .Include(e => e.Documents)
            .FirstOrDefaultAsync(e => e.EmployeeId == request.EmployeeId, cancellationToken);

        if (employee == null)
            throw new KeyNotFoundException($"Employee {request.EmployeeId} not found");

        // 2. Update Core Data
        _mapper.Map(request.Data, employee);
        employee.UpdatedAt = DateTime.UtcNow;

        // 3. Update Financial Data
        if (employee.Compensation != null)
        {
            _mapper.Map(request.Data, employee.Compensation);
            employee.Compensation.UpdatedAt = DateTime.UtcNow;
        }

        // 4. Differential Update for Collections

        // Qualifications
        UpdateCollection(
            employee.Qualifications,
            request.Data.Qualifications,
            (existing, dto) => existing.QualificationId == dto.QualificationId,
            (existing, dto) => _mapper.Map(dto, existing)
        );

        // Experiences
        UpdateCollection(
            employee.Experiences,
            request.Data.Experiences,
            (existing, dto) => existing.ExperienceId == dto.ExperienceId,
            (existing, dto) => _mapper.Map(dto, existing)
        );

        // Emergency Contacts
        UpdateCollection(
            employee.EmergencyContacts,
            request.Data.EmergencyContacts,
            (existing, dto) => existing.ContactId == dto.ContactId,
            (existing, dto) => _mapper.Map(dto, existing)
        );

        // --- NEW COLLECTIONS ---

        // Contracts
        UpdateCollection(
            employee.Contracts,
            request.Data.Contracts,
            (existing, dto) => existing.ContractId == dto.ContractId,
            (existing, dto) => _mapper.Map(dto, existing)
        );

        // Certifications
        UpdateCollection(
            employee.Certifications,
            request.Data.Certifications,
            (existing, dto) => existing.CertId == dto.CertificationId,
            (existing, dto) => _mapper.Map(dto, existing)
        );

        // Bank Accounts
        UpdateCollection(
            employee.BankAccounts,
            request.Data.BankAccounts,
            (existing, dto) => existing.BankId == dto.EmployeeBankAccountId,
            (existing, dto) => _mapper.Map(dto, existing)
        );

        // Dependents
        UpdateCollection(
            employee.Dependents,
            request.Data.Dependents,
            (existing, dto) => existing.DependentId == dto.DependentId,
            (existing, dto) => _mapper.Map(dto, existing)
        );

        // Addresses
        UpdateCollection(
            employee.Addresses,
            request.Data.Addresses,
            (existing, dto) => existing.AddressId == dto.AddressId,
            (existing, dto) => _mapper.Map(dto, existing)
        );

        // Documents
        UpdateCollection(
            employee.Documents,
            request.Data.Documents,
            (existing, dto) => existing.DocumentId == dto.DocumentId,
            (existing, dto) => _mapper.Map(dto, existing)
        );

        // --- Salary Sync Logic ---
        // Find active contract to sync compensation
        var activeContract = employee.Contracts
            .OrderByDescending(c => c.StartDate)
            .FirstOrDefault(c => c.ContractStatus == "ACTIVE" || c.ContractStatus == null); // Fallback to latest

        if (activeContract != null)
        {
            if (employee.Compensation == null)
            {
                employee.Compensation = new EmployeeCompensation { EmployeeId = employee.EmployeeId };
                _context.EmployeeCompensations.Add(employee.Compensation);
            }
            
            employee.Compensation.BasicSalary = activeContract.BasicSalary;
            employee.Compensation.HousingAllowance = activeContract.HousingAllowance;
            employee.Compensation.TransportAllowance = activeContract.TransportAllowance;
            employee.Compensation.OtherAllowances = activeContract.OtherAllowances;
            // Bank info could also be synced if present in Contract? Usually not.
        }

        // 5. Save
        await _context.SaveChangesAsync(cancellationToken);

        return employee.EmployeeId;
    }

    private void UpdateCollection<TEntity, TDto>(
        ICollection<TEntity> currentItems,
        IEnumerable<TDto> newItems,
        Func<TEntity, TDto, bool> matchKey,
        Action<TEntity, TDto> mapper)
        where TEntity : class
    {
        var newItemsList = newItems?.ToList() ?? new List<TDto>();

        // Identify items to remove
        var itemsToRemove = currentItems.Where(existing => 
            !newItemsList.Any(newItem => matchKey(existing, newItem))).ToList();

        foreach (var item in itemsToRemove)
        {
            currentItems.Remove(item);
        }

        // Identify items to add or update
        foreach (var newItem in newItemsList)
        {
            var existingItem = currentItems.FirstOrDefault(existing => matchKey(existing, newItem));

            if (existingItem != null)
            {
                // Update
                mapper(existingItem, newItem);
            }
            else
            {
                // Add
                // Create new instance using Activator or expected mapper behavior
                // But better: Use AutoMapper to create the instance directly from DTO
                var entity = _mapper.Map<TEntity>(newItem);
                currentItems.Add(entity);
            }
        }
    }
}
