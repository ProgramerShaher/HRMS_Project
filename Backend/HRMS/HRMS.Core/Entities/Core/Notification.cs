using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HRMS.Core.Entities.Common;

namespace HRMS.Core.Entities.Core
{
    /// <summary>
    /// كيان الاشعارات - تنبيهات النظام للمستخدمين
    /// </summary>
    [Table("NOTIFICATIONS", Schema = "HR_CORE")]
    public class Notification : BaseEntity
    {
        [Key]
        [Column("NOTIFICATION_ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long NotificationId { get; set; }

        [Required]
        [Column("RECIPIENT_ID")]
        public int RecipientId { get; set; }

        [MaxLength(50)]
        [Column("NOTIFICATION_TYPE")]
        public string? NotificationType { get; set; }

        [Required]
        [MaxLength(200)]
        [Column("TITLE_AR")]
        public string TitleAr { get; set; } = string.Empty;

        [MaxLength(500)]
        [Column("MESSAGE_AR")]
        public string? MessageAr { get; set; }

        [Column("IS_READ")]
        public byte IsRead { get; set; } = 0;

        [Column("READ_AT")]
        public DateTime? ReadAt { get; set; }

        [MaxLength(20)]
        [Column("PRIORITY")]
        public string? Priority { get; set; } = "NORMAL";

        [MaxLength(100)]
        [Column("REFERENCE_TABLE")]
        public string? ReferenceTable { get; set; }

        [Column("REFERENCE_ID")]
        public long? ReferenceId { get; set; }
    }
}
