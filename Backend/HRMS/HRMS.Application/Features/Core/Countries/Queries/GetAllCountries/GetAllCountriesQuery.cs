using HRMS.Core.Entities.Core;
using MediatR;
using System.Collections.Generic;

namespace HRMS.Application.Features.Core.Countries.Queries.GetAllCountries
{
    public class GetAllCountriesQuery : IRequest<List<Country>>
    {
    }
}
