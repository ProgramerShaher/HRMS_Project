using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HRMS.Core.Entities.Common;
using HRMS.Core.Entities.Personnel;

namespace HRMS.Core.Entities.Attendance
{
    /// <summary>
    /// كيان طلبات تبديل المناوبات - يحتوي على طلبات الموظفين لتبديل (Swap) المناوبات
    /// </summary>
    [Table("SHIFT_SWAP_REQUESTS", Schema = "HR_ATTENDANCE")]
    public class ShiftSwapRequest : BaseEntity
    {
        /// <summary>
        /// المعرف الفريد للطلب
        /// </summary>
        [Key]
        [Column("REQUEST_ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RequestId { get; set; }

        /// <summary>
        /// معرف الموظف مقدم الطلب
        /// </summary>
        [Required(ErrorMessage = "مقدم الطلب مطلوب")]
        [Column("REQUESTER_ID")]
        [ForeignKey(nameof(Requester))]
        public int RequesterId { get; set; }

        /// <summary>
        /// معرف الموظف البديل (المستهدف)
        /// </summary>
        [Required(ErrorMessage = "الموظف البديل مطلوب")]
        [Column("TARGET_EMPLOYEE_ID")]
        [ForeignKey(nameof(TargetEmployee))]
        public int TargetEmployeeId { get; set; }

        /// <summary>
        /// تاريخ الجدول المراد تبديله
        /// </summary>
        [Required(ErrorMessage = "تاريخ الجدول مطلوب")]
        [Column("ROSTER_DATE")]
        public DateTime RosterDate { get; set; }

        /// <summary>
        /// حالة الطلب (PENDING, APPROVED, REJECTED)
        /// </summary>
        [MaxLength(20)]
        [Column("STATUS")]
        public string? Status { get; set; } = "PENDING";

        /// <summary>
        /// تعليق المدير
        /// </summary>
        [MaxLength(200)]
        [Column("MANAGER_COMMENT")]
        public string? ManagerComment { get; set; }

        // ═══════════════════════════════════════════════════════════
        // Navigation Properties - العلاقات
        // ═══════════════════════════════════════════════════════════

        /// <summary>
        /// الموظف مقدم الطلب
        /// </summary>
        public virtual Employee Requester { get; set; } = null!;

        /// <summary>
        /// الموظف البديل
        /// </summary>
        public virtual Employee TargetEmployee { get; set; } = null!;
    }
}
