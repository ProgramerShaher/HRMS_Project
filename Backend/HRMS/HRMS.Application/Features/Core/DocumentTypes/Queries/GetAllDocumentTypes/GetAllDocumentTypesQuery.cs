using MediatR;
using HRMS.Application.DTOs.Core;

namespace HRMS.Application.Features.Core.DocumentTypes.Queries.GetAllDocumentTypes;

/// <summary>
/// استعلام للحصول على قائمة أنواع الوثائق مع الترقيم
/// </summary>
public class GetAllDocumentTypesQuery : IRequest<PagedResult<DocumentTypeListDto>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
    public string SortBy { get; set; } = "DocumentTypeNameAr";
    public bool SortDescending { get; set; } = false;
}
