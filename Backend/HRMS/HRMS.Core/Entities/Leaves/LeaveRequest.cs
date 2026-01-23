using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HRMS.Core.Entities.Common;
using HRMS.Core.Entities.Personnel;

namespace HRMS.Core.Entities.Leaves
{
    /// <summary>
    /// كيان طلبات الإجازة - سجلات تقديم الإجازات وحالتها
    /// </summary>
    [Table("LEAVE_REQUESTS", Schema = "HR_LEAVES")]
    public class LeaveRequest : BaseEntity
    {
        /// <summary>
        /// المعرف الفريد للطلب
        /// </summary>
        [Key]
        [Column("REQUEST_ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RequestId { get; set; }

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
        /// تاريخ البداية
        /// </summary>
        [Required(ErrorMessage = "تاريخ البداية مطلوب")]
        [Column("START_DATE")]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// تاريخ النهاية
        /// </summary>
        [Required(ErrorMessage = "تاريخ النهاية مطلوب")]
        [Column("END_DATE")]
        public DateTime EndDate { get; set; }

        /// <summary>
        /// عدد الأيام
        /// </summary>
        [Required(ErrorMessage = "عدد الأيام مطلوب")]
        [Column("DAYS_COUNT")]
        public int DaysCount { get; set; }

        /// <summary>
        /// سبب الإجازة
        /// </summary>
        [MaxLength(200)]
        [Column("REASON")]
        public string? Reason { get; set; }

        /// <summary>
        /// مسار المرفقات
        /// </summary>
        [MaxLength(500)]
        [Column("ATTACHMENT_PATH")]
        public string? AttachmentPath { get; set; }

        /// <summary>
        /// حالة الطلب (PENDING, MANAGER_APPROVED, HR_APPROVED, REJECTED)
        /// </summary>
        [MaxLength(20)]
        [Column("STATUS")]
        public string? Status { get; set; } = "PENDING";

        /// <summary>
        /// سبب الرفض
        /// </summary>
        [MaxLength(200)]
        [Column("REJECTION_REASON")]
        public string? RejectionReason { get; set; }

        /// <summary>
        /// هل تم خصمها من الرصيد (1=نعم، 0=لا)
        /// </summary>
        [Column("IS_POSTED_TO_BALANCE")]
        public byte IsPostedToBalance { get; set; } = 0;

        // ═══════════════════════════════════════════════════════════
        // Navigation Properties - العلاقات
        // ═══════════════════════════════════════════════════════════

        /// <summary>
        /// الموظف مقدم الطلب
        /// </summary>
        public virtual Employee Employee { get; set; } = null!;

        /// <summary>
        /// نوع الإجازة
        /// </summary>
        public virtual LeaveType LeaveType { get; set; } = null!;
    }
}
