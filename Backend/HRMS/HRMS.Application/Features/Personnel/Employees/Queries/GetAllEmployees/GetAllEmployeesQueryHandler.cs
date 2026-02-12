using AutoMapper;
using HRMS.Application.DTOs.Core;
using HRMS.Application.DTOs.Personnel;
using HRMS.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using HRMS.Core.Entities.Attendance;

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

        // 3. Fetch Attendance Data (Raw Punches) for the fetched employees
        // Using RawPunchLogs to ensure immediate "Live" status update without waiting for processing
        var employeeIds = items.Select(e => e.EmployeeId).ToList();
        var today = DateTime.Today;
        var tomorrow = today.AddDays(1);

        var rawPunches = await _context.RawPunchLogs
            .AsNoTracking()
            .Where(p => employeeIds.Contains(p.EmployeeId) 
                        && p.PunchTime >= today 
                        && p.PunchTime < tomorrow)
            .Select(p => new { p.EmployeeId, p.PunchType, p.PunchTime })
            .ToListAsync(cancellationToken);

        // Group by Employee and find latest punches
        // Logic: Find latest IN and latest OUT for today
        // Note: This is a simplification for the "Device" view. 
        // For accurate shift logic, we'd need complex pairing, but for "Last Action", this works.
        var employeePunchStatus = rawPunches
            .GroupBy(p => p.EmployeeId)
            .ToDictionary(g => g.Key, g => new {
                LastIn = g.Where(p => p.PunchType == "IN").OrderByDescending(p => p.PunchTime).FirstOrDefault()?.PunchTime,
                LastOut = g.Where(p => p.PunchType == "OUT").OrderByDescending(p => p.PunchTime).FirstOrDefault()?.PunchTime,
                LatestAction = g.OrderByDescending(p => p.PunchTime).FirstOrDefault()
            });

        // In-Memory Mapping
        var dtos = items.Select(e => {
            var status = employeePunchStatus.ContainsKey(e.EmployeeId) ? employeePunchStatus[e.EmployeeId] : null;
            
            // Determine effective LastPunchIn/Out based on sequence
            // If LatestAction is IN, then we are IN. If OUT, we are OUT.
            // But we want to show BOTH times if they exist today.
            
            return new EmployeeListDto
            {
                EmployeeId = e.EmployeeId,
                EmployeeNumber = e.EmployeeNumber,
                FullNameAr = e.FullNameAr, 
                FullNameEn = e.FullNameEn,
                DepartmentName = e.Department?.DeptNameAr ?? "",
                JobTitle = e.Job?.JobTitleAr ?? "", 
                Mobile = e.Mobile,
                HireDate = e.HireDate,
                IsActive = e.IsDeleted == 0,
                LastPunchIn = status?.LastIn,
                LastPunchOut = status?.LastOut
            };
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
