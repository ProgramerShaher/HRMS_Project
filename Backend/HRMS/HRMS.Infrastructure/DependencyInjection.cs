using HRMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using HRMS.Application.Interfaces;
using HRMS.Infrastructure.Services;

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
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // تسجيل DbContext مع كل الإعدادات المتقدمة
            services.AddDbContext<HRMSDbContext>(options => {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });

            // التسجيل الصحيح والواضح للواجهة
            services.AddScoped<IApplicationDbContext, HRMSDbContext>();
            
            // تسجيل الخدمات الأساسية (Domain Services)
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IReportingService, ReportingService>();

            return services;
        }
    }
}
