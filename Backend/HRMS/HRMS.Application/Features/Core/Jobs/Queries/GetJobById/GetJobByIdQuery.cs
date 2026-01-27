using MediatR;
using HRMS.Application.DTOs.Core;

namespace HRMS.Application.Features.Core.Jobs.Queries.GetJobById;

public class GetJobByIdQuery : IRequest<JobDto?>
{
    public int JobId { get; set; }

    public GetJobByIdQuery(int jobId)
    {
        JobId = jobId;
    }
}
