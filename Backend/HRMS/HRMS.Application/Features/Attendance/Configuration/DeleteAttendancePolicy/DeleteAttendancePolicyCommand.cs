using HRMS.Core.Utilities;
using MediatR;

namespace HRMS.Application.Features.Attendance.Configuration.DeleteAttendancePolicy;

/// <summary>
/// Command to delete (soft delete) an attendance policy.
/// Marks the policy as deleted without removing from database.
/// </summary>
public record DeleteAttendancePolicyCommand(int PolicyId) : IRequest<Result<bool>>;
