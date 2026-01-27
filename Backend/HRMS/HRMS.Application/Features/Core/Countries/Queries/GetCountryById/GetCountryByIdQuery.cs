using MediatR;
using HRMS.Application.DTOs.Core;

namespace HRMS.Application.Features.Core.Countries.Queries.GetCountryById;

/// <summary>
/// استعلام للحصول على دولة بمعرفها
/// </summary>
public class GetCountryByIdQuery : IRequest<CountryDto?>
{
    public int CountryId { get; set; }

    public GetCountryByIdQuery(int id)
    {
        CountryId = id;
    }
}
