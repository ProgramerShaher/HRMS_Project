using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HRMS.Core.Entities.Common;

namespace HRMS.Core.Entities.Core
{
    /// <summary>
    /// كيان الموافقات - تتبع سلسلة الموافقات للطلبات
    /// </summary>
    [Table("WORKFLOW_APPROVALS", Schema = "HR_CORE")]
    public class WorkflowApproval : BaseEntity
    {
        [Key]
        [Column("APPROVAL_ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ApprovalId { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("REQUEST_TYPE")]
        public string RequestType { get; set; } = string.Empty;

        [Required]
        [Column("REQUEST_ID")]
        public long RequestId { get; set; }

        [Required]
        [Column("APPROVER_LEVEL")]
        public byte ApproverLevel { get; set; }

        [Required]
        [Column("APPROVER_ID")]
        public int ApproverId { get; set; }

        [MaxLength(20)]
        [Column("STATUS")]
        public string? Status { get; set; } = "PENDING"; // PENDING, APPROVED, REJECTED, DELEGATED

        [Column("APPROVAL_DATE")]
        public DateTime? ApprovalDate { get; set; }

        [MaxLength(500)]
        [Column("COMMENTS")]
        public string? Comments { get; set; }
    }
}
