using Microsoft.AspNetCore.Identity;
using HRMS.Core.Entities.Personnel;

namespace HRMS.Core.Entities.Identity
{
    /// <summary>
    /// كيان المستخدم - يرث من IdentityUser لإضافة وظائف المصادقة والتفويض
    /// </summary>
    /// <remarks>
    /// يستخدم int كمعرف بدلاً من string الافتراضي لتوافق أفضل مع قاعدة البيانات
    /// </remarks>
    public class ApplicationUser : IdentityUser<int>
    {
        #region Basic Information - المعلومات الأساسية

        /// <summary>
        /// الاسم الكامل بالعربية
        /// </summary>
        public string? FullNameAr { get; set; }

        /// <summary>
        /// الاسم الكامل بالإنجليزية
        /// </summary>
        public string? FullNameEn { get; set; }

        /// <summary>
        /// هل الحساب نشط (1 = نشط، 0 = معطل)
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// تاريخ إنشاء الحساب
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// تاريخ آخر تسجيل دخول
        /// </summary>
        public DateTime? LastLoginAt { get; set; }

        #endregion

        #region Employee Relationship - العلاقة مع الموظف

        /// <summary>
        /// معرف الموظف (اختياري - قد يكون المستخدم ليس موظفاً)
        /// </summary>
        public int? EmployeeId { get; set; }

        /// <summary>
        /// الموظف المرتبط بهذا الحساب
        /// </summary>
        public virtual Employee? Employee { get; set; }

        #endregion

        #region Refresh Token - رمز التحديث

        /// <summary>
        /// رمز التحديث (Refresh Token) للحفاظ على الجلسة
        /// </summary>
        public string? RefreshToken { get; set; }

        /// <summary>
        /// تاريخ انتهاء رمز التحديث
        /// </summary>
        public DateTime? RefreshTokenExpiryTime { get; set; }

        #endregion
        #region UI Properties
        
        /// <summary>
        /// أدوار المستخدم (للعرض فقط)
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public List<string> Roles { get; set; } = new();

        #endregion
    }
}
