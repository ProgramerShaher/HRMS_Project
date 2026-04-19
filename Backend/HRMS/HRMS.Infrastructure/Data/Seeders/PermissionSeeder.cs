using HRMS.Core.Entities.Identity;
using HRMS.Infrastructure.Data;

namespace HRMS.Infrastructure.Data.Seeders
{
    /// <summary>
    /// بذر الصلاحيات الافتراضية (Default Permissions Seeder)
    /// </summary>
    public static class PermissionSeeder
    {
        /// <summary>
        /// إضافة الصلاحيات الافتراضية إلى قاعدة البيانات
        /// </summary>
        public static async Task SeedPermissionsAsync(HRMSDbContext context)
        {
            if (context.Permissions.Any())
            {
                return; // الصلاحيات موجودة مسبقاً
            }

            var permissions = new List<Permission>
            {
                // ===================================
                // Personnel Module - وحدة الموظفين
                // ===================================
                new Permission { Name = "Employees.View", NameAr = "عرض الموظفين", Module = "Personnel", Description = "View employees list and details" },
                new Permission { Name = "Employees.Create", NameAr = "إضافة موظف", Module = "Personnel", Description = "Create new employees" },
                new Permission { Name = "Employees.Edit", NameAr = "تعديل موظف", Module = "Personnel", Description = "Edit employee information" },
                new Permission { Name = "Employees.Delete", NameAr = "حذف موظف", Module = "Personnel", Description = "Delete employees" },
                new Permission { Name = "Employees.Export", NameAr = "تصدير الموظفين", Module = "Personnel", Description = "Export employees data" },

                // ===================================
                // Payroll Module - وحدة الرواتب
                // ===================================
                new Permission { Name = "Payroll.View", NameAr = "عرض الرواتب", Module = "Payroll", Description = "View payroll information" },
                new Permission { Name = "Payroll.Process", NameAr = "معالجة الرواتب", Module = "Payroll", Description = "Process monthly payroll" },
                new Permission { Name = "Payroll.Approve", NameAr = "اعتماد الرواتب", Module = "Payroll", Description = "Approve payroll runs" },
                new Permission { Name = "Payroll.Edit", NameAr = "تعديل الرواتب", Module = "Payroll", Description = "Edit payroll details" },
                new Permission { Name = "Payroll.Export", NameAr = "تصدير الرواتب", Module = "Payroll", Description = "Export payroll reports" },
                
                // Loans
                new Permission { Name = "Loans.View", NameAr = "عرض السلف", Module = "Payroll", Description = "View loans" },
                new Permission { Name = "Loans.Create", NameAr = "إنشاء سلفة", Module = "Payroll", Description = "Create new loans" },
                new Permission { Name = "Loans.Approve", NameAr = "اعتماد السلف", Module = "Payroll", Description = "Approve loan requests" },
                new Permission { Name = "Loans.Delete", NameAr = "حذف سلفة", Module = "Payroll", Description = "Delete loans" },

                // ===================================
                // Leaves Module - وحدة الإجازات
                // ===================================
                new Permission { Name = "Leaves.View", NameAr = "عرض الإجازات", Module = "Leaves", Description = "View leave requests" },
                new Permission { Name = "Leaves.Create", NameAr = "طلب إجازة", Module = "Leaves", Description = "Create leave requests" },
                new Permission { Name = "Leaves.Approve", NameAr = "اعتماد الإجازات", Module = "Leaves", Description = "Approve leave requests" },
                new Permission { Name = "Leaves.Reject", NameAr = "رفض الإجازات", Module = "Leaves", Description = "Reject leave requests" },
                new Permission { Name = "Leaves.Manage", NameAr = "إدارة الإجازات", Module = "Leaves", Description = "Manage leave settings and types" },
                new Permission { Name = "Leaves.ViewAll", NameAr = "عرض جميع الإجازات", Module = "Leaves", Description = "View all employees leaves" },

                // ===================================
                // Attendance Module - وحدة الحضور
                // ===================================
                new Permission { Name = "Attendance.View", NameAr = "عرض الحضور", Module = "Attendance", Description = "View attendance records" },
                new Permission { Name = "Attendance.Manage", NameAr = "إدارة الحضور", Module = "Attendance", Description = "Manage attendance and rosters" },
                new Permission { Name = "Attendance.Approve", NameAr = "اعتماد طلبات الحضور", Module = "Attendance", Description = "Approve attendance requests" },
                new Permission { Name = "Attendance.Edit", NameAr = "تعديل الحضور", Module = "Attendance", Description = "Edit attendance records" },
                new Permission { Name = "Attendance.ViewReports", NameAr = "عرض تقارير الحضور", Module = "Attendance", Description = "View attendance reports" },

                // ===================================
                // Performance Module - وحدة الأداء
                // ===================================
                new Permission { Name = "Performance.View", NameAr = "عرض الأداء", Module = "Performance", Description = "View performance data" },
                new Permission { Name = "Performance.Manage", NameAr = "إدارة الأداء", Module = "Performance", Description = "Manage performance appraisals" },
                new Permission { Name = "Performance.Evaluate", NameAr = "تقييم الأداء", Module = "Performance", Description = "Evaluate employee performance" },
                new Permission { Name = "Violations.View", NameAr = "عرض المخالفات", Module = "Performance", Description = "View violations" },
                new Permission { Name = "Violations.Create", NameAr = "إضافة مخالفة", Module = "Performance", Description = "Create violations" },
                new Permission { Name = "Violations.Manage", NameAr = "إدارة المخالفات", Module = "Performance", Description = "Manage violations and disciplinary actions" },

                // ===================================
                // Recruitment Module - وحدة التوظيف
                // ===================================
                new Permission { Name = "Recruitment.View", NameAr = "عرض التوظيف", Module = "Recruitment", Description = "View recruitment data" },
                new Permission { Name = "Recruitment.Manage", NameAr = "إدارة التوظيف", Module = "Recruitment", Description = "Manage recruitment process" },
                new Permission { Name = "Candidates.View", NameAr = "عرض المرشحين", Module = "Recruitment", Description = "View candidates" },
                new Permission { Name = "Candidates.Create", NameAr = "إضافة مرشح", Module = "Recruitment", Description = "Add new candidates" },

                // ===================================
                // Setup Module - وحدة الإعدادات
                // ===================================
                new Permission { Name = "Setup.View", NameAr = "عرض الإعدادات", Module = "Setup", Description = "View system setup" },
                new Permission { Name = "Setup.Manage", NameAr = "إدارة الإعدادات", Module = "Setup", Description = "Manage system settings" },
                new Permission { Name = "Setup.Countries", NameAr = "إدارة الدول", Module = "Setup", Description = "Manage countries" },
                new Permission { Name = "Setup.Cities", NameAr = "إدارة المدن", Module = "Setup", Description = "Manage cities" },
                new Permission { Name = "Setup.Departments", NameAr = "إدارة الأقسام", Module = "Setup", Description = "Manage departments" },
                new Permission { Name = "Setup.Jobs", NameAr = "إدارة الوظائف", Module = "Setup", Description = "Manage jobs" },

                // ===================================
                // Reports Module - وحدة التقارير
                // ===================================
                new Permission { Name = "Reports.View", NameAr = "عرض التقارير", Module = "Reports", Description = "View reports" },
                new Permission { Name = "Reports.Export", NameAr = "تصدير التقارير", Module = "Reports", Description = "Export reports" },
                new Permission { Name = "Reports.Advanced", NameAr = "التقارير المتقدمة", Module = "Reports", Description = "Access advanced reports" },

                // ===================================
                // Users & Roles Module - وحدة المستخدمين والأدوار
                // ===================================
                new Permission { Name = "Users.View", NameAr = "عرض المستخدمين", Module = "Administration", Description = "View users" },
                new Permission { Name = "Users.Create", NameAr = "إضافة مستخدم", Module = "Administration", Description = "Create new users" },
                new Permission { Name = "Users.Edit", NameAr = "تعديل مستخدم", Module = "Administration", Description = "Edit user information" },
                new Permission { Name = "Users.Delete", NameAr = "حذف مستخدم", Module = "Administration", Description = "Delete users" },
                new Permission { Name = "Users.AssignRoles", NameAr = "تعيين الأدوار", Module = "Administration", Description = "Assign roles to users" },
                
                new Permission { Name = "Roles.View", NameAr = "عرض الأدوار", Module = "Administration", Description = "View roles" },
                new Permission { Name = "Roles.Create", NameAr = "إضافة دور", Module = "Administration", Description = "Create new roles" },
                new Permission { Name = "Roles.Edit", NameAr = "تعديل دور", Module = "Administration", Description = "Edit role information" },
                new Permission { Name = "Roles.Delete", NameAr = "حذف دور", Module = "Administration", Description = "Delete roles" },
                new Permission { Name = "Roles.AssignPermissions", NameAr = "تعيين الصلاحيات", Module = "Administration", Description = "Assign permissions to roles" },
            };

            await context.Permissions.AddRangeAsync(permissions);
            await context.SaveChangesAsync();
        }

