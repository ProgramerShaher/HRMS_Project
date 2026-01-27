using MediatR;
using HRMS.Application.DTOs.Core;

namespace HRMS.Application.Features.Core.JobGrades.Queries.GetAllJobGrades;

public class GetAllJobGradesQuery : IRequest<PagedResult<JobGradeListDto>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
    public string SortBy { get; set; } = "GradeLevel";
    public bool SortDescending { get; set; } = false;
}
