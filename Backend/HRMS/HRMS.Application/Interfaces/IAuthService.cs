using HRMS.Application.DTOs.Auth;
using HRMS.Core.Entities.Identity;
using HRMS.Core.Utilities;

namespace HRMS.Application.Interfaces
{
    /// <summary>
    /// واجهة خدمة المصادقة (Authentication Service)
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// تسجيل مستخدم جديد
        /// </summary>
        Task<Result<AuthResponse>> RegisterAsync(RegisterRequest request);

        /// <summary>
        /// تسجيل الدخول
        /// </summary>
        Task<Result<AuthResponse>> LoginAsync(LoginRequest request);

        /// <summary>
        /// تحديث الرمز باستخدام Refresh Token
        /// </summary>
        Task<Result<AuthResponse>> RefreshTokenAsync(string refreshToken);

        /// <summary>
        /// تسجيل الخروج
        /// </summary>
        Task<bool> LogoutAsync(int userId);

        /// <summary>
        /// إضافة دور لمستخدم
        /// </summary>
        Task<bool> AddUserToRoleAsync(int userId, string roleName);

        /// <summary>
        /// إزالة دور من مستخدم
        /// </summary>
        Task<bool> RemoveUserFromRoleAsync(int userId, string roleName);

        /// <summary>
        /// الحصول على جميع المستخدمين
        /// </summary>
        Task<List<ApplicationUser>> GetAllUsersAsync();

        /// <summary>
        /// الحصول على أدوار المستخدم
        /// </summary>
        Task<List<string>> GetUserRolesAsync(int userId);
    }
}
