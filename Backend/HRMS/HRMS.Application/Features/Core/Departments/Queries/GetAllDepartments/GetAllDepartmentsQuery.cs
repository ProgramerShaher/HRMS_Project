using HRMS.Core.Entities.Core;
using MediatR;
using System.Collections.Generic;

namespace HRMS.Application.Features.Core.Departments.Queries.GetAllDepartments
{
    public class GetAllDepartmentsQuery : IRequest<List<Department>> { }
}
