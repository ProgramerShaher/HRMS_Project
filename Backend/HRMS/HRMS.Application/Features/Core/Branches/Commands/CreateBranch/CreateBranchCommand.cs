using MediatR;
using System;

namespace HRMS.Application.Features.Core.Branches.Commands.CreateBranch
{
    public class CreateBranchCommand : IRequest<int>
    {
        public string BranchNameAr { get; set; }
        public string BranchNameEn { get; set; }
        public int? CityId { get; set; }
        public string Address { get; set; }
    }
}
