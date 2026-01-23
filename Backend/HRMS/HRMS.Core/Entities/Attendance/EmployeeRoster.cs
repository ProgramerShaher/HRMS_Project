using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HRMS.Core.Entities.Common;
using HRMS.Core.Entities.Personnel;

namespace HRMS.Core.Entities.Attendance
{
    /// <summary>
    /// كيان جدول الموظف - يحدد مناوبة العمل لكل موظف في يوم محدد
    /// </summary>
    [Table("EMPLOYEE_ROSTERS", Schema = "HR_ATTENDANCE")]
    public class EmployeeRoster : BaseEntity
    {
        /// <summary>
        /// المعرف الفريد للسجل
        /// </summary>
        [Key]
        [Column("ROSTER_ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long RosterId { get; set; }

        /// <summary>
        /// معرف الموظف
        /// </summary>
        [Required(ErrorMessage = "الموظف مطلوب")]
        [Column("EMPLOYEE_ID")]
        [ForeignKey(nameof(Employee))]
        public int EmployeeId { get; set; }

        /// <summary>
        /// تاريخ الجدول
        /// </summary>
        [Required(ErrorMessage = "تاريخ الجدول مطلوب")]
        [Column("ROSTER_DATE")]
        public DateTime RosterDate { get; set; }

        /// <summary>
        /// معرف المناوبة المخططة
        /// </summary>
        [Column("SHIFT_ID")]
        [ForeignKey(nameof(ShiftType))]
        public int? ShiftId { get; set; }

        /// <summary>
        /// هل هو يوم راحة (1=نعم، 0=لا)
        /// </summary>
        [Column("IS_OFF_DAY")]
        public byte IsOffDay { get; set; } = 0;

        // ═══════════════════════════════════════════════════════════
        // Navigation Properties - العلاقات
        // ═══════════════════════════════════════════════════════════

        /// <summary>
        /// الموظف صاحب الجدول
        /// </summary>
        public virtual Employee Employee { get; set; } = null!;

        /// <summary>
        /// نوع المناوبة المخططة
        /// </summary>
        public virtual ShiftType? ShiftType { get; set; }
    }
}
