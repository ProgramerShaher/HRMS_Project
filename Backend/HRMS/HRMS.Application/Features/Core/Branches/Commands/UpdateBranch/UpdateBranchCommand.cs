using MediatR;

namespace HRMS.Application.Features.Core.Branches.Commands.UpdateBranch
{
    public class UpdateBranchCommand : IRequest<bool>
    {
        public int BranchId { get; set; }
        public string BranchNameAr { get; set; }
        public string BranchNameEn { get; set; }
        public int? CityId { get; set; }
        public string Address { get; set; }
    }
}
