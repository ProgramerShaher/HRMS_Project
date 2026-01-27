using MediatR;

namespace HRMS.Application.Features.Core.Jobs.Commands.CreateJob;

public class CreateJobCommand : IRequest<int>
{
    public string JobTitleAr { get; set; } = string.Empty;
    public string? JobTitleEn { get; set; }
    public int? DefaultGradeId { get; set; }
    public byte IsMedical { get; set; }
}
