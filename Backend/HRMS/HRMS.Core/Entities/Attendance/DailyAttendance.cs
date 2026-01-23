using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HRMS.Core.Entities.Common;
using HRMS.Core.Entities.Personnel;

namespace HRMS.Core.Entities.Attendance
{
    /// <summary>
    /// كيان الحضور اليومي المعالج - يحتوي على بيانات الحضور النهائية بعد المعالجة
    /// </summary>
    [Table("DAILY_ATTENDANCE", Schema = "HR_ATTENDANCE")]
    public class DailyAttendance : BaseEntity
    {
        /// <summary>
        /// المعرف الفريد للسجل
        /// </summary>
        [Key]
        [Column("RECORD_ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long RecordId { get; set; }

        /// <summary>
        /// معرف الموظف
        /// </summary>
        [Required(ErrorMessage = "الموظف مطلوب")]
        [Column("EMPLOYEE_ID")]
        [ForeignKey(nameof(Employee))]
        public int EmployeeId { get; set; }

        /// <summary>
        /// تاريخ الحضور
        /// </summary>
        [Required(ErrorMessage = "تاريخ الحضور مطلوب")]
        [Column("ATTENDANCE_DATE")]
        public DateTime AttendanceDate { get; set; }

        /// <summary>
        /// معرف المناوبة المخططة
        /// </summary>
        [Column("PLANNED_SHIFT_ID")]
        [ForeignKey(nameof(PlannedShift))]
        public int? PlannedShiftId { get; set; }

        /// <summary>
        /// وقت الدخول الفعلي
        /// </summary>
        [Column("ACTUAL_IN_TIME")]
        public DateTime? ActualInTime { get; set; }

        /// <summary>
        /// وقت الخروج الفعلي
        /// </summary>
        [Column("ACTUAL_OUT_TIME")]
        public DateTime? ActualOutTime { get; set; }

        /// <summary>
        /// دقائق التأخير
        /// </summary>
        [Column("LATE_MINUTES")]
        public short LateMinutes { get; set; } = 0;

        /// <summary>
        /// دقائق الانصراف المبكر
        /// </summary>
        [Column("EARLY_LEAVE_MINUTES")]
        public short EarlyLeaveMinutes { get; set; } = 0;

        /// <summary>
        /// دقائق العمل الإضافي
        /// </summary>
        [Column("OVERTIME_MINUTES")]
        public short OvertimeMinutes { get; set; } = 0;

        /// <summary>
        /// الحالة (PRESENT, ABSENT, LEAVE, HOLIDAY, MISSING_PUNCH)
        /// </summary>
        [MaxLength(20)]
        [Column("STATUS")]
        public string? Status { get; set; }

        // ═══════════════════════════════════════════════════════════
        // Navigation Properties - العلاقات
        // ═══════════════════════════════════════════════════════════

        /// <summary>
        /// الموظف صاحب السجل
        /// </summary>
        public virtual Employee Employee { get; set; } = null!;

        /// <summary>
        /// المناوبة المخططة
        /// </summary>
        public virtual ShiftType? PlannedShift { get; set; }
    }
}
