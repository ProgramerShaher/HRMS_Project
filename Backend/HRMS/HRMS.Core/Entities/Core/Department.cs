using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HRMS.Core.Entities.Common;

namespace HRMS.Core.Entities.Core
{
    /// <summary>
    /// كيان الأقسام - يحتوي على بيانات الأقسام والهيكل التنظيمي
    /// </summary>
    [Table("DEPARTMENTS", Schema = "HR_CORE")]
    public class Department : BaseEntity
    {
        /// <summary>
        /// المعرف الفريد للقسم
        /// </summary>
        [Key]
        [Column("DEPT_ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DeptId { get; set; }

        /// <summary>
        /// اسم القسم بالعربية
        /// </summary>
        [Required(ErrorMessage = "اسم القسم بالعربية مطلوب")]
        [MaxLength(100, ErrorMessage = "اسم القسم لا يمكن أن يتجاوز 100 حرف")]
        [Column("DEPT_NAME_AR")]
        public string DeptNameAr { get; set; } = string.Empty;

        /// <summary>
        /// اسم القسم بالإنجليزية
        /// </summary>
        [MaxLength(100)]
        [Column("DEPT_NAME_EN")]
        public string? DeptNameEn { get; set; }

        /// <summary>
        /// معرف القسم الأب (للأقسام الفرعية)
        /// </summary>
        [Column("PARENT_DEPT_ID")]
        [ForeignKey(nameof(ParentDepartment))]
        public int? ParentDeptId { get; set; }



        /// <summary>
        /// رمز مركز التكلفة
        /// </summary>
        [MaxLength(50)]
        [Column("COST_CENTER_CODE")]
        public string? CostCenterCode { get; set; }

        /// <summary>
        /// معرف مدير القسم
        /// </summary>
        [Column("MANAGER_ID")]
        public int? ManagerId { get; set; }

        /// <summary>
        /// حالة القسم (1 = نشط، 0 = غير نشط)
        /// </summary>
        [Column("IS_ACTIVE")]
        public byte IsActive { get; set; } = 1;

        // ═══════════════════════════════════════════════════════════
        // Navigation Properties - العلاقات
        // ═══════════════════════════════════════════════════════════



        /// <summary>
        /// القسم الأب (في حالة الأقسام الفرعية)
        /// </summary>
        public virtual Department? ParentDepartment { get; set; }

        /// <summary>
        /// الأقسام الفرعية التابعة لهذا القسم
        /// </summary>
        [InverseProperty(nameof(ParentDepartment))]
        public virtual ICollection<Department> SubDepartments { get; set; } = new List<Department>();

        /// <summary>
        /// الوظائف الموجودة في هذا القسم
        /// </summary>
        public virtual ICollection<Job> Jobs { get; set; } = new List<Job>();
    }
}
