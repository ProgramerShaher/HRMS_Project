using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HRMS.Core.Entities.Common;

namespace HRMS.Core.Entities.Core
{
    /// <summary>
    /// كيان الدرجات الوظيفية - يحدد مستويات الوظائف ونطاقات الرواتب
    /// </summary>
    [Table("JOB_GRADES", Schema = "HR_CORE")]
    public class JobGrade : BaseEntity
    {
        /// <summary>
        /// المعرف الفريد للدرجة الوظيفية
        /// </summary>
        [Key]
        [Column("JOB_GRADE_ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int JobGradeId { get; set; }

        /// <summary>
        /// رمز الدرجة الوظيفية
        /// </summary>
        [Required]
        [MaxLength(20)]
        [Column("GRADE_CODE")]
        public string GradeCode { get; set; } = string.Empty;

        /// <summary>
        /// اسم الدرجة الوظيفية بالعربية
        /// </summary>
        [Required]
        [MaxLength(100)]
        [Column("GRADE_NAME_AR")]
        public string GradeNameAr { get; set; } = string.Empty;

        /// <summary>
        /// اسم الدرجة الوظيفية بالإنجليزية
        /// </summary>
        [Required]
        [MaxLength(100)]
        [Column("GRADE_NAME_EN")]
        public string GradeNameEn { get; set; } = string.Empty;

        /// <summary>
        /// مستوى الدرجة (1 = الأدنى)
        /// </summary>
        [Column("GRADE_LEVEL")]
        public int GradeLevel { get; set; }

        /// <summary>
        /// الحد الأدنى للراتب
        /// </summary>
        [Column("MIN_SALARY")]
        [Range(0, double.MaxValue)]
        public decimal MinSalary { get; set; }

        /// <summary>
        /// الحد الأقصى للراتب
        /// </summary>
        [Column("MAX_SALARY")]
        [Range(0, double.MaxValue)]
        public decimal MaxSalary { get; set; }

        /// <summary>
        /// إعدادات المزايا (JSON)
        /// </summary>
        [Column("BENEFITS_CONFIG")]
        public string? BenefitsConfig { get; set; }

        /// <summary>
        /// وصف الدرجة الوظيفية
        /// </summary>
        [MaxLength(500)]
        [Column("DESCRIPTION")]
        public string? Description { get; set; }

        // Navigation Properties
        public virtual ICollection<Job> Jobs { get; set; } = new List<Job>();
    }
}
