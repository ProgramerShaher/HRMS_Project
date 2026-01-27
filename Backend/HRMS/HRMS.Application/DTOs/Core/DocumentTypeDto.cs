namespace HRMS.Application.DTOs.Core;

public class DocumentTypeDto
{
    public int DocumentTypeId { get; set; }
    public string DocumentTypeNameAr { get; set; } = string.Empty;
    public string DocumentTypeNameEn { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? AllowedExtensions { get; set; }
    public bool IsRequired { get; set; }
    public bool HasExpiry { get; set; }
    public int? DefaultExpiryDays { get; set; }
    public int? MaxFileSizeMB { get; set; }
}

public class DocumentTypeListDto
{
    public int DocumentTypeId { get; set; }
    public string DocumentTypeNameAr { get; set; } = string.Empty;
    public string DocumentTypeNameEn { get; set; } = string.Empty;
    public bool IsRequired { get; set; }
    public bool HasExpiry { get; set; }
}
