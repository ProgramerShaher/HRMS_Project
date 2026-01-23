using HRMS.Core.Entities.Core;
using MediatR;
using System.Collections.Generic;

namespace HRMS.Application.Features.Core.Cities.Queries.GetAllCities
{
    public class GetAllCitiesQuery : IRequest<List<City>> { }
}
