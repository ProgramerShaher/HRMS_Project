using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HRMS.Core.Entities.Common;
using HRMS.Core.Entities.Personnel;

namespace HRMS.Core.Entities.Leaves
{
    /// <summary>
    /// سجل تاريخ اعتماد الإجازات - لتتبع جميع الإجراءات التي تمت على الطلب
    /// Leave Approval History - Tracks all actions taken on a leave request
    /// </summary>
    [Table("LEAVE_APPROVAL_HISTORY", Schema = "HR_LEAVES")]
    public class LeaveApprovalHistory : BaseEntity
    {
        public LeaveApprovalHistory() { }

        /// <summary>
        /// المعرف الفريد للسجل
        /// </summary>
        [Key]
        [Column("HISTORY_ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int HistoryId { get; set; }

        /// <summary>
        /// معرف طلب الإجازة
        /// </summary>
        [Required]
        [Column("REQUEST_ID")]
        [ForeignKey(nameof(LeaveRequest))]
        public int RequestId { get; set; }

        /// <summary>
        /// نوع الإجراء (Submit, Approve, Reject, Cancel)
        /// </summary>
        [Required]
        [MaxLength(50)]
        [Column("ACTION_TYPE")]
        public string ActionType { get; set; } = string.Empty;

        /// <summary>
        /// معرف المستخدم الذي قام بالإجراء (عادة رقم الموظف أو userId)
        /// </summary>
        [MaxLength(50)]
        [Column("PERFORMED_BY_USER_ID")]
        public string? PerformedByUserId { get; set; }
        
        /// <summary>
        /// معرف الموظف الذي قام بالإجراء (إذا كان متاحاً)
        /// </summary>
        [Column("PERFORMED_BY_EMPLOYEE_ID")]
        public int? PerformedByEmployeeId { get; set; }

        /// <summary>
        /// تاريخ وتوقيت الإجراء
        /// </summary>
        [Required]
        [Column("ACTION_DATE")]
        public DateTime ActionDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// ملاحظات أو تعليق على الإجراء
        /// </summary>
        [MaxLength(500)]
        [Column("COMMENT")]
        public string? Comment { get; set; }

        /// <summary>
        /// الحالة السابقة للطلب قبل الإجراء
        /// </summary>
        [MaxLength(50)]
        [Column("PREVIOUS_STATUS")]
        public string? PreviousStatus { get; set; }

        /// <summary>
        /// الحالة الجديدة للطلب بعد الإجراء
        /// </summary>
        [MaxLength(50)]
        [Column("NEW_STATUS")]
        public string? NewStatus { get; set; }

        // ═══════════════════════════════════════════════════════════
        // Navigation Properties - العلاقات
        // ═══════════════════════════════════════════════════════════

        public virtual LeaveRequest LeaveRequest { get; set; } = null!;
        public virtual Employee? PerformedByEmployee { get; set; }
    }
}
