using HRMS.Application.DTOs.Payroll.Configuration;
using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Payroll.Configuration.Structure.Queries.GetAllStructures;

/// <summary>
/// الحصول على جدول شامل بهياكل رواتب جميع الموظفين
/// Get comprehensive table of all employees' salary structures
/// </summary>
public class GetAllStructuresQuery : IRequest<Result<List<EmployeeStructureSummaryDto>>>
{
    public int? DepartmentId { get; set; }
    public string? SearchTerm { get; set; }
}

public class GetAllStructuresQueryHandler : IRequestHandler<GetAllStructuresQuery, Result<List<EmployeeStructureSummaryDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetAllStructuresQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<EmployeeStructureSummaryDto>>> Handle(GetAllStructuresQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // استعلام أساسي للموظفين مع هياكلهم
            // Base query for employees with their structures
            var query = _context.Employees
                .Include(e => e.Department)
                .Include(e => e.Job)
                .Include(e => e.Compensation)
                .Where(e => e.IsDeleted == 0)
                .AsQueryable();

            // تطبيق الفلاتر
            // Apply filters
            if (request.DepartmentId.HasValue)
            {
                query = query.Where(e => e.DepartmentId == request.DepartmentId.Value);
            }

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var searchLower = request.SearchTerm.ToLower();
                query = query.Where(e =>
                    e.FullNameAr.ToLower().Contains(searchLower) ||
                    e.EmployeeNumber.ToLower().Contains(searchLower));
            }

            // تنفيذ الاستعلام
            // Execute query
            var employees = await query.ToListAsync(cancellationToken);

            // تحويل إلى DTO
            // Map to DTO
            var result = employees.Select(e =>
            {
                var basicSalary = e.Compensation?.BasicSalary ?? 0;
                
                return new EmployeeStructureSummaryDto
                {
                    EmployeeId = e.EmployeeId,
                    EmployeeCode = e.EmployeeNumber,
                    EmployeeNameAr = e.FullNameAr,
                    DepartmentName = e.Department?.DeptNameAr,
                    JobTitle = e.Job?.JobTitleAr,
                    BasicSalary = basicSalary,
                    AllowancesCount = 0,
                    TotalAllowances = 0,
                    DeductionsCount = 0,
                    TotalDeductions = 0,
                    GrossSalary = basicSalary,
                    NetSalary = basicSalary,
                    HasStructure = e.Compensation != null,
                    LastUpdated = e.Compensation?.UpdatedAt ?? e.Compensation?.CreatedAt
                };
            }).ToList();

            return Result<List<EmployeeStructureSummaryDto>>.Success(result);
        }
        catch (Exception ex)
        {
            return Result<List<EmployeeStructureSummaryDto>>.Failure($"Error retrieving structures: {ex.Message}");
        }
    }
}
