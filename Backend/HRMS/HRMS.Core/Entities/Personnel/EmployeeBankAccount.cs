using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HRMS.Core.Entities.Common;
using HRMS.Core.Entities.Core;

namespace HRMS.Core.Entities.Personnel
{
    /// <summary>
    /// كيان الحسابات البنكية للموظفين - يحتوي على تفاصيل الحسابات البنكية للموظف
    /// </summary>
    [Table("EMPLOYEE_BANK_ACCOUNTS", Schema = "HR_PERSONNEL")]
    public class EmployeeBankAccount : BaseEntity
    {
        /// <summary>
        /// المعرف الفريد للحساب
        /// </summary>
        [Key]
        [Column("ACCOUNT_ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AccountId { get; set; }

        /// <summary>
        /// معرف الموظف صاحب الحساب
        /// </summary>
        [Required(ErrorMessage = "الموظف مطلوب")]
        [Column("EMPLOYEE_ID")]
        [ForeignKey(nameof(Employee))]
        public int EmployeeId { get; set; }

        /// <summary>
        /// معرف البنك
        /// </summary>
        [Required(ErrorMessage = "البنك مطلوب")]
        [Column("BANK_ID")]
        [ForeignKey(nameof(Bank))]
        public int BankId { get; set; }

        /// <summary>
        /// رقم الحساب
        /// </summary>
        [Required(ErrorMessage = "رقم الحساب مطلوب")]
        [MaxLength(50)]
        [Column("ACCOUNT_NUMBER")]
        public string AccountNumber { get; set; } = string.Empty;

        /// <summary>
        /// رقم الآيبان
        /// </summary>
        [MaxLength(50)]
        [Column("IBAN")]
        public string? Iban { get; set; }

        /// <summary>
        /// هل هو الحساب الرئيسي للراتب (1=نعم، 0=لا)
        /// </summary>
        [Column("IS_PRIMARY")]
        public byte IsPrimary { get; set; } = 1;

        /// <summary>
        /// هل الحساب نشط (1=نعم، 0=لا)
        /// </summary>
        [Column("IS_ACTIVE")]
        public byte IsActive { get; set; } = 1;

        // ═══════════════════════════════════════════════════════════
        // Navigation Properties - العلاقات
        // ═══════════════════════════════════════════════════════════

        /// <summary>
        /// الموظف صاحب الحساب
        /// </summary>
        public virtual Employee Employee { get; set; } = null!;

        /// <summary>
        /// البنك
        /// </summary>
        public virtual Bank Bank { get; set; } = null!;
    }
}
