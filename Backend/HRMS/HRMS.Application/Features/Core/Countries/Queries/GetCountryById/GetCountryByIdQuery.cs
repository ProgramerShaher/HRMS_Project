using HRMS.Core.Entities.Core;
using MediatR;

namespace HRMS.Application.Features.Core.Countries.Queries.GetCountryById
{
    public class GetCountryByIdQuery : IRequest<Country>
    {
        public int Id { get; set; }

        public GetCountryByIdQuery(int id)
        {
            Id = id;
        }
    }
}
