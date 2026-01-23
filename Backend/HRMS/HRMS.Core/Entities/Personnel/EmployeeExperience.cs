using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HRMS.Core.Entities.Common;

namespace HRMS.Core.Entities.Personnel
{
    /// <summary>
    /// كيان الخبرات السابقة - يحتوي على التوظيف السابق للموظف
    /// </summary>
    [Table("EMPLOYEE_EXPERIENCES", Schema = "HR_PERSONNEL")]
    public class EmployeeExperience : BaseEntity
    {
        /// <summary>
        /// المعرف الفريد للخبرة
        /// </summary>
        [Key]
        [Column("EXPERIENCE_ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ExperienceId { get; set; }

        /// <summary>
        /// معرف الموظف
        /// </summary>
        [Required(ErrorMessage = "الموظف مطلوب")]
        [Column("EMPLOYEE_ID")]
        [ForeignKey(nameof(Employee))]
        public int EmployeeId { get; set; }

        /// <summary>
        /// اسم الشركة بالعربية
        /// </summary>
        [Required(ErrorMessage = "اسم الشركة مطلوب")]
        [MaxLength(200)]
        [Column("COMPANY_NAME_AR")]
        public string CompanyNameAr { get; set; } = string.Empty;

        /// <summary>
        /// المسمى الوظيفي بالعربية
        /// </summary>
        [MaxLength(100)]
        [Column("JOB_TITLE_AR")]
        public string? JobTitleAr { get; set; }

        /// <summary>
        /// تاريخ البداية
        /// </summary>
        [Required(ErrorMessage = "تاريخ البداية مطلوب")]
        [Column("START_DATE")]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// تاريخ النهاية
        /// </summary>
        [Column("END_DATE")]
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// هل هي الخبرة الحالية (1=نعم، 0=لا)
        /// </summary>
        [Column("IS_CURRENT")]
        public byte IsCurrent { get; set; } = 0;

        /// <summary>
        /// المسؤوليات والمهام
        /// </summary>
        [MaxLength(500)]
        [Column("RESPONSIBILITIES")]
        public string? Responsibilities { get; set; }

        /// <summary>
        /// سبب ترك العمل
        /// </summary>
        [MaxLength(200)]
        [Column("REASON_FOR_LEAVING")]
        public string? ReasonForLeaving { get; set; }

        // ═══════════════════════════════════════════════════════════
        // Navigation Properties - العلاقات
        // ═══════════════════════════════════════════════════════════

        /// <summary>
        /// الموظف صاحب الخبرة
        /// </summary>
        public virtual Employee Employee { get; set; } = null!;
    }
}
