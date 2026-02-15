using HRMS.Core.Entities.Identity;
namespace HRMS.Application.Interfaces
{
    /// <summary>
    /// واجهة خدمة الصلاحيات (Permission Service)
    /// </summary>
    public interface IPermissionService
    {
        /// <summary>
        /// الحصول على جميع صلاحيات المستخدم
        /// </summary>
        Task<List<string>> GetUserPermissionsAsync(int userId);

        /// <summary>
        /// الحصول على صلاحيات دور معين
        /// </summary>
        Task<List<string>> GetRolePermissionsAsync(int roleId);

        /// <summary>
        /// التحقق من امتلاك المستخدم لصلاحية معينة
        /// </summary>
        /// <summary>
        /// الحصول على جميع الأدوار
        /// </summary>
        Task<List<ApplicationRole>> GetAllRolesAsync();

        /// <summary>
        /// الحصول على دور بواسطة المعرف
        /// </summary>
        Task<ApplicationRole?> GetRoleByIdAsync(int roleId);

        /// <summary>
        /// إنشاء دور جديد
        /// </summary>
        Task<bool> CreateRoleAsync(string roleName, string? description);

        /// <summary>
        /// تحديث دور
        /// </summary>
        Task<bool> UpdateRoleAsync(int roleId, string roleName, string? description);

        /// <summary>
        /// حذف دور
        /// </summary>
        Task<bool> DeleteRoleAsync(int roleId);

        /// <summary>
        /// الحصول على جميع الصلاحيات المتاحة في النظام
        /// </summary>
        Task<List<Permission>> GetAllPermissionsAsync();

        /// <summary>
        /// تحديث صلاحيات دور معين (إضافة/حذف)
        /// </summary>
        Task<bool> UpdateRolePermissionsAsync(int roleId, List<int> permissionIds);
    }
}
