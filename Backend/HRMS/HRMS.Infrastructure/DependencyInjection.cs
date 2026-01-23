using HRMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace HRMS.Infrastructure
{
    /// <summary>
    /// فئة تسجيل خدمات طبقة البنية التحتية (Infrastructure Layer)
    /// </summary>
    /// <remarks>
    /// تحتوي على جميع إعدادات تسجيل الخدمات الخاصة بالوصول للبيانات.
    /// يتم استدعاؤها من Program.cs لتسجيل الخدمات في حاوية الحقن (DI Container).
    /// تم اعتماد أفضل ممارسة: CQRS + DbContext مباشرة (بدون Repository Pattern).
    /// </remarks>
    public static class DependencyInjection
    {
        /// <summary>
        /// تسجيل خدمات طبقة البنية التحتية
        /// </summary>
        /// <param name="services">مجموعة الخدمات</param>
        /// <param name="configuration">إعدادات التطبيق</param>
        /// <returns>مجموعة الخدمات بعد التسجيل</returns>
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services, 
            IConfiguration configuration)
        {
            #region Database Configuration - إعدادات قاعدة البيانات

            services.AddDbContext<HRMSDbContext>(options =>
            {
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    sqlOptions =>
                    {
                        sqlOptions.MigrationsAssembly(typeof(HRMSDbContext).Assembly.FullName);
                        sqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 5,
                            maxRetryDelay: TimeSpan.FromSeconds(30),
                            errorNumbersToAdd: null);
                        sqlOptions.CommandTimeout(60);
                    });

#if DEBUG
                options.EnableSensitiveDataLogging();
                options.EnableDetailedErrors();
#endif
            });

            #endregion

            return services;
        }
    }
}
