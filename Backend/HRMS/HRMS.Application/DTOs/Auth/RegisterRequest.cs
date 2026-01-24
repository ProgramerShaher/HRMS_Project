namespace HRMS.Application.DTOs.Auth
{
    /// <summary>
    /// نموذج طلب تسجيل مستخدم جديد
    /// </summary>
    public class RegisterRequest
    {
        /// <summary>
        /// اسم المستخدم (فريد)
        /// </summary>
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// البريد الإلكتروني
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// كلمة المرور
        /// </summary>
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// تأكيد كلمة المرور
        /// </summary>
        public string ConfirmPassword { get; set; } = string.Empty;

        /// <summary>
        /// الاسم الكامل بالعربية
        /// </summary>
        public string? FullNameAr { get; set; }

        /// <summary>
        /// الاسم الكامل بالإنجليزية
        /// </summary>
        public string? FullNameEn { get; set; }

        /// <summary>
        /// رقم الجوال
        /// </summary>
        public string? PhoneNumber { get; set; }

        /// <summary>
        /// معرف الموظف (اختياري - إذا كان المستخدم موظف)
        /// </summary>
        public int? EmployeeId { get; set; }
    }
}
