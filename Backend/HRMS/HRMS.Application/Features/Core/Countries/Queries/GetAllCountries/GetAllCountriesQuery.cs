using MediatR;
using HRMS.Application.DTOs.Core;
using HRMS.Application.DTOs;

namespace HRMS.Application.Features.Core.Countries.Queries.GetAllCountries;

/// <summary>
/// استعلام للحصول على قائمة الدول مع الترقيم
/// </summary>
public class GetAllCountriesQuery : IRequest<PagedResult<CountryListDto>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string SortBy { get; set; } = "CountryNameAr";
    public string SortDirection { get; set; } = "asc";
    public bool? IsActive { get; set; }
}
