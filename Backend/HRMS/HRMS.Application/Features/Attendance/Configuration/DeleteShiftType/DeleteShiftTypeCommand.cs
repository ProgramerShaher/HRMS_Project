using HRMS.Core.Utilities;
using MediatR;

namespace HRMS.Application.Features.Attendance.Configuration.DeleteShiftType;

/// <summary>
/// Command to delete (soft delete) a shift type.
/// Marks shift as deleted without removing from database.
/// </summary>
public record DeleteShiftTypeCommand(int ShiftId) : IRequest<Result<bool>>;
