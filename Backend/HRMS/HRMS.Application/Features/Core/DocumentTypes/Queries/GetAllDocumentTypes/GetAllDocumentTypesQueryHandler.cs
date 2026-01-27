using MediatR;
using Microsoft.EntityFrameworkCore;
using HRMS.Application.Interfaces;
using HRMS.Application.DTOs.Core;

namespace HRMS.Application.Features.Core.DocumentTypes.Queries.GetAllDocumentTypes;

/// <summary>
/// معالج استعلام الحصول على قائمة أنواع الوثائق
/// </summary>
public class GetAllDocumentTypesQueryHandler : IRequestHandler<GetAllDocumentTypesQuery, PagedResult<DocumentTypeListDto>>
{
    private readonly IApplicationDbContext _context;

    public GetAllDocumentTypesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<DocumentTypeListDto>> Handle(GetAllDocumentTypesQuery request, CancellationToken cancellationToken)
    {
        var query = _context.DocumentTypes
            .Where(d => d.IsDeleted == 0)
            .AsQueryable();

        // Search
        if (!string.IsNullOrEmpty(request.SearchTerm))
        {
            query = query.Where(d => 
                d.DocumentTypeNameAr.Contains(request.SearchTerm) ||
                d.DocumentTypeNameEn.Contains(request.SearchTerm));
        }

        // Total count
        var totalCount = await query.CountAsync(cancellationToken);

        // Sorting
        query = request.SortBy.ToLower() switch
        {
            "documenttypenamearen" => request.SortDescending 
                ? query.OrderByDescending(d => d.DocumentTypeNameEn)
                : query.OrderBy(d => d.DocumentTypeNameEn),
            _ => request.SortDescending
                ? query.OrderByDescending(d => d.DocumentTypeNameAr)
                : query.OrderBy(d => d.DocumentTypeNameAr)
        };

        // Pagination
        var items = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(d => new DocumentTypeListDto
            {
                DocumentTypeId = d.DocumentTypeId,
                DocumentTypeNameAr = d.DocumentTypeNameAr,
                DocumentTypeNameEn = d.DocumentTypeNameEn,
                IsRequired = d.IsRequired,
                HasExpiry = d.HasExpiry
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<DocumentTypeListDto>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
}
