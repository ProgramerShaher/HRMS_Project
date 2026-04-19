using HRMS.Core.Entities.Common;

namespace HRMS.Core.Entities.Identity
{
    /// <summary>
    /// كيان الصلاحية - يمثل صلاحية واحدة في النظام
    /// </summary>
    public class Permission : BaseEntity
    {
        /// <summary>
        /// معرف الصلاحية
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// اسم الصلاحية (مثل: Employees.View, Employees.Create)
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// اسم الصلاحية بالعربية
        /// </summary>
        public string NameAr { get; set; } = string.Empty;

        /// <summary>
        /// الوحدة التي تنتمي إليها الصلاحية (Personnel, Payroll, Leaves, etc.)
        /// </summary>
        public string Module { get; set; } = string.Empty;

        /// <summary>
        /// وصف الصلاحية
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// الأدوار المرتبطة بهذه الصلاحية
        /// </summary>
        public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    }
}
