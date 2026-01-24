using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HRMS.Core.Entities.Common;

namespace HRMS.Core.Entities.Core
{
    /// <summary>
    /// كيان المدن - يحتوي على بيانات المدن التابعة للدول
    /// </summary>
    [Table("CITIES", Schema = "HR_CORE")]
    public class City : BaseEntity
    {
        /// <summary>
        /// المعرف الفريد للمدينة
        /// </summary>
        [Key]
        [Column("CITY_ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CityId { get; set; }

        /// <summary>
        /// معرف الدولة التابعة لها المدينة
        /// </summary>
        [Required(ErrorMessage = "الدولة مطلوبة")]
        [Column("COUNTRY_ID")]
        [ForeignKey(nameof(Country))]
        public int CountryId { get; set; }

        /// <summary>
        /// اسم المدينة بالعربية
        /// </summary>
        [Required(ErrorMessage = "اسم المدينة بالعربية مطلوب")]
        [MaxLength(100, ErrorMessage = "اسم المدينة لا يمكن أن يتجاوز 100 حرف")]
        [Column("CITY_NAME_AR")]
        public string CityNameAr { get; set; } = string.Empty;

        /// <summary>
        /// اسم المدينة بالإنجليزية
        /// </summary>
        [Required(ErrorMessage = "اسم المدينة بالإنجليزية مطلوب")]
        [MaxLength(100, ErrorMessage = "اسم المدينة لا يمكن أن يتجاوز 100 حرف")]
        [Column("CITY_NAME_EN")]
        public string CityNameEn { get; set; } = string.Empty;

        // ═══════════════════════════════════════════════════════════
        // Navigation Properties - العلاقات
        // ═══════════════════════════════════════════════════════════

        /// <summary>
        /// الدولة التابعة لها المدينة
        /// </summary>
        public virtual Country Country { get; set; } = null!;


    }
}
