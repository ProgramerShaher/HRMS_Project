using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HRMS.Core.Entities.Common;

namespace HRMS.Core.Entities.Leaves
{
    /// <summary>
    /// كيان قواعد استحقاق الإجازات - يحدد كيفية اكتساب رصيد الإجازة
    /// </summary>
    [Table("LEAVE_ACCRUAL_RULES", Schema = "HR_LEAVES")]
    public class LeaveAccrualRule : BaseEntity
    {
        /// <summary>
        /// المعرف الفريد للقاعدة
        /// </summary>
        [Key]
        [Column("RULE_ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RuleId { get; set; }

        /// <summary>
        /// معرف نوع الإجازة
        /// </summary>
        [Required(ErrorMessage = "نوع الإجازة مطلوب")]
        [Column("LEAVE_TYPE_ID")]
        [ForeignKey(nameof(LeaveType))]
        public int LeaveTypeId { get; set; }

        /// <summary>
        /// دورة الاستحقاق (MONTHLY, YEARLY, JOINING_DATE)
        /// </summary>
        [MaxLength(20)]
        [Column("ACCRUAL_FREQUENCY")]
        public string? AccrualFrequency { get; set; }

        /// <summary>
        /// عدد الأيام المستحقة في الفترة
        /// </summary>
        [Required(ErrorMessage = "عدد الأيام مطلوب")]
        [Column("DAYS_PER_PERIOD", TypeName = "decimal(5, 2)")]
        public decimal DaysPerPeriod { get; set; }

        /// <summary>
        /// الحد الأقصى للتراكم
        /// </summary>
        [Column("MAX_ACCUMULATION")]
        public short? MaxAccumulation { get; set; }

        /// <summary>
        /// هل يحسب بالتناسب (1=نعم، 0=لا)
        /// </summary>
        [Column("IS_PRORATED")]
        public byte IsProrated { get; set; } = 1;

        // ═══════════════════════════════════════════════════════════
        // Navigation Properties - العلاقات
        // ═══════════════════════════════════════════════════════════

        /// <summary>
        /// نوع الإجازة
        /// </summary>
        public virtual LeaveType LeaveType { get; set; } = null!;
    }
}
