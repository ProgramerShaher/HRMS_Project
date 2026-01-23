using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HRMS.Core.Entities.Common;

namespace HRMS.Core.Entities.Core
{
    /// <summary>
    /// كيان الفروع - يحتوي على بيانات فروع المستشفى
    /// </summary>
    [Table("BRANCHES", Schema = "HR_CORE")]
    public class Branch : BaseEntity
    {
        /// <summary>
        /// المعرف الفريد للفرع
        /// </summary>
        [Key]
        [Column("BRANCH_ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BranchId { get; set; }

        /// <summary>
        /// اسم الفرع بالعربية
        /// </summary>
        [Required(ErrorMessage = "اسم الفرع بالعربية مطلوب")]
        [MaxLength(100, ErrorMessage = "اسم الفرع لا يمكن أن يتجاوز 100 حرف")]
        [Column("BRANCH_NAME_AR")]
        public string BranchNameAr { get; set; } = string.Empty;

        /// <summary>
        /// اسم الفرع بالإنجليزية
        /// </summary>
        [MaxLength(100)]
        [Column("BRANCH_NAME_EN")]
        public string? BranchNameEn { get; set; }

        /// <summary>
        /// معرف المدينة التي يقع فيها الفرع
        /// </summary>
        [Column("CITY_ID")]
        [ForeignKey(nameof(City))]
        public int? CityId { get; set; }

        /// <summary>
        /// عنوان الفرع
        /// </summary>
        [MaxLength(200)]
        [Column("ADDRESS")]
        public string? Address { get; set; }

        // ═══════════════════════════════════════════════════════════
        // Navigation Properties - العلاقات
        // ═══════════════════════════════════════════════════════════

        /// <summary>
        /// المدينة التي يقع فيها الفرع
        /// </summary>
        public virtual City? City { get; set; }

        /// <summary>
        /// الأقسام الموجودة في هذا الفرع
        /// </summary>
        public virtual ICollection<Department> Departments { get; set; } = new List<Department>();
    }
}
