using MediatR;
using HRMS.Application.DTOs.Core;

namespace HRMS.Application.Features.Core.JobGrades.Queries.GetJobGradeById;

public class GetJobGradeByIdQuery : IRequest<JobGradeDto?>
{
    public int JobGradeId { get; set; }

    public GetJobGradeByIdQuery(int jobGradeId)
    {
        JobGradeId = jobGradeId;
    }
}
