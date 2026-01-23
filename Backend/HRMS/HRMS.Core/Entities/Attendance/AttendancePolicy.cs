using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HRMS.Core.Entities.Common;
using HRMS.Core.Entities.Core;

namespace HRMS.Core.Entities.Attendance
{
    /// <summary>
    /// كيان سياسات الحضور - يحدد قواعد الدوام والتأخير والإضافي
    /// </summary>
    [Table("ATTENDANCE_POLICIES", Schema = "HR_ATTENDANCE")]
    public class AttendancePolicy : BaseEntity
    {
        /// <summary>
        /// المعرف الفريد للسياسة
        /// </summary>
        [Key]
        [Column("POLICY_ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PolicyId { get; set; }

        /// <summary>
        /// اسم السياسة بالعربية
        /// </summary>
        [Required(ErrorMessage = "اسم السياسة مطلوب")]
        [MaxLength(100, ErrorMessage = "اسم السياسة لا يمكن أن يتجاوز 100 حرف")]
        [Column("POLICY_NAME_AR")]
        public string PolicyNameAr { get; set; } = string.Empty;

        /// <summary>
        /// معرف القسم المطبق عليه (اختياري)
        /// </summary>
        [Column("DEPT_ID")]
        [ForeignKey(nameof(Department))]
        public int? DeptId { get; set; }

        /// <summary>
        /// معرف الوظيفة المطبق عليها (اختياري)
        /// </summary>
        [Column("JOB_ID")]
        [ForeignKey(nameof(Job))]
        public int? JobId { get; set; }

        /// <summary>
        /// فترة السماح بالتأخير (دقائق)
        /// </summary>
        [Column("LATE_GRACE_MINS")]
        public short LateGraceMins { get; set; } = 15;

        /// <summary>
        /// معامل الوقت الإضافي العادي (مثلاً 1.5)
        /// </summary>
        [Column("OVERTIME_MULTIPLIER", TypeName = "decimal(3, 2)")]
        public decimal OvertimeMultiplier { get; set; } = 1.5m;

        /// <summary>
        /// معامل الوقت الإضافي في العطلات (مثلاً 2.0)
        /// </summary>
        [Column("WEEKEND_OT_MULTIPLIER", TypeName = "decimal(3, 2)")]
        public decimal WeekendOtMultiplier { get; set; } = 2.0m;

        // ═══════════════════════════════════════════════════════════
        // Navigation Properties - العلاقات
        // ═══════════════════════════════════════════════════════════

        /// <summary>
        /// القسم
        /// </summary>
        public virtual Department? Department { get; set; }

        /// <summary>
        /// الوظيفة
        /// </summary>
        public virtual Job? Job { get; set; }
    }
}
