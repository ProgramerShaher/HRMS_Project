using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HRMS.Core.Entities.Common;
using HRMS.Core.Entities.Personnel;

namespace HRMS.Core.Entities.Attendance;

[Table("PERMISSION_REQUESTS", Schema = "HR_ATTENDANCE")]
public class PermissionRequest : BaseEntity
{
    [Key]
    [Column("PERMISSION_REQUEST_ID")]
    public int PermissionRequestId { get; set; }

    [Column("EMPLOYEE_ID")]
    [ForeignKey(nameof(Employee))]
    public int EmployeeId { get; set; }

    [Column("PERMISSION_TYPE")]
    [MaxLength(20)]
    public string PermissionType { get; set; } = string.Empty; // LateEntry, EarlyExit

    [Column("PERMISSION_DATE")]
    public DateTime PermissionDate { get; set; }

    [Column("HOURS")]
    public decimal Hours { get; set; }

    [Column("REASON")]
    [MaxLength(500)]
    public string Reason { get; set; } = string.Empty;

    [Column("STATUS")]
    [MaxLength(20)]
    public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected

    [Column("REJECTION_REASON")]
    [MaxLength(500)]
    public string? RejectionReason { get; set; }

    [Column("APPROVED_BY")]
    public int? ApprovedBy { get; set; }

    [Column("APPROVED_AT")]
    public DateTime? ApprovedAt { get; set; }

    public virtual Employee Employee { get; set; } = null!;
}
