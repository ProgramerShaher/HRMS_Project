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
        var adminEmail = "admin@hrms.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);

        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = "admin", 
                Email = adminEmail,
                FullNameAr = "المدير العام",
                FullNameEn = "Super Admin",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            // STRICT REQUIREMENT: Password "Admin@123"
            var result = await userManager.CreateAsync(adminUser, "Admin@123");

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "System_Admin");
            }
        }
        else if (adminUser.UserName != "admin")
        {
            adminUser.UserName = "admin";
            adminUser.NormalizedUserName = "ADMIN";
            await userManager.UpdateAsync(adminUser);
        }
    }
}
