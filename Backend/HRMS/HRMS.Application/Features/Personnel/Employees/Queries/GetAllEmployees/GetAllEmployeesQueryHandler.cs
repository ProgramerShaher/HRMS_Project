using AutoMapper;
using HRMS.Application.DTOs.Core;
using HRMS.Application.DTOs.Personnel;
using HRMS.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Personnel.Employees.Queries.GetAllEmployees;

public class GetAllEmployeesQueryHandler : IRequestHandler<GetAllEmployeesQuery, PagedResult<EmployeeListDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetAllEmployeesQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PagedResult<EmployeeListDto>> Handle(GetAllEmployeesQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Employees
            .Include(e => e.Department)
            .Include(e => e.Job)
            .AsNoTracking() // Performance
            .AsQueryable();

        // 1. Filtering (Search Engine style)
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var term = request.SearchTerm.Trim().ToLower();
            query = query.Where(e => 
                e.FirstNameAr.Contains(term) || 
                e.LastNameAr.Contains(term) ||
                e.FullNameEn.ToLower().Contains(term) ||
                e.EmployeeNumber.Contains(term) ||
                e.Mobile.Contains(term));
        }

        if (request.DepartmentId.HasValue)
            query = query.Where(e => e.DepartmentId == request.DepartmentId);

        if (request.JobId.HasValue)
            query = query.Where(e => e.JobId == request.JobId);

        if (request.IsActive.HasValue)
            query = query.Where(e => e.IsDeleted == (request.IsActive.Value ? 0 : 1)); // Assuming IsDeleted logic
        else
            query = query.Where(e => e.IsDeleted == 0); // Default active only

        // 2. Pagination
        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(e => e.CreatedAt)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        // In-Memory Mapping to use computed FullNameAr
        var dtos = items.Select(e => new EmployeeListDto
        {
            EmployeeId = e.EmployeeId,
            EmployeeNumber = e.EmployeeNumber,
            FullNameAr = e.FullNameAr, // Now works because 'e' is in memory
            FullNameEn = e.FullNameEn,
            DepartmentName = e.Department?.DeptNameAr ?? "",
            JobTitle = e.Job?.JobTitleAr ?? "", // Safety check
            Mobile = e.Mobile,
            HireDate = e.HireDate,
            IsActive = e.IsDeleted == 0
        }).ToList();

        return new PagedResult<EmployeeListDto>
        {
            Items = dtos,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
}