        /// <summary>
        /// ربط الصلاحيات الافتراضية بالأدوار
        /// </summary>
        public static async Task SeedRolePermissionsAsync(HRMSDbContext context)
        {
            if (context.RolePermissions.Any())
            {
                return; // الربط موجود مسبقاً
            }

            // الحصول على الأدوار والصلاحيات
            var systemAdminRole = context.ApplicationRoles.FirstOrDefault(r => r.Name == "System_Admin");
            var hrManagerRole = context.ApplicationRoles.FirstOrDefault(r => r.Name == "HR_Manager");
            var employeeRole = context.ApplicationRoles.FirstOrDefault(r => r.Name == "Employee");
            
            var allPermissions = context.Permissions.ToList();

            if (systemAdminRole != null)
            {
                // System_Admin: جميع الصلاحيات
                var systemAdminPermissions = allPermissions.Select(p => new RolePermission
                {
                    RoleId = systemAdminRole.Id,
                    PermissionId = p.Id,
                    CreatedAt = DateTime.UtcNow
                }).ToList();

                await context.RolePermissions.AddRangeAsync(systemAdminPermissions);
            }

            if (hrManagerRole != null)
            {
                // HR_Manager: صلاحيات الموظفين والإجازات والحضور
                var hrPermissions = allPermissions
                    .Where(p => p.Module == "Personnel" || p.Module == "Leaves" || p.Module == "Attendance" || p.Module == "Performance")
                    .Select(p => new RolePermission
                    {
                        RoleId = hrManagerRole.Id,
                        PermissionId = p.Id,
                        CreatedAt = DateTime.UtcNow
                    }).ToList();

                await context.RolePermissions.AddRangeAsync(hrPermissions);
            }

            if (employeeRole != null)
            {
                // Employee: صلاحيات محدودة (عرض فقط)
                var employeePermissions = allPermissions
                    .Where(p => p.Name.Contains("View") && !p.Name.Contains("ViewAll"))
                    .Select(p => new RolePermission
                    {
                        RoleId = employeeRole.Id,
                        PermissionId = p.Id,
                        CreatedAt = DateTime.UtcNow
                    }).ToList();

                // إضافة صلاحية طلب الإجازة
                var leaveCreatePermission = allPermissions.FirstOrDefault(p => p.Name == "Leaves.Create");
                if (leaveCreatePermission != null)
                {
                    employeePermissions.Add(new RolePermission
                    {
                        RoleId = employeeRole.Id,
                        PermissionId = leaveCreatePermission.Id,
                        CreatedAt = DateTime.UtcNow
                    });
                }

                await context.RolePermissions.AddRangeAsync(employeePermissions);
            }

            await context.SaveChangesAsync();
        }
    }
}
