using MediatR;

namespace HRMS.Application.Features.Core.Jobs.Commands.DeleteJob;

public class DeleteJobCommand : IRequest<bool>
{
    public int JobId { get; set; }

    public DeleteJobCommand(int jobId)
    {
        JobId = jobId;
    }
}
