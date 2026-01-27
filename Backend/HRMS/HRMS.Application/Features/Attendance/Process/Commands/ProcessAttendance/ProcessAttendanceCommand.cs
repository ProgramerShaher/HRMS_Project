using FluentValidation;
using MediatR;
using System;

namespace HRMS.Application.Features.Attendance.Process.Commands.ProcessAttendance;

// 1. Command
public record ProcessAttendanceCommand(DateTime TargetDate) : IRequest<int>;

// 2. Validator
// Validator is in a separate file
