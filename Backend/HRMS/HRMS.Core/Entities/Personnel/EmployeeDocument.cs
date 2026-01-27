using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HRMS.Core.Entities.Common;
using HRMS.Core.Entities.Core; // For DocumentType

namespace HRMS.Core.Entities.Personnel;

/// <summary>
/// وثائق الموظف (تراخيص، شهادات، عقود)
/// </summary>
[Table("EMPLOYEE_DOCUMENTS", Schema = "HR_PERSONNEL")]
public class EmployeeDocument : BaseEntity
{
    [Key]
    [Column("DOCUMENT_ID")]
    public int DocumentId { get; set; }

    [Column("EMPLOYEE_ID")]
    public int EmployeeId { get; set; }

    [Column("DOCUMENT_TYPE_ID")]
    public int DocumentTypeId { get; set; }

    [Column("DOCUMENT_NUMBER")]
    [MaxLength(50)]
    public string? DocumentNumber { get; set; }

    [Column("ISSUE_DATE")]
    public DateTime? IssueDate { get; set; }

    [Column("EXPIRY_DATE")]
    public DateTime? ExpiryDate { get; set; }

    // --- File Info (Best Practice) ---
    [Column("FILE_NAME")] // الاسم الأصلي
    [MaxLength(255)]
    public required string FileName { get; set; }

    [Column("FILE_PATH")] // المسار المخزن (Relative)
    [MaxLength(500)]
    public required string FilePath { get; set; }

    [Column("CONTENT_TYPE")] // e.g. application/pdf
    [MaxLength(100)]
    public string? ContentType { get; set; }

    [Column("FILE_SIZE")]
    public long FileSize { get; set; }

    // Navigation
    [ForeignKey(nameof(EmployeeId))]
    public virtual Employee Employee { get; set; } = null!;

    [ForeignKey(nameof(DocumentTypeId))]
    public virtual DocumentType DocumentType { get; set; } = null!;
}
