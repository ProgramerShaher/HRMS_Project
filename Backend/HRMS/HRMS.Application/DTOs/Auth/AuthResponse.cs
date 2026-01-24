namespace HRMS.Application.DTOs.Auth
{
    /// <summary>
    /// نموذج استجابة المصادقة (Authentication Response)
    /// </summary>
    public class AuthResponse
    {
        /// <summary>
        /// معرف المستخدم
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// اسم المستخدم
        /// </summary>
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// البريد الإلكتروني
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// الاسم الكامل
        /// </summary>
        public string? FullName { get; set; }

        /// <summary>
        /// رمز الوصول (Access Token - JWT)
        /// </summary>
        public string Token { get; set; } = string.Empty;

        /// <summary>
        /// رمز التحديث (Refresh Token)
        /// </summary>
        public string? RefreshToken { get; set; }

        /// <summary>
        /// تاريخ انتهاء الرمز
        /// </summary>
        public DateTime TokenExpiration { get; set; }

        /// <summary>
        /// الأدوار المخصصة للمستخدم
        /// </summary>
        public List<string> Roles { get; set; } = new();

        /// <summary>
        /// معرف الموظف (إذا كان المستخدم موظف)
        /// </summary>
        public int? EmployeeId { get; set; }
    }
}
