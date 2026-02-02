using MediatR;
using HRMS.Core.Utilities;

namespace HRMS.Application.Features.Attendance.Commands.ManualCorrection;

public record ManualCorrectionCommand(
    long DailyAttendanceId,
    string CorrectionType, // "InTime", "OutTime", "Status"
    string NewValue,
    string AuditNote
) : IRequest<Result<bool>>;
