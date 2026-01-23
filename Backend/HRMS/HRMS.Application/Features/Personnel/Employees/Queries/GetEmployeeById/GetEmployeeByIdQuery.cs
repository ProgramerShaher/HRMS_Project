using HRMS.Application.Features.Personnel.Employees.DTOs;
using MediatR;

namespace HRMS.Application.Features.Personnel.Employees.Queries.GetEmployeeById
{
    public class GetEmployeeByIdQuery : IRequest<EmployeeDto>
    {
        public int Id { get; set; }

        public GetEmployeeByIdQuery(int id)
        {
            Id = id;
        }
    }
}
