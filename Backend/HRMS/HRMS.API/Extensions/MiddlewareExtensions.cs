using Serilog;
using Microsoft.AspNetCore.Identity;
using HRMS.Core.Entities.Identity;
using HRMS.API.Middleware;
using HRMS.Infrastructure.Data.Seeders;

namespace HRMS.API.Extensions;

/// <summary>
/// امتدادات Middleware Pipeline
/// </summary>
public static class MiddlewareExtensions
{
    /// <summary>
    /// تكوين Middleware Pipeline
    /// </summary>
    public static IApplicationBuilder UseCustomMiddleware(this IApplicationBuilder app, IWebHostEnvironment env)
    {
        // Serilog Request Logging
        app.UseSerilogRequestLogging();

        // Exception Handling
        // Exception Handling
        app.UseMiddleware<GlobalExceptionMiddleware>();
        
        // if (env.IsDevelopment())
        // {
        //     app.UseDeveloperExceptionPage();
        // }
        // else
        // {
        //     app.UseHsts();
        // }

        // HTTPS Redirection
        app.UseHttpsRedirection();

        // CORS
        app.UseCors("AllowAll");

        // Authentication & Authorization
        app.UseAuthentication();
        app.UseAuthorization();

        return app;
    }

    /// <summary>
    /// تنفيذ Seed Data (الأدوار والمستخدم الافتراضي)
    /// </summary>
    public static async Task SeedDatabaseAsync(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var services = scope.ServiceProvider;

        try
        {
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = services.GetRequiredService<RoleManager<ApplicationRole>>();

            await IdentitySeeder.SeedAsync(userManager, roleManager);

            Log.Information("✅ Default roles and admin user seeded successfully");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "❌ An error occurred while seeding roles");
        }
    }
}
