using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HRMS.Core.Entities.Common;

namespace HRMS.Core.Entities.Personnel
{
    /// <summary>
    /// كيان التابعين (المرافقين) - يحتوي على بيانات أفراد عائلة الموظف
    /// </summary>
    [Table("DEPENDENTS", Schema = "HR_PERSONNEL")]
    public class Dependent : BaseEntity
    {
        /// <summary>
        /// المعرف الفريد للتابع
        /// </summary>
        [Key]
        [Column("DEPENDENT_ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DependentId { get; set; }

        /// <summary>
        /// معرف الموظف العائل
        /// </summary>
        [Required(ErrorMessage = "الموظف مطلوب")]
        [Column("EMPLOYEE_ID")]
        [ForeignKey(nameof(Employee))]
        public int EmployeeId { get; set; }

        /// <summary>
        /// الاسم بالعربية
        /// </summary>
        [Required(ErrorMessage = "الاسم مطلوب")]
        [MaxLength(100)]
        [Column("NAME_AR")]
        public string NameAr { get; set; } = string.Empty;

        /// <summary>
        /// العلاقة (زوجة، ابن، ابنة، أب، أم)
        /// </summary>
        [Required(ErrorMessage = "العلاقة مطلوبة")]
        [MaxLength(50)]
        [Column("RELATIONSHIP")]
        public string Relationship { get; set; } = string.Empty;

        /// <summary>
        /// تاريخ الميلاد
        /// </summary>
        [Column("BIRTH_DATE")]
        public DateTime? BirthDate { get; set; }

        /// <summary>
        /// رقم الهوية
        /// </summary>
        [MaxLength(20)]
        [Column("NATIONAL_ID")]
        public string? NationalId { get; set; }

        /// <summary>
        /// هل يستحق تذاكر سفر (1=نعم، 0=لا)
        /// </summary>
        [Column("IS_ELIGIBLE_FOR_TICKET")]
        public byte IsEligibleForTicket { get; set; } = 1;

        /// <summary>
        /// هل يستحق تأمين طبي (1=نعم، 0=لا)
        /// </summary>
        [Column("IS_ELIGIBLE_FOR_INSURANCE")]
        public byte IsEligibleForInsurance { get; set; } = 1;

        // ═══════════════════════════════════════════════════════════
        // Navigation Properties - العلاقات
        // ═══════════════════════════════════════════════════════════

        /// <summary>
        /// الموظف العائل
        /// </summary>
        public virtual Employee Employee { get; set; } = null!;
    }
}
