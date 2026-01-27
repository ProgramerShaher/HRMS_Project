using MediatR;
using HRMS.Application.DTOs.Core;
using HRMS.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

using AutoMapper;
using AutoMapper.QueryableExtensions;

namespace HRMS.Application.Features.Core.Countries.Queries.SearchCountries;

public class SearchCountriesQueryHandler : IRequestHandler<SearchCountriesQuery, PagedResult<CountryListDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public SearchCountriesQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PagedResult<CountryListDto>> Handle(SearchCountriesQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Countries.AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.Trim().ToLower();
            query = query.Where(c => 
                c.CountryNameAr.ToLower().Contains(searchTerm) ||
                c.CountryNameEn.ToLower().Contains(searchTerm) ||
                (c.CitizenshipNameAr != null && c.CitizenshipNameAr.ToLower().Contains(searchTerm))
            );
        }

        if (!string.IsNullOrWhiteSpace(request.IsoCode))
        {
            query = query.Where(c => c.IsoCode == request.IsoCode.ToUpper());
        }

        if (request.IsActive.HasValue)
        {
            query = query.Where(c => c.IsActive == request.IsActive.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        // Sorting
        query = request.SortBy.ToLower() switch
        {
            "countrynameen" => request.SortDirection.ToLower() == "desc" 
                ? query.OrderByDescending(c => c.CountryNameEn)
                : query.OrderBy(c => c.CountryNameEn),
            "isocode" => request.SortDirection.ToLower() == "desc"
                ? query.OrderByDescending(c => c.IsoCode)
                : query.OrderBy(c => c.IsoCode),
            "createdat" => request.SortDirection.ToLower() == "desc"
                ? query.OrderByDescending(c => c.CreatedAt)
                : query.OrderBy(c => c.CreatedAt),
            _ => request.SortDirection.ToLower() == "desc"
                ? query.OrderByDescending(c => c.CountryNameAr)
                : query.OrderBy(c => c.CountryNameAr)
        };

        var items = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ProjectTo<CountryListDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        return new PagedResult<CountryListDto>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
}
