using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HRMS.Core.Entities.Common;

namespace HRMS.Core.Entities.Core
{
    /// <summary>
    /// كيان الدرجات الوظيفية - يحتوي على بيانات الدرجات ونطاقات الرواتب
    /// </summary>
    [Table("JOB_GRADES", Schema = "HR_CORE")]
    public class JobGrade : BaseEntity
    {
        /// <summary>
        /// المعرف الفريد للدرجة الوظيفية
        /// </summary>
        [Key]
        [Column("GRADE_ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int GradeId { get; set; }

        /// <summary>
        /// اسم الدرجة الوظيفية
        /// </summary>
        [Required(ErrorMessage = "اسم الدرجة الوظيفية مطلوب")]
        [MaxLength(50, ErrorMessage = "اسم الدرجة لا يمكن أن يتجاوز 50 حرف")]
        [Column("GRADE_NAME")]
        public string GradeName { get; set; } = string.Empty;

        /// <summary>
        /// الحد الأدنى للراتب
        /// </summary>
        [Column("MIN_SALARY", TypeName = "decimal(10,2)")]
        public decimal? MinSalary { get; set; }

        /// <summary>
        /// الحد الأقصى للراتب
        /// </summary>
        [Column("MAX_SALARY", TypeName = "decimal(10,2)")]
        public decimal? MaxSalary { get; set; }

        /// <summary>
        /// درجة تذكرة السفر (اقتصادية، درجة أعمال، درجة أولى)
        /// </summary>
        [MaxLength(20)]
        [Column("TICKET_CLASS")]
        public string? TicketClass { get; set; }

        // ═══════════════════════════════════════════════════════════
        // Navigation Properties - العلاقات
        // ═══════════════════════════════════════════════════════════

        /// <summary>
        /// الوظائف المرتبطة بهذه الدرجة
        /// </summary>
        public virtual ICollection<Job> Jobs { get; set; } = new List<Job>();
    }
}
