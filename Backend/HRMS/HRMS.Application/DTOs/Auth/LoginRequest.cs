namespace HRMS.Application.DTOs.Auth
{
    /// <summary>
    /// نموذج طلب تسجيل الدخول
    /// </summary>
    public class LoginRequest
    {
        /// <summary>
        /// اسم المستخدم أو البريد الإلكتروني
        /// </summary>
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// كلمة المرور
        /// </summary>
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// تذكرني (للحفاظ على الجلسة)
        /// </summary>
        public bool RememberMe { get; set; } = false;
    }
}
