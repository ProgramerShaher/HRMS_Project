using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HRMS.Core.Entities.Common;
using HRMS.Core.Entities.Personnel;

namespace HRMS.Core.Entities.Leaves
{
    /// <summary>
    /// كيان أرصدة إجازات الموظفين - يتتبع الرصيد المتاح لكل نوع إجازة سنوياً
    /// </summary>
    [Table("EMPLOYEE_LEAVE_BALANCES", Schema = "HR_LEAVES")]
    public class EmployeeLeaveBalance : BaseEntity
    {
        /// <summary>
        /// المعرف الفريد للرصيد
        /// </summary>
        [Key]
        [Column("BALANCE_ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BalanceId { get; set; }

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
        /// الرصيد الحالي
        /// </summary>
        [Column("CURRENT_BALANCE", TypeName = "decimal(5, 2)")]
        public decimal CurrentBalance { get; set; } = 0;

        /// <summary>
        /// السنة المالية
        /// </summary>
        [Required(ErrorMessage = "السنة مطلوبة")]
        [Column("YEAR")]
        public short Year { get; set; }

        // ═══════════════════════════════════════════════════════════
        // Navigation Properties - العلاقات
        // ═══════════════════════════════════════════════════════════

        /// <summary>
        /// الموظف صاحب الرصيد
        /// </summary>
        public virtual Employee Employee { get; set; } = null!;

        /// <summary>
        /// نوع الإجازة
        /// </summary>
        public virtual LeaveType LeaveType { get; set; } = null!;
    }
}
