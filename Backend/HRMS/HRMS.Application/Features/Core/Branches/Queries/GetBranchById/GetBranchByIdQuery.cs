using HRMS.Core.Entities.Core;
using MediatR;

namespace HRMS.Application.Features.Core.Branches.Queries.GetBranchById
{
    public class GetBranchByIdQuery : IRequest<Branch>
    {
        public int Id { get; set; }

        public GetBranchByIdQuery(int id)
        {
            Id = id;
        }
    }
}
