using MediatR;
using Microsoft.EntityFrameworkCore;
using HRMS.Application.Interfaces;
using HRMS.Application.DTOs.Core;
using AutoMapper;
using AutoMapper.QueryableExtensions;

namespace HRMS.Application.Features.Core.Jobs.Queries.GetAllJobs;

public class GetAllJobsQueryHandler : IRequestHandler<GetAllJobsQuery, PagedResult<JobDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetAllJobsQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PagedResult<JobDto>> Handle(GetAllJobsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Jobs
            .Where(j => j.IsDeleted == 0)
            .AsQueryable();

        if (!string.IsNullOrEmpty(request.SearchTerm))
        {
            query = query.Where(j => 
                j.JobTitleAr.Contains(request.SearchTerm) || 
                (j.JobTitleEn != null && j.JobTitleEn.Contains(request.SearchTerm)));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(j => j.JobTitleAr)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ProjectTo<JobDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        return new PagedResult<JobDto>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
}
