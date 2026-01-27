using System;

namespace HRMS.Application.DTOs.Personnel;

public class EmployeeDocumentDto
{
    public int DocumentId { get; set; }
    public string DocumentTypeName { get; set; } = string.Empty;
    public string? DocumentNumber { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public int DocumentTypeId { get; set; } // Added for Create/Update scenarios
}
