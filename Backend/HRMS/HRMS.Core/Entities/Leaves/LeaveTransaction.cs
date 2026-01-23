using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HRMS.Core.Entities.Common;
using HRMS.Core.Entities.Personnel;

namespace HRMS.Core.Entities.Leaves
{
    /// <summary>
    /// كيان حركات الإجازات - سجل تفصيلي لجميع الحركات التي تؤثر على الرصيد
    /// </summary>
    [Table("LEAVE_TRANSACTIONS", Schema = "HR_LEAVES")]
    public class LeaveTransaction : BaseEntity
    {
        /// <summary>
        /// المعرف الفريد للحركة
        /// </summary>
        [Key]
        [Column("TRANSACTION_ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long TransactionId { get; set; }

        /// <summary>
        /// معرف الموظف
        /// </summary>
        [Required(ErrorMessage = "الموظف مطلوب")]
        [Column("EMPLOYEE_ID")]
        [ForeignKey(nameof(Employee))]
        public int EmployeeId { get; set; }

        /// <summary>
        /// معرف نوع الإجازة
        /// </summary>
        [Required(ErrorMessage = "نوع الإجازة مطلوب")]
        [Column("LEAVE_TYPE_ID")]
        [ForeignKey(nameof(LeaveType))]
        public int LeaveTypeId { get; set; }

        /// <summary>
        /// نوع الحركة (ACCRUAL, DEDUCTION, ADJUSTMENT, CARRY_FORWARD)
        /// </summary>
        [Required(ErrorMessage = "نوع الحركة مطلوب")]
        [MaxLength(20)]
        [Column("TRANSACTION_TYPE")]
        public string TransactionType { get; set; } = string.Empty;

        /// <summary>
        /// عدد الأيام (موجب للإضافة، سالب للخصم)
        /// </summary>
        [Required(ErrorMessage = "عدد الأيام مطلوب")]
        [Column("DAYS", TypeName = "decimal(5, 2)")]
        public decimal Days { get; set; }

        /// <summary>
        /// تاريخ الحركة
        /// </summary>
        [Column("TRANSACTION_DATE")]
        public DateTime TransactionDate { get; set; } = DateTime.Now;

        /// <summary>
        /// معرف العملية المرجعية (رقم طلب الإجازة مثلاً)
        /// </summary>
        [Column("REFERENCE_ID")]
        public int? ReferenceId { get; set; }

        /// <summary>
        /// ملاحظات
        /// </summary>
        [MaxLength(200)]
        [Column("NOTES")]
        public string? Notes { get; set; }

        // ═══════════════════════════════════════════════════════════
        // Navigation Properties - العلاقات
        // ═══════════════════════════════════════════════════════════

        /// <summary>
        /// الموظف
        /// </summary>
        public virtual Employee Employee { get; set; } = null!;

        /// <summary>
        /// نوع الإجازة
        /// </summary>
        public virtual LeaveType LeaveType { get; set; } = null!;
    }
}
