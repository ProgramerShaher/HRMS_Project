using MediatR;
using HRMS.Application.DTOs.Core;

namespace HRMS.Application.Features.Core.DocumentTypes.Queries.GetDocumentTypeById;

/// <summary>
/// استعلام للحصول على نوع وثيقة بمعرفه
/// </summary>
public class GetDocumentTypeByIdQuery : IRequest<DocumentTypeDto?>
{
    public int DocumentTypeId { get; set; }

    public GetDocumentTypeByIdQuery(int documentTypeId)
    {
        DocumentTypeId = documentTypeId;
    }
}
