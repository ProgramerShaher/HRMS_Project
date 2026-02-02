using HRMS.Application.DTOs.Payroll.Configuration;
using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Payroll.Configuration.Structure.Queries.GetEmployeeSalaryStructure;

public record GetEmployeeSalaryStructureQuery(int EmployeeId) : IRequest<Result<EmployeeSalaryStructureDto>>;

public class GetEmployeeSalaryStructureQueryHandler : IRequestHandler<GetEmployeeSalaryStructureQuery, Result<EmployeeSalaryStructureDto>>
{
    private readonly IApplicationDbContext _context;

    public GetEmployeeSalaryStructureQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<EmployeeSalaryStructureDto>> Handle(GetEmployeeSalaryStructureQuery request, CancellationToken cancellationToken)
    {
        var employee = await _context.Employees
            .Include(e => e.Job) // Include Job
            .ThenInclude(j => j.DefaultGrade) // Include Grade
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.EmployeeId == request.EmployeeId, cancellationToken);

        if (employee == null)
            return Result<EmployeeSalaryStructureDto>.Failure("الموظف غير موجود");

        var structures = await _context.SalaryStructures
            .Include(s => s.SalaryElement)
            .Where(s => s.EmployeeId == request.EmployeeId && s.IsActive == 1)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var dto = new EmployeeSalaryStructureDto
        {
            EmployeeId = employee.EmployeeId,
            EmployeeName = employee.FullNameAr, 
            JobTitleAr = employee.Job?.JobTitleAr ?? "N/A", // Map Job
            GradeNameAr = employee.Job?.DefaultGrade?.GradeNameAr ?? "N/A", // Map Grade
            Elements = structures.Select(s => new EmployeeStructureItemDto
            {
                StructureId = s.StructureId,
                ElementId = s.ElementId,
                ElementNameAr = s.SalaryElement.ElementNameAr,
                ElementType = s.SalaryElement.ElementType ?? "UNKNOWN",
                Amount = s.Amount,
                Percentage = s.Percentage
            }).ToList()
        };

        return Result<EmployeeSalaryStructureDto>.Success(dto);
    }
}
