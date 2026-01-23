using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HRMS.Core.Entities.Common;

namespace HRMS.Core.Entities.Core
{
    /// <summary>
    /// كيان سجلات التدقيق - تتبع جميع العمليات التي تتم على النظام
    /// </summary>
    [Table("AUDIT_LOGS", Schema = "HR_CORE")]
    public class AuditLog
    {
        [Key]
        [Column("LOG_ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long LogId { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("TABLE_NAME")]
        public string TableName { get; set; } = string.Empty;

        [Required]
        [Column("RECORD_ID")]
        public long RecordId { get; set; }

        [MaxLength(20)]
        [Column("ACTION_TYPE")]
        public string? ActionType { get; set; } // INSERT, UPDATE, DELETE

        [Column("OLD_VALUE")]
        public string? OldValue { get; set; }

        [Column("NEW_VALUE")]
        public string? NewValue { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("PERFORMED_BY")]
        public string PerformedBy { get; set; } = string.Empty;

        [Column("PERFORMED_AT")]
        public DateTime PerformedAt { get; set; } = DateTime.Now;

        [MaxLength(50)]
        [Column("IP_ADDRESS")]
        public string? IpAddress { get; set; }

        [MaxLength(200)]
        [Column("USER_AGENT")]
        public string? UserAgent { get; set; }
    }
}
