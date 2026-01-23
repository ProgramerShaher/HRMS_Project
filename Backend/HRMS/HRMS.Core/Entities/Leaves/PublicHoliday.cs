using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HRMS.Core.Entities.Common;

namespace HRMS.Core.Entities.Leaves
{
    /// <summary>
    /// كيان العطل الرسمية - يحدد أيام العطل الرسمية التي لا تحسب من رصيد الإجازة
    /// </summary>
    [Table("PUBLIC_HOLIDAYS", Schema = "HR_LEAVES")]
    public class PublicHoliday : BaseEntity
    {
        /// <summary>
        /// المعرف الفريد للعطلة
        /// </summary>
        [Key]
        [Column("HOLIDAY_ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int HolidayId { get; set; }

        /// <summary>
        /// اسم العطلة بالعربية
        /// </summary>
        [Required(ErrorMessage = "اسم العطلة مطلوب")]
        [MaxLength(100, ErrorMessage = "اسم العطلة لا يمكن أن يتجاوز 100 حرف")]
        [Column("HOLIDAY_NAME_AR")]
        public string HolidayNameAr { get; set; } = string.Empty;

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
        /// السنة
        /// </summary>
        [Required(ErrorMessage = "السنة مطلوبة")]
        [Column("YEAR")]
        public short Year { get; set; }
    }
}
