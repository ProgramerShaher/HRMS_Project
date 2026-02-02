using HRMS.Application.DTOs.Attendance;
using HRMS.Core.Utilities;
using MediatR;

namespace HRMS.Application.Features.Attendance.Configuration.GetShiftTypes;

/// <summary>
/// Query to retrieve all shift types.
/// Returns list of active shifts with their timing configuration.
/// </summary>
public record GetShiftTypesQuery : IRequest<Result<List<ShiftTypeDto>>>;
