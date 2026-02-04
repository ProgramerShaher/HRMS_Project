using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HRMS.Core.Entities.Common;

namespace HRMS.Core.Entities.Accounting;

/// <summary>
/// كيان سطر قيد اليومية
/// Journal Entry Line Item Entity
/// </summary>
[Table("JOURNAL_ENTRY_LINES", Schema = "ACCOUNTING")]
public class JournalEntryLine : BaseEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long LineId { get; set; }

    /// <summary>
    /// معرف القيد الرئيسي
    /// </summary>
    [Required]
    [Column("JOURNAL_ENTRY_ID")]
    [ForeignKey(nameof(JournalEntry))]
    public long JournalEntryId { get; set; }

    /// <summary>
    /// معرف الحساب
    /// </summary>
    [Required]
    [Column("ACCOUNT_ID")]
    [ForeignKey(nameof(Account))]
    public int AccountId { get; set; }

    /// <summary>
    /// المبلغ المدين
    /// </summary>
    [Column(TypeName = "decimal(18, 2)")]
    public decimal DebitAmount { get; set; }

    /// <summary>
    /// المبلغ الدائن
    /// </summary>
    [Column(TypeName = "decimal(18, 2)")]
    public decimal CreditAmount { get; set; }

    /// <summary>
    /// وصف السطر
    /// </summary>
    [MaxLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// رقم السطر (للترتيب)
    /// </summary>
    public short LineNumber { get; set; }

    // Navigation Properties
    public virtual JournalEntry JournalEntry { get; set; } = null!;
    public virtual Account Account { get; set; } = null!;

    /// <summary>
    /// التحقق من صحة السطر (إما مدين أو دائن، وليس كلاهما)
    /// </summary>
    public bool IsValid()
    {
        // يجب أن يكون أحدهما فقط أكبر من صفر
        return (DebitAmount > 0 && CreditAmount == 0) || (CreditAmount > 0 && DebitAmount == 0);
    }
}
