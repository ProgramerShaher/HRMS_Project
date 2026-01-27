using MediatR;

namespace HRMS.Application.Features.Core.Jobs.Commands.UpdateJob;

public class UpdateJobCommand : IRequest<int>
{
    public int JobId { get; set; }
    public string JobTitleAr { get; set; } = string.Empty;
    public string? JobTitleEn { get; set; }
    public int? DefaultGradeId { get; set; }
    public byte IsMedical { get; set; }
}
