using MediatR;
using HRMS.Application.DTOs.Core;
using HRMS.Application.DTOs;

namespace HRMS.Application.Features.Core.Countries.Queries.SearchCountries;

/// <summary>
/// استعلام البحث المتقدم في الدول
/// </summary>
public class SearchCountriesQuery : IRequest<PagedResult<CountryListDto>>
{
    public string? SearchTerm { get; set; }
    public string? IsoCode { get; set; }
    public bool? IsActive { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string SortBy { get; set; } = "CountryNameAr";
    public string SortDirection { get; set; } = "asc";
}
