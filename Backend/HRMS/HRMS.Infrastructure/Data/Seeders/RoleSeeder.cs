using Microsoft.AspNetCore.Identity;
using HRMS.Core.Entities.Identity;

namespace HRMS.Infrastructure.Data.Seeders
{
    /// <summary>
    /// بذر الأدوار الافتراضية (Default Roles Seeder)
    /// </summary>
    public static class RoleSeeder
    {
        /// <summary>
        /// إضافة الأدوار الافتراضية إلى قاعدة البيانات
        /// </summary>
        public static async Task SeedRolesAsync(RoleManager<ApplicationRole> roleManager)
        {
            var roles = new List<(string Name, string NameAr, string Description)>
            {
                ("System_Admin", "مدير النظام", "صلاحيات كاملة على النظام"),
                ("HR_Manager", "مدير الموارد البشرية", "إدارة كاملة للموظفين والعمليات"),
                ("HR_Employee", "موظف موارد بشرية", "عرض وتعديل بيانات الموظفين"),
                ("Payroll_Admin", "مدير الرواتب", "إدارة الرواتب والبدلات"),
                ("Payroll_Employee", "موظف رواتب", "عرض ومعالجة الرواتب"),
                ("Department_Manager", "مدير قسم", "إدارة موظفي القسم"),
                ("Employee", "موظف", "عرض البيانات الشخصية فقط"),
                ("Reports_Viewer", "مشاهد التقارير", "عرض التقارير فقط"),
                ("Attendance_Admin", "مدير الحضور", "إدارة الحضور والانصراف"),
                ("Leave_Approver", "مدير الإجازات", "الموافقة على طلبات الإجازات"),
                ("Recruitment_Manager", "مدير التوظيف", "إدارة عمليات التوظيف"),
                ("Performance_Manager", "مدير الأداء", "إدارة تقييمات الأداء")
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

        /// <summary>
        /// إنشاء مستخدم Admin افتراضي
        /// </summary>
        public static async Task SeedDefaultAdminAsync(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager)
        {
            // التأكد من وجود دور System_Admin
            if (!await roleManager.RoleExistsAsync("System_Admin"))
            {
                await SeedRolesAsync(roleManager);
            }

            // التحقق من عدم وجود Admin مسبقاً
            var adminUser = await userManager.FindByNameAsync("admin");
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = "admin",
                    Email = "admin@hrms.com",
                    FullNameAr = "المدير العام",
                    FullNameEn = "System Administrator",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                // كلمة المرور الافتراضية (يجب تغييرها)
                var result = await userManager.CreateAsync(adminUser, "Admin@123");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "System_Admin");
                }
            }
        }
    }
}
