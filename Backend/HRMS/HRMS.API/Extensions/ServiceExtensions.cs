using System.Reflection.Metadata;
using System.Text;
using HRMS.Application.Interfaces;
using HRMS.Application.Services;
using HRMS.Application.Features.Payroll.Processing.Services;
using HRMS.Application.Settings;
using HRMS.Core.Entities.Identity;
using HRMS.Infrastructure.Data;
using HRMS.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using HRMS.Application.Features.Payroll.Loans.Services;

namespace HRMS.API.Extensions;

/// <summary>
/// امتدادات تسجيل الخدمات في Dependency Injection Container
/// </summary>
public static class ServiceExtensions
{
    /// <summary>
    /// تسجيل قاعدة البيانات و DbContext
    /// </summary>
    public static IServiceCollection AddDatabaseServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<HRMSDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly("HRMS.Infrastructure")
            )
        );

        // تسجيل الواجهة - Dependency Inversion Principle
        // استخدام cast صريح لأن HRMSDbContext لا ينفذ الواجهة رسمياً (Duck Typing)
        services.AddScoped<IApplicationDbContext>(provider => 
            (IApplicationDbContext)provider.GetRequiredService<HRMSDbContext>());

        return services;
    }

   
    public static IServiceCollection AddIdentityServices(this IServiceCollection services)
    {
        services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
        {
            // Password settings
            options.Password.RequireDigit = true;
            options.Password.RequiredLength = 8;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = true;
            options.Password.RequireLowercase = true;

            // Lockout settings
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.AllowedForNewUsers = true;

            // User settings
            options.User.RequireUniqueEmail = true;
        })
        .AddEntityFrameworkStores<HRMSDbContext>()
        .AddDefaultTokenProviders();

        return services;
    }

    /// <summary>
    /// تسجيل JWT Authentication
    /// </summary>
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSettingsSection = configuration.GetSection("JwtSettings");
        services.Configure<JwtSettings>(jwtSettingsSection);
        var jwtSettings = jwtSettingsSection.Get<JwtSettings>()!;

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret))
            };
        });

        return services;
    }

    /// <summary>
    /// تسجيل Authorization (مع إمكانية التعطيل للتطوير)
    /// </summary>
    public static IServiceCollection AddAuthorizationPolicies(this IServiceCollection services, bool isDevelopment = false)
    {
        if (isDevelopment)
        {
            // 🔓 DEVELOPMENT MODE: تعطيل Authorization للاختبار
            services.AddAuthorization(options =>
            {
                // إلغاء جميع متطلبات Authorization
                options.DefaultPolicy = new AuthorizationPolicyBuilder()
                    .RequireAssertion(_ => true)
                    .Build();
                    
                options.FallbackPolicy = new AuthorizationPolicyBuilder()
                    .RequireAssertion(_ => true)
                    .Build();
            });
        }
        else
        {
            // 🔐 PRODUCTION MODE: Authorization مفعل
            services.AddAuthorization();
        }

        return services;
    }

    /// <summary>
    /// تسجيل خدمات التطبيق (Services)
    /// </summary>
    public static IServiceCollection AddCustomServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IPermissionService, PermissionService>();
        services.AddScoped<IFileService, FileService>();
        services.AddScoped<PayrollAccountingService>();
        
        // Payroll & Loan Services
        services.AddScoped<GenerateInstallmentsService>();
        
        // يمكن إضافة المزيد من الخدمات هنا

        return services;
    }
}
