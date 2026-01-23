using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HRMS.Core.Entities.Common;

namespace HRMS.Core.Entities.Attendance
{
    /// <summary>
    /// كيان أنواع المناوبات - يحتوي على تعريفات الورديات (صباحي، مسائي، ليلي)
    /// </summary>
    [Table("SHIFT_TYPES", Schema = "HR_ATTENDANCE")]
    public class ShiftType : BaseEntity
    {
        /// <summary>
        /// المعرف الفريد للمناوبة
        /// </summary>
        [Key]
        [Column("SHIFT_ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ShiftId { get; set; }

        /// <summary>
        /// اسم المناوبة بالعربية
        /// </summary>
        [Required(ErrorMessage = "اسم المناوبة مطلوب")]
        [MaxLength(100, ErrorMessage = "اسم المناوبة لا يمكن أن يتجاوز 100 حرف")]
        [Column("SHIFT_NAME_AR")]
        public string ShiftNameAr { get; set; } = string.Empty;

        /// <summary>
        /// وقت البداية (HH:mm)
        /// </summary>
        [Required(ErrorMessage = "وقت البداية مطلوب")]
        [MaxLength(5)]
        [Column("START_TIME")]
        public string StartTime { get; set; } = string.Empty;

        /// <summary>
        /// وقت النهاية (HH:mm)
        /// </summary>
        [Required(ErrorMessage = "وقت النهاية مطلوب")]
        [MaxLength(5)]
        [Column("END_TIME")]
        public string EndTime { get; set; } = string.Empty;

        /// <summary>
        /// عدد الساعات
        /// </summary>
        [Column("HOURS_COUNT")]
        public decimal HoursCount { get; set; }

        /// <summary>
        /// هل المناوبة تمتد لليوم التالي (1=نعم، 0=لا)
        /// </summary>
        [Column("IS_CROSS_DAY")]
        public byte IsCrossDay { get; set; } = 0;

        /// <summary>
        /// فترة السماح بالدقيقة
        /// </summary>
        [Column("GRACE_PERIOD_MINS")]
        public short GracePeriodMins { get; set; } = 15;
    }
}
