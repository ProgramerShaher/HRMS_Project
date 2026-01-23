using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HRMS.Core.Entities.Common;

namespace HRMS.Core.Entities.Core
{
    /// <summary>
    /// كيان الدول - يحتوي على بيانات الدول والجنسيات
    /// </summary>
    [Table("COUNTRIES", Schema = "HR_CORE")]
    public class Country : BaseEntity
    {
        /// <summary>
        /// المعرف الفريد للدولة
        /// </summary>
        [Key]
        [Column("COUNTRY_ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CountryId { get; set; }

        /// <summary>
        /// اسم الدولة بالعربية
        /// </summary>
        [Required(ErrorMessage = "اسم الدولة بالعربية مطلوب")]
        [MaxLength(100, ErrorMessage = "اسم الدولة لا يمكن أن يتجاوز 100 حرف")]
        [Column("COUNTRY_NAME_AR")]
        public string CountryNameAr { get; set; } = string.Empty;

        /// <summary>
        /// اسم الدولة بالإنجليزية
        /// </summary>
        [Required(ErrorMessage = "اسم الدولة بالإنجليزية مطلوب")]
        [MaxLength(100, ErrorMessage = "اسم الدولة لا يمكن أن يتجاوز 100 حرف")]
        [Column("COUNTRY_NAME_EN")]
        public string CountryNameEn { get; set; } = string.Empty;

        /// <summary>
        /// اسم الجنسية بالعربية (مثل: سعودي، مصري)
        /// </summary>
        [MaxLength(100)]
        [Column("CITIZENSHIP_NAME_AR")]
        public string? CitizenshipNameAr { get; set; }

        /// <summary>
        /// رمز الدولة ISO (مثل: SA, EG, AE)
        /// </summary>
        [MaxLength(2)]
        [Column("ISO_CODE")]
        public string? IsoCode { get; set; }

        // ═══════════════════════════════════════════════════════════
        // Navigation Properties - العلاقات
        // ═══════════════════════════════════════════════════════════

        /// <summary>
        /// المدن التابعة لهذه الدولة
        /// </summary>
        public virtual ICollection<City> Cities { get; set; } = new List<City>();
    }
}
