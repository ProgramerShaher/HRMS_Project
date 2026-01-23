using HRMS.Core.Entities.Core;
using MediatR;
using System.Collections.Generic;

namespace HRMS.Application.Features.Core.Branches.Queries.GetAllBranches
{
    /// <summary>
    /// استعلام جلب جميع الفروع
    /// </summary>
    public class GetAllBranchesQuery : IRequest<List<Branch>>
    {
    }
}
