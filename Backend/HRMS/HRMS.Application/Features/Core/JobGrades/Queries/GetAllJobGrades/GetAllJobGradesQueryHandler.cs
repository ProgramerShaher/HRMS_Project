using MediatR;
using Microsoft.EntityFrameworkCore;
using HRMS.Application.Interfaces;
using HRMS.Application.DTOs.Core;
using AutoMapper;

namespace HRMS.Application.Features.Core.JobGrades.Queries.GetAllJobGrades;

public class GetAllJobGradesQueryHandler : IRequestHandler<GetAllJobGradesQuery, PagedResult<JobGradeListDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetAllJobGradesQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PagedResult<JobGradeListDto>> Handle(GetAllJobGradesQuery request, CancellationToken cancellationToken)
    {
        var query = _context.JobGrades
            .Where(g => g.IsDeleted == 0)
            .AsQueryable();

        if (!string.IsNullOrEmpty(request.SearchTerm))
        {
            query = query.Where(g => 
                g.GradeCode.Contains(request.SearchTerm) ||
                g.GradeNameAr.Contains(request.SearchTerm) ||
                g.GradeNameEn.Contains(request.SearchTerm));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        query = request.SortBy.ToLower() switch
        {
            "gradecode" => request.SortDescending ? query.OrderByDescending(g => g.GradeCode) : query.OrderBy(g => g.GradeCode),
            "gradenamear" => request.SortDescending ? query.OrderByDescending(g => g.GradeNameAr) : query.OrderBy(g => g.GradeNameAr),
            _ => request.SortDescending ? query.OrderByDescending(g => g.GradeLevel) : query.OrderBy(g => g.GradeLevel)
        };

        var items = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(g => new JobGradeListDto
            {
                JobGradeId = g.JobGradeId,
                GradeCode = g.GradeCode,
                GradeNameAr = g.GradeNameAr,
                GradeNameEn = g.GradeNameEn,
                GradeLevel = g.GradeLevel,
                MinSalary = g.MinSalary,
                MaxSalary = g.MaxSalary
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<JobGradeListDto>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
}
