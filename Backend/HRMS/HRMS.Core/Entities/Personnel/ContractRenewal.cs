using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HRMS.Core.Entities.Common;

namespace HRMS.Core.Entities.Personnel
{
    /// <summary>
    /// كيان تجديدات العقود - يحتوي على سجل تجديدات العقود للموظفين
    /// </summary>
    [Table("CONTRACT_RENEWALS", Schema = "HR_PERSONNEL")]
    public class ContractRenewal : BaseEntity
    {
        /// <summary>
        /// المعرف الفريد للتجديد
        /// </summary>
        [Key]
        [Column("RENEWAL_ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RenewalId { get; set; }

        /// <summary>
        /// معرف العقد الذي تم تجديده
        /// </summary>
        [Required(ErrorMessage = "العقد مطلوب")]
        [Column("CONTRACT_ID")]
        [ForeignKey(nameof(Contract))]
        public int ContractId { get; set; }

        /// <summary>
        /// تاريخ انتهاء العقد السابق
        /// </summary>
        [Required(ErrorMessage = "تاريخ الانتهاء السابق مطلوب")]
        [Column("OLD_END_DATE")]
        public DateTime OldEndDate { get; set; }

        /// <summary>
        /// تاريخ بداية التجديد
        /// </summary>
        [Required(ErrorMessage = "تاريخ بداية التجديد مطلوب")]
        [Column("NEW_START_DATE")]
        public DateTime NewStartDate { get; set; }

        /// <summary>
        /// تاريخ نهاية التجديد
        /// </summary>
        [Required(ErrorMessage = "تاريخ نهاية التجديد مطلوب")]
        [Column("NEW_END_DATE")]
        public DateTime NewEndDate { get; set; }

        /// <summary>
        /// تاريخ إجراء التجديد
        /// </summary>
        [Column("RENEWAL_DATE")]
        public DateTime RenewalDate { get; set; } = DateTime.Now;

        /// <summary>
        /// ملاحظات على التجديد
        /// </summary>
        [MaxLength(500)]
        [Column("NOTES")]
        public string? Notes { get; set; }

        // ═══════════════════════════════════════════════════════════
        // Navigation Properties - العلاقات
        // ═══════════════════════════════════════════════════════════

        /// <summary>
        /// العقد المرتبط
        /// </summary>
        public virtual Contract Contract { get; set; } = null!;
    }
}
