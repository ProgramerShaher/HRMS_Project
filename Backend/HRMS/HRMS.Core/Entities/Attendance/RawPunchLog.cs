using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HRMS.Core.Entities.Common;
using HRMS.Core.Entities.Personnel;

namespace HRMS.Core.Entities.Attendance
{
    /// <summary>
    /// كيان سجلات البصمة الخام - يحتوي على بيانات الحضور من أجهزة البصمة
    /// </summary>
    [Table("RAW_PUNCH_LOGS", Schema = "HR_ATTENDANCE")]
    public class RawPunchLog : BaseEntity
    {
        /// <summary>
        /// المعرف الفريد للسجل
        /// </summary>
        [Key]
        [Column("LOG_ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long LogId { get; set; }

        /// <summary>
        /// معرف الموظف
        /// </summary>
        [Required(ErrorMessage = "الموظف مطلوب")]
        [Column("EMPLOYEE_ID")]
        [ForeignKey(nameof(Employee))]
        public int EmployeeId { get; set; }

        /// <summary>
        /// معرف الجهاز الذي تم تسجيل البصمة عليه
        /// </summary>
        [MaxLength(50)]
        [Column("DEVICE_ID")]
        public string? DeviceId { get; set; }

        /// <summary>
        /// وقت البصمة
        /// </summary>
        [Required(ErrorMessage = "وقت البصمة مطلوب")]
        [Column("PUNCH_TIME")]
        public DateTime PunchTime { get; set; }

        /// <summary>
        /// نوع البصمة (IN, OUT, BREAK_IN, BREAK_OUT)
        /// </summary>
        [MaxLength(10)]
        [Column("PUNCH_TYPE")]
        public string? PunchType { get; set; }

        /// <summary>
        /// هل تمت معالجة السجل (1=نعم، 0=لا)
        /// </summary>
        [Column("IS_PROCESSED")]
        public byte IsProcessed { get; set; } = 0;

        // ═══════════════════════════════════════════════════════════
        // Navigation Properties - العلاقات
        // ═══════════════════════════════════════════════════════════

        /// <summary>
        /// الموظف صاحب البصمة
        /// </summary>
        public virtual Employee Employee { get; set; } = null!;
    }
}
