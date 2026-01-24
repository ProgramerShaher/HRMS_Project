namespace HRMS.Application.Settings
{
    /// <summary>
    /// إعدادات JWT (JSON Web Token)
    /// </summary>
    public class JwtSettings
    {
        /// <summary>
        /// المفتاح السري لتوقيع الرموز
        /// </summary>
        public string Secret { get; set; } = string.Empty;

        /// <summary>
        /// الجهة المُصدرة للرمز (Issuer)
        /// </summary>
        public string Issuer { get; set; } = string.Empty;

        /// <summary>
        /// الجهة المستقبلة للرمز (Audience)
        /// </summary>
        public string Audience { get; set; } = string.Empty;

        /// <summary>
        /// مدة صلاحية الرمز بالدقائق (Access Token)
        /// </summary>
        public int AccessTokenExpirationMinutes { get; set; } = 60;

        /// <summary>
        /// مدة صلاحية رمز التحديث بالأيام (Refresh Token)
        /// </summary>
        public int RefreshTokenExpirationDays { get; set; } = 7;
    }
}
