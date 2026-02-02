using HRMS.Application.DTOs.Attendance;
using HRMS.Core.Utilities;
using MediatR;

namespace HRMS.Application.Features.Attendance.Configuration.GetAttendancePolicies;

/// <summary>
/// Query to retrieve all attendance policies.
/// Returns list of active policies with their configuration.
/// </summary>
public record GetAttendancePoliciesQuery : IRequest<Result<List<AttendancePolicyDto>>>;
