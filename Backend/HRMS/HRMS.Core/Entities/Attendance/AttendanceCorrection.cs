using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HRMS.Core.Entities.Common;
using HRMS.Core.Entities.Personnel;

namespace HRMS.Core.Entities.Attendance
{
    /// <summary>
    /// كيان تصحيح الحضور - لتتبع التعديلات اليدوية على سجلات الحضور
    /// Attendance Correction Entity - Audit trail for manual changes
    /// </summary>
    [Table("ATTENDANCE_CORRECTIONS", Schema = "HR_ATTENDANCE")]
    public class AttendanceCorrection : BaseEntity
    {
        [Key]
        [Column("CORRECTION_ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long CorrectionId { get; set; }

        [Required]
        [Column("EMPLOYEE_ID")]
        [ForeignKey(nameof(Employee))]
        public int EmployeeId { get; set; }

        [Required]
        [Column("ATTENDANCE_DATE")]
        public DateTime AttendanceDate { get; set; }

        [Required]
        [Column("DAILY_ATTENDANCE_ID")]
        [ForeignKey(nameof(DailyAttendance))]
        public long DailyAttendanceId { get; set; }

        [MaxLength(50)]
        [Column("FIELD_NAME")]
        public string FieldName { get; set; } = string.Empty; // e.g., "ActualInTime"

        [MaxLength(100)]
        [Column("OLD_VALUE")]
        public string? OldValue { get; set; }

        [MaxLength(100)]
        [Column("NEW_VALUE")]
        public string? NewValue { get; set; }

        [Required]
        [MaxLength(500)]
        [Column("AUDIT_NOTE")]
        public string AuditNote { get; set; } = string.Empty;

        // Navigation Properties
        public virtual Employee Employee { get; set; } = null!;
        public virtual DailyAttendance DailyAttendance { get; set; } = null!;
    }
}
