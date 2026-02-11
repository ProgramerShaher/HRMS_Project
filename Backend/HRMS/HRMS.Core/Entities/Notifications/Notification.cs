using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HRMS.Core.Entities.Common;

namespace HRMS.Core.Entities.Notifications;

[Table("NOTIFICATIONS", Schema = "HR_COMMON")]
public class Notification : BaseEntity
{
    [Key]
    [Column("NOTIFICATION_ID")]
    public Guid NotificationId { get; set; } = Guid.NewGuid();

    [Column("USER_ID")]
    [MaxLength(450)]
    public required string UserId { get; set; } // ApplicationUser.Id

    [Column("TITLE")]
    [MaxLength(200)]
    public required string Title { get; set; }

    [Column("MESSAGE")]
    [MaxLength(1000)]
    public required string Message { get; set; }

    [Column("TYPE")]
    [MaxLength(50)]
    public string Type { get; set; } = "Info"; // Info, Warning, Error, Success

    [Column("REFERENCE_TYPE")]
    [MaxLength(100)]
    public string? ReferenceType { get; set; } // e.g., "LeaveRequest", "Document"

    [Column("REFERENCE_ID")]
    [MaxLength(100)]
    public string? ReferenceId { get; set; } // e.g., "1023"

    [Column("IS_READ")]
    public bool IsRead { get; set; } = false;

    [Column("CREATED_AT")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
