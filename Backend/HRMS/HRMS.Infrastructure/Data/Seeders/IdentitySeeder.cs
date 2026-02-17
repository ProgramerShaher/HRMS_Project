using Microsoft.AspNetCore.Identity;
using HRMS.Core.Entities.Identity;

namespace HRMS.Infrastructure.Data.Seeders;

/// <summary>
/// بذر بيانات الهوية (Identity Seeder) - الأدوار والمستخدمين الافتراضيين
/// </summary>
public static class IdentitySeeder
{
    /// <summary>
    /// إضافة الأدوار والمستخدم السوبر أدمن
    /// </summary>
    public static async Task SeedAsync(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
    {
        await SeedRolesAsync(roleManager);
        await SeedSuperAdminAsync(userManager);
    }

    private static async Task SeedRolesAsync(RoleManager<ApplicationRole> roleManager)
    {
        var roles = new List<(string Name, string NameAr, string Description)>
        {
            ("System_Admin", "مدير النظام", "صلاحيات كاملة على النظام"),
            ("HR_Manager", "مدير الموارد البشرية", "إدارة كاملة للموظفين والعمليات"),
            ("Employee", "موظف", "عرض البيانات الشخصية فقط"),
            // Add other roles if necessary
        };

        foreach (var (name, nameAr, description) in roles)
        {
            if (!await roleManager.RoleExistsAsync(name))
            {
                var role = new ApplicationRole
                {
                    Name = name,
                    NameAr = nameAr,
                    Description = description,
                    NormalizedName = name.ToUpper(),
                    CreatedAt = DateTime.UtcNow
                };

                await roleManager.CreateAsync(role);
            }
        }
    }

    private static async Task SeedSuperAdminAsync(UserManager<ApplicationUser> userManager)
    {
        // 1. System Admin
        var adminEmail = "admin@hrms.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);

        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = "admin", 
                Email = adminEmail,
                FullNameAr = "المدير العام",
                FullNameEn = "System Administrator",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            var result = await userManager.CreateAsync(adminUser, "Admin@123");
            if (result.Succeeded) await userManager.AddToRoleAsync(adminUser, "System_Admin");
        }

        // 2. HR Manager
        var hrEmail = "hr@hrms.com";
        var hrUser = await userManager.FindByEmailAsync(hrEmail);
        if (hrUser == null)
        {
            hrUser = new ApplicationUser
            {
                UserName = "hr_manager",
                Email = hrEmail,
                FullNameAr = "مدير الموارد البشرية",
                FullNameEn = "HR Manager",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            var result = await userManager.CreateAsync(hrUser, "HR@123");
            if (result.Succeeded) await userManager.AddToRoleAsync(hrUser, "HR_Manager");
        }

        // 3. Employee
        var empEmail = "employee@hrms.com";
        var empUser = await userManager.FindByEmailAsync(empEmail);
        if (empUser == null)
        {
            empUser = new ApplicationUser
            {
                UserName = "employee",
                Email = empEmail,
                FullNameAr = "موظف تجريبي",
                FullNameEn = "Demo Employee",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            var result = await userManager.CreateAsync(empUser, "Employee@123");
            if (result.Succeeded) await userManager.AddToRoleAsync(empUser, "Employee");
        }
    }
}
