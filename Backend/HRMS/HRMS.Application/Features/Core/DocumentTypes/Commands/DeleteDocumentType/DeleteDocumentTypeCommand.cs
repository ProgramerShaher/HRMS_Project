using MediatR;

namespace HRMS.Application.Features.Core.DocumentTypes.Commands.DeleteDocumentType;

/// <summary>
/// أمر حذف نوع الوثيقة
/// </summary>
public class DeleteDocumentTypeCommand : IRequest<bool>
{
    public int DocumentTypeId { get; set; }

    public DeleteDocumentTypeCommand(int documentTypeId)
    {
        DocumentTypeId = documentTypeId;
    }
}
