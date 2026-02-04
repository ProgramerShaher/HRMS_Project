using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HRMS.Core.Entities.Common;

namespace HRMS.Core.Entities.Accounting;

/// <summary>
/// كيان قيد اليومية - رأس القيد
/// Journal Entry Header Entity
/// </summary>
[Table("JOURNAL_ENTRIES", Schema = "ACCOUNTING")]
public class JournalEntry : BaseEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long JournalEntryId { get; set; }

    /// <summary>
    /// تاريخ القيد
    /// </summary>
    [Required]
    public DateTime EntryDate { get; set; } = DateTime.Now;

    /// <summary>
    /// وصف القيد
    /// </summary>
    [Required]
    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// المصدر: PAYROLL, MANUAL, INVOICE, etc.
    /// </summary>
    [MaxLength(50)]
    public string? SourceModule { get; set; }

    /// <summary>
    /// معرف المصدر (مثلاً RunId من الرواتب)
    /// </summary>
    public int? SourceReferenceId { get; set; }

    /// <summary>
    /// حالة القيد: DRAFT, POSTED, REVERSED
    /// </summary>
    [Required]
    [MaxLength(20)]
    public string Status { get; set; } = "DRAFT";

    /// <summary>
    /// إجمالي المدين
    /// </summary>
    [Column(TypeName = "decimal(18, 2)")]
    public decimal TotalDebit { get; set; }

    /// <summary>
    /// إجمالي الدائن
    /// </summary>
    [Column(TypeName = "decimal(18, 2)")]
    public decimal TotalCredit { get; set; }

    /// <summary>
    /// تاريخ الترحيل
    /// </summary>
    public DateTime? PostedDate { get; set; }

    /// <summary>
    /// ملاحظات إضافية
    /// </summary>
    [MaxLength(1000)]
    public string? Notes { get; set; }

    // Navigation Properties
    public virtual ICollection<JournalEntryLine> Lines { get; set; } = new List<JournalEntryLine>();

    /// <summary>
    /// التحقق من توازن القيد (المدين = الدائن)
    /// </summary>
    public bool IsBalanced() => TotalDebit == TotalCredit;
}
