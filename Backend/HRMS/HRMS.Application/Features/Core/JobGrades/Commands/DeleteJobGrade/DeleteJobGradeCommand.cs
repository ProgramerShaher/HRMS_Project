using MediatR;

namespace HRMS.Application.Features.Core.JobGrades.Commands.DeleteJobGrade;

public class DeleteJobGradeCommand : IRequest<bool>
{
    public int JobGradeId { get; set; }

    public DeleteJobGradeCommand(int jobGradeId)
    {
        JobGradeId = jobGradeId;
    }
}
