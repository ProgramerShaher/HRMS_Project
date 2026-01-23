using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HRMS.Core.Entities.Common;

namespace HRMS.Core.Entities.Personnel
{
    /// <summary>
    /// كيان جهات الاتصال للطوارئ - يحتوي على أرقام التواصل في حالات الطوارئ
    /// </summary>
    [Table("EMERGENCY_CONTACTS", Schema = "HR_PERSONNEL")]
    public class EmergencyContact : BaseEntity
    {
        /// <summary>
        /// المعرف الفريد لجهة الاتصال
        /// </summary>
        [Key]
        [Column("CONTACT_ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ContactId { get; set; }

        /// <summary>
        /// معرف الموظف
        /// </summary>
        [Required(ErrorMessage = "الموظف مطلوب")]
        [Column("EMPLOYEE_ID")]
        [ForeignKey(nameof(Employee))]
        public int EmployeeId { get; set; }

        /// <summary>
        /// الاسم بالعربية
        /// </summary>
        [Required(ErrorMessage = "الاسم بالعربية مطلوب")]
        [MaxLength(100)]
        [Column("CONTACT_NAME_AR")]
        public string ContactNameAr { get; set; } = string.Empty;

        /// <summary>
        /// العلاقة (أب، أم، أخ، زوجة)
        /// </summary>
        [MaxLength(50)]
        [Column("RELATIONSHIP")]
        public string? Relationship { get; set; }

        /// <summary>
        /// رقم الهاتف الأساسي
        /// </summary>
        [Required(ErrorMessage = "رقم الهاتف الأساسي مطلوب")]
        [MaxLength(20)]
        [Column("PHONE_PRIMARY")]
        public string PhonePrimary { get; set; } = string.Empty;

        /// <summary>
        /// رقم الهاتف الثانوي
        /// </summary>
        [MaxLength(20)]
        [Column("PHONE_SECONDARY")]
        public string? PhoneSecondary { get; set; }

        /// <summary>
        /// هل هي جهة الاتصال الرئيسية (1=نعم، 0=لا)
        /// </summary>
        [Column("IS_PRIMARY")]
        public byte IsPrimary { get; set; } = 0;

        // ═══════════════════════════════════════════════════════════
        // Navigation Properties - العلاقات
        // ═══════════════════════════════════════════════════════════

        /// <summary>
        /// الموظف صاحب جهة الاتصال
        /// </summary>
        public virtual Employee Employee { get; set; } = null!;
    }
}
