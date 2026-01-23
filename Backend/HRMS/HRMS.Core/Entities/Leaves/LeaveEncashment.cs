using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HRMS.Core.Entities.Common;
using HRMS.Core.Entities.Personnel;

namespace HRMS.Core.Entities.Leaves
{
    /// <summary>
    /// كيان صرف الإجازات - سجلات صرف رصيد الإجازة نقداً
    /// </summary>
    [Table("LEAVE_ENCASHMENT", Schema = "HR_LEAVES")]
    public class LeaveEncashment : BaseEntity
    {
        /// <summary>
        /// المعرف الفريد للصرف
        /// </summary>
        [Key]
        [Column("ENCASH_ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EncashId { get; set; }

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
        /// عدد الأيام المصروفة
        /// </summary>
        [Required(ErrorMessage = "عدد الأيام مطلوب")]
        [Column("DAYS_ENCASHED", TypeName = "decimal(5, 2)")]
        public decimal DaysEncashed { get; set; }

        /// <summary>
        /// المبلغ المدفوع
        /// </summary>
        [Required(ErrorMessage = "المبلغ مطلوب")]
        [Column("AMOUNT", TypeName = "decimal(10, 2)")]
        public decimal Amount { get; set; }

        /// <summary>
        /// تاريخ الصرف
        /// </summary>
        [Column("ENCASH_DATE")]
        public DateTime EncashDate { get; set; } = DateTime.Now;

        /// <summary>
        /// معرف مسير الراتب المرتبط (اختياري)
        /// </summary>
        [Column("PAYROLL_RUN_ID")]
        public int? PayrollRunId { get; set; }

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
