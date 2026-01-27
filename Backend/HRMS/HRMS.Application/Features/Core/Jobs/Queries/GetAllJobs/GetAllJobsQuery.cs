using MediatR;
using HRMS.Application.DTOs.Core;

namespace HRMS.Application.Features.Core.Jobs.Queries.GetAllJobs;

public class GetAllJobsQuery : IRequest<PagedResult<JobDto>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
}
