using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HRMS.Core.Entities.Common;
using HRMS.Core.Entities.Personnel;

namespace HRMS.Core.Entities.Attendance
{
    /// <summary>
    /// كيان طلبات العمل الإضافي - يحتوي على طلبات الموظفين للعمل الإضافي
    /// </summary>
    [Table("OVERTIME_REQUESTS", Schema = "HR_ATTENDANCE")]
    public class OvertimeRequest : BaseEntity
    {
        /// <summary>
        /// المعرف الفريد للطلب
        /// </summary>
        [Key]
        [Column("OT_REQUEST_ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OtRequestId { get; set; }

        /// <summary>
        /// معرف الموظف مقدم الطلب
        /// </summary>
        [Required(ErrorMessage = "الموظف مطلوب")]
        [Column("EMPLOYEE_ID")]
        [ForeignKey(nameof(Employee))]
        public int EmployeeId { get; set; }

        /// <summary>
        /// تاريخ تقديم الطلب
        /// </summary>
        [Column("REQUEST_DATE")]
        public DateTime RequestDate { get; set; } = DateTime.Now;

        /// <summary>
        /// تاريخ العمل الإضافي
        /// </summary>
        [Required(ErrorMessage = "تاريخ العمل الإضافي مطلوب")]
        [Column("WORK_DATE")]
        public DateTime WorkDate { get; set; }

        /// <summary>
        /// عدد الساعات المطلوبة
        /// </summary>
        [Required(ErrorMessage = "عدد الساعات مطلوب")]
        [Column("HOURS_REQUESTED", TypeName = "decimal(4, 2)")]
        public decimal HoursRequested { get; set; }

        /// <summary>
        /// سبب العمل الإضافي
        /// </summary>
        [MaxLength(200)]
        [Column("REASON")]
        public string? Reason { get; set; }

        /// <summary>
        /// حالة الطلب (PENDING, APPROVED, REJECTED)
        /// </summary>
        [MaxLength(20)]
        [Column("STATUS")]
        public string? Status { get; set; } = "PENDING";

        /// <summary>
        /// معرف المدير الذي وافق على الطلب
        /// </summary>
        [Column("APPROVED_BY")]
        [ForeignKey(nameof(Approver))]
        public int? ApprovedBy { get; set; }

        /// <summary>
        /// عدد الساعات المعتمدة
        /// </summary>
        [Column("APPROVED_HOURS", TypeName = "decimal(4, 2)")]
        public decimal? ApprovedHours { get; set; }

        // ═══════════════════════════════════════════════════════════
        // Navigation Properties - العلاقات
        // ═══════════════════════════════════════════════════════════

        /// <summary>
        /// الموظف مقدم الطلب
        /// </summary>
        public virtual Employee Employee { get; set; } = null!;

        /// <summary>
        /// المدير الذي وافق على الطلب
        /// </summary>
        public virtual Employee? Approver { get; set; }
    }
}
