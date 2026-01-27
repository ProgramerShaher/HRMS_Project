using MediatR;

namespace HRMS.Application.Features.Core.DocumentTypes.Commands.CreateDocumentType;

/// <summary>
/// أمر إنشاء نوع وثيقة جديد
/// </summary>
public class CreateDocumentTypeCommand : IRequest<int>
{
    public string DocumentTypeNameAr { get; set; } = string.Empty;
    public string DocumentTypeNameEn { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? AllowedExtensions { get; set; }
    public bool IsRequired { get; set; }
    public bool HasExpiry { get; set; }
    public int? DefaultExpiryDays { get; set; }
    public int? MaxFileSizeMB { get; set; }
}
