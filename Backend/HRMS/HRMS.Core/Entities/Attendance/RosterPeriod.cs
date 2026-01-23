using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HRMS.Core.Entities.Common;

namespace HRMS.Core.Entities.Attendance
{
    /// <summary>
    /// كيان فترات الجداول (الروستر) - يحدد الفترات الزمنية لجداول المناوبات
    /// </summary>
    [Table("ROSTER_PERIODS", Schema = "HR_ATTENDANCE")]
    public class RosterPeriod : BaseEntity
    {
        /// <summary>
        /// المعرف الفريد للفترة
        /// </summary>
        [Key]
        [Column("PERIOD_ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PeriodId { get; set; }

        /// <summary>
        /// السنة
        /// </summary>
        [Required(ErrorMessage = "السنة مطلوبة")]
        [Column("YEAR")]
        public short Year { get; set; }

        /// <summary>
        /// الشهر
        /// </summary>
        [Required(ErrorMessage = "الشهر مطلوب")]
        [Column("MONTH")]
        public byte Month { get; set; }

        /// <summary>
        /// تاريخ البداية
        /// </summary>
        [Required(ErrorMessage = "تاريخ البداية مطلوب")]
        [Column("START_DATE")]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// تاريخ النهاية
        /// </summary>
        [Required(ErrorMessage = "تاريخ النهاية مطلوب")]
        [Column("END_DATE")]
        public DateTime EndDate { get; set; }

        /// <summary>
        /// هل الفترة مغلقة (1=نعم، 0=لا)
        /// </summary>
        [Column("IS_LOCKED")]
        public byte IsLocked { get; set; } = 0;

        /// <summary>
        /// معرف المستخدم الذي أغلق الفترة
        /// </summary>
        [Column("LOCKED_BY")]
        public int? LockedBy { get; set; }

        /// <summary>
        /// ملاحظات
        /// </summary>
        [MaxLength(200)]
        [Column("NOTES")]
        public string? Notes { get; set; }
    }
}
