using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HRMS.Core.Entities.Common;

namespace HRMS.Core.Entities.Accounting;

/// <summary>
/// كيان الحساب - دليل الحسابات
/// Chart of Accounts Entity
/// </summary>
[Table("ACCOUNTS", Schema = "ACCOUNTING")]
public class Account : BaseEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int AccountId { get; set; }

    /// <summary>
    /// رمز الحساب - Account Code (e.g., "5100", "2100")
    /// </summary>
    [Required]
    [MaxLength(20)]
    public string AccountCode { get; set; } = string.Empty;

    /// <summary>
    /// اسم الحساب بالعربية
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string AccountNameAr { get; set; } = string.Empty;

    /// <summary>
    /// اسم الحساب بالإنجليزية
    /// </summary>
    [MaxLength(200)]
    public string? AccountNameEn { get; set; }

    /// <summary>
    /// نوع الحساب: ASSET, LIABILITY, EQUITY, REVENUE, EXPENSE
    /// </summary>
    [Required]
    [MaxLength(20)]
    public string AccountType { get; set; } = string.Empty;

    /// <summary>
    /// الحساب الأب (للحسابات الفرعية)
    /// </summary>
    [Column("PARENT_ACCOUNT_ID")]
    public int? ParentAccountId { get; set; }

    /// <summary>
    /// هل الحساب نشط؟
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// مستوى الحساب في الشجرة (1 = رئيسي، 2 = فرعي، إلخ)
    /// </summary>
    public byte Level { get; set; } = 1;

    // Navigation Properties
    [ForeignKey(nameof(ParentAccountId))]
    public virtual Account? ParentAccount { get; set; }

    public virtual ICollection<Account> SubAccounts { get; set; } = new List<Account>();
    public virtual ICollection<JournalEntryLine> JournalEntryLines { get; set; } = new List<JournalEntryLine>();
}
