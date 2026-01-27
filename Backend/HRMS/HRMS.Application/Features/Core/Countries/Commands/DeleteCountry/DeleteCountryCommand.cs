using MediatR;

namespace HRMS.Application.Features.Core.Countries.Commands.DeleteCountry;

/// <summary>
/// أمر حذف دولة
/// </summary>
public class DeleteCountryCommand : IRequest<bool>
{
    public int CountryId { get; set; }

    public DeleteCountryCommand(int countryId)
    {
        CountryId = countryId;
    }
}
