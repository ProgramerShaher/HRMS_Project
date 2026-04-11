using HRMS.Core.Utilities;
using MediatR;

namespace HRMS.Application.Features.Attendance.Punch.Queries.GetDeviceEmployees;

public class GetDeviceEmployeesQuery : IRequest<Result<List<DeviceEmployeeDto>>>
{
}
