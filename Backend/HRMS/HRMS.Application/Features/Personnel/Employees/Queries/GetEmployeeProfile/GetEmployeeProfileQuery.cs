using MediatR;
using HRMS.Application.DTOs.Personnel;

namespace HRMS.Application.Features.Personnel.Employees.Queries.GetEmployeeProfile;

public class GetEmployeeProfileQuery : IRequest<EmployeeProfileDto?>
{
    public int EmployeeId { get; set; }

    public GetEmployeeProfileQuery(int employeeId)
    {
        EmployeeId = employeeId;
    }
}
