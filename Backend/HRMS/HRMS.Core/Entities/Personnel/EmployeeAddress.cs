using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HRMS.Core.Entities.Common;
using HRMS.Core.Entities.Core;

namespace HRMS.Core.Entities.Personnel
{
    /// <summary>
    /// كيان العناوين - يحتوي على عناوين الموظفين (الحالي، الدائم، الطوارئ)
    /// </summary>
    [Table("EMPLOYEE_ADDRESSES", Schema = "HR_PERSONNEL")]
    public class EmployeeAddress : BaseEntity
    {
        /// <summary>
        /// المعرف الفريد للعنوان
        /// </summary>
        [Key]
        [Column("ADDRESS_ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AddressId { get; set; }

        /// <summary>
        /// معرف الموظف
        /// </summary>
        [Required(ErrorMessage = "الموظف مطلوب")]
        [Column("EMPLOYEE_ID")]
        [ForeignKey(nameof(Employee))]
        public int EmployeeId { get; set; }

        /// <summary>
        /// نوع العنوان (CURRENT, PERMANENT, EMERGENCY)
        /// </summary>
        [MaxLength(20)]
        [Column("ADDRESS_TYPE")]
        public string? AddressType { get; set; }

        /// <summary>
        /// معرف المدينة
        /// </summary>
        [Column("CITY_ID")]
        [ForeignKey(nameof(City))]
        public int? CityId { get; set; }

        /// <summary>
        /// الحي
        /// </summary>
        [MaxLength(100)]
        [Column("DISTRICT")]
        public string? District { get; set; }

        /// <summary>
        /// اسم الشارع
        /// </summary>
        [MaxLength(200)]
        [Column("STREET")]
        public string? Street { get; set; }

        /// <summary>
        /// رقم المبنى
        /// </summary>
        [MaxLength(20)]
        [Column("BUILDING_NO")]
        public string? BuildingNo { get; set; }

        /// <summary>
        /// الرمز البريدي
        /// </summary>
        [MaxLength(20)]
        [Column("POSTAL_CODE")]
        public string? PostalCode { get; set; }

        // ═══════════════════════════════════════════════════════════
        // Navigation Properties - العلاقات
        // ═══════════════════════════════════════════════════════════

        /// <summary>
        /// الموظف صاحب العنوان
        /// </summary>
        public virtual Employee Employee { get; set; } = null!;

        /// <summary>
        /// المدينة
        /// </summary>
        public virtual City? City { get; set; }
    }
}
