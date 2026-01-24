using Microsoft.AspNetCore.Identity;

namespace HRMS.Core.Entities.Identity
{
    /// <summary>
    /// كيان الدور - يرث من IdentityRole لإدارة الأدوار والصلاحيات
    /// </summary>
    public class ApplicationRole : IdentityRole<int>
    {
        /// <summary>
        /// اسم الدور بالعربية
        /// </summary>
        public string? NameAr { get; set; }

        /// <summary>
        /// وصف الدور
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// تاريخ الإنشاء
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
