using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HRMS.Core.Entities.Common;
using HRMS.Core.Entities.Core;

namespace HRMS.Core.Entities.Personnel
{
    /// <summary>
    /// كيان المؤهلات العلمية - يحتوي على الدرجات العلمية وتخصصات الموظف
    /// </summary>
    [Table("EMPLOYEE_QUALIFICATIONS", Schema = "HR_PERSONNEL")]
    public class EmployeeQualification : BaseEntity
    {
        /// <summary>
        /// المعرف الفريد للمؤهل
        /// </summary>
        [Key]
        [Column("QUALIFICATION_ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int QualificationId { get; set; }

        /// <summary>
        /// معرف الموظف
        /// </summary>
        [Required(ErrorMessage = "الموظف مطلوب")]
        [Column("EMPLOYEE_ID")]
        [ForeignKey(nameof(Employee))]
        public int EmployeeId { get; set; }

        /// <summary>
        /// نوع الدرجة العلمية (بكالوريوس، ماجستير، دكتوراه، دبلوم)
        /// </summary>
        [Required(ErrorMessage = "نوع الدرجة العلمية مطلوب")]
        [MaxLength(50)]
        [Column("DEGREE_TYPE")]
        public string DegreeType { get; set; } = string.Empty;

        /// <summary>
        /// التخصص بالعربية
        /// </summary>
        [Required(ErrorMessage = "التخصص مطلوب")]
        [MaxLength(100)]
        [Column("MAJOR_AR")]
        public string MajorAr { get; set; } = string.Empty;

        /// <summary>
        /// الجامعة أو المؤسسة التعليمية
        /// </summary>
        [MaxLength(200)]
        [Column("UNIVERSITY_AR")]
        public string? UniversityAr { get; set; }

        /// <summary>
        /// دولة التخرج
        /// </summary>
        [Column("COUNTRY_ID")]
        [ForeignKey(nameof(Country))]
        public int? CountryId { get; set; }

        /// <summary>
        /// سنة التخرج
        /// </summary>
        [Column("GRADUATION_YEAR")]
        public short? GraduationYear { get; set; }

        /// <summary>
        /// التقدير (ممتاز، جيد جداً...)
        /// </summary>
        [MaxLength(20)]
        [Column("GRADE")]
        public string? Grade { get; set; }

        /// <summary>
        /// مسار صورة الشهادة
        /// </summary>
        [MaxLength(500)]
        [Column("ATTACHMENT_PATH")]
        public string? AttachmentPath { get; set; }

        // ═══════════════════════════════════════════════════════════
        // Navigation Properties - العلاقات
        // ═══════════════════════════════════════════════════════════

        /// <summary>
        /// الموظف صاحب المؤهل
        /// </summary>
        public virtual Employee Employee { get; set; } = null!;

        /// <summary>
        /// دولة التخرج
        /// </summary>
        public virtual Country? Country { get; set; }
    }
}
