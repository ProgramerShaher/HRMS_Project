using MediatR;
using Microsoft.EntityFrameworkCore;
using HRMS.Application.Interfaces;
using HRMS.Application.DTOs.Core;
using AutoMapper;
using AutoMapper.QueryableExtensions;

namespace HRMS.Application.Features.Core.Departments.Queries.GetAllDepartments;

/// <summary>
/// معالج استعلام الحصول على قائمة الأقسام
/// </summary>
public class GetAllDepartmentsQueryHandler : IRequestHandler<GetAllDepartmentsQuery, PagedResult<DepartmentDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetAllDepartmentsQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PagedResult<DepartmentDto>> Handle(GetAllDepartmentsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Departments
            .Where(d => d.IsDeleted == 0)
            .AsQueryable();

        if (!string.IsNullOrEmpty(request.SearchTerm))
        {
            query = query.Where(d => 
                d.DeptNameAr.Contains(request.SearchTerm) || 
                (d.DeptNameEn != null && d.DeptNameEn.Contains(request.SearchTerm)));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(d => d.DeptNameAr)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ProjectTo<DepartmentDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        return new PagedResult<DepartmentDto>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
}
