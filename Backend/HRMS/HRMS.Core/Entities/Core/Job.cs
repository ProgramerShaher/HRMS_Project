using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HRMS.Core.Entities.Common;

namespace HRMS.Core.Entities.Core
{
    /// <summary>
    /// كيان الوظائف - يحتوي على بيانات المسميات الوظيفية
    /// </summary>
    [Table("JOBS", Schema = "HR_CORE")]
    public class Job : BaseEntity
    {
        /// <summary>
        /// المعرف الفريد للوظيفة
        /// </summary>
        [Key]
        [Column("JOB_ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int JobId { get; set; }

        /// <summary>
        /// المسمى الوظيفي بالعربية
        /// </summary>
        [Required(ErrorMessage = "المسمى الوظيفي بالعربية مطلوب")]
        [MaxLength(100, ErrorMessage = "المسمى الوظيفي لا يمكن أن يتجاوز 100 حرف")]
        [Column("JOB_TITLE_AR")]
        public string JobTitleAr { get; set; } = string.Empty;

        /// <summary>
        /// المسمى الوظيفي بالإنجليزية
        /// </summary>
        [MaxLength(100)]
        [Column("JOB_TITLE_EN")]
        public string? JobTitleEn { get; set; }

        /// <summary>
        /// معرف الدرجة الوظيفية الافتراضية
        /// </summary>
        [Column("DEFAULT_GRADE_ID")]
        [ForeignKey(nameof(DefaultGrade))]
        public int? DefaultGradeId { get; set; }

        /// <summary>
        /// هل الوظيفة طبية (1 = نعم، 0 = لا)
        /// </summary>
        [Column("IS_MEDICAL")]
        public byte IsMedical { get; set; } = 0;

        // ═══════════════════════════════════════════════════════════
        // Navigation Properties - العلاقات
        // ═══════════════════════════════════════════════════════════

        /// <summary>
        /// الدرجة الوظيفية الافتراضية
        /// </summary>
        public virtual JobGrade? DefaultGrade { get; set; }
    }
}
