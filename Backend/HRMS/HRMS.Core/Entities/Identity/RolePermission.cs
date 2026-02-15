namespace HRMS.Core.Entities.Identity
{
    /// <summary>
    /// كيان ربط الأدوار بالصلاحيات - جدول Many-to-Many
    /// </summary>
    public class RolePermission
    {
        /// <summary>
        /// معرف الدور
        /// </summary>
        public int RoleId { get; set; }

        /// <summary>
        /// الدور المرتبط
        /// </summary>
        public virtual ApplicationRole Role { get; set; } = null!;

        /// <summary>
        /// معرف الصلاحية
        /// </summary>
        public int PermissionId { get; set; }

        /// <summary>
        /// الصلاحية المرتبطة
        /// </summary>
        public virtual Permission Permission { get; set; } = null!;

        /// <summary>
        /// تاريخ الإضافة
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
