using HRMS.Core.Entities.Core;
using MediatR;
using System.Collections.Generic;

namespace HRMS.Application.Features.Core.Jobs.Queries.GetAllJobs
{
    public class GetAllJobsQuery : IRequest<List<Job>> { }
}
