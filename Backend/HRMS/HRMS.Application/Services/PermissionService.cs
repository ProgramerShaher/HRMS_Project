using HRMS.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using HRMS.Core.Entities.Identity;

namespace HRMS.Application.Services
{
    /// <summary>
    /// خدمة إدارة الصلاحيات (Permission Service)
    /// </summary>
    public class PermissionService : IPermissionService
    {
        private readonly IApplicationDbContext _context;

        public PermissionService(IApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// الحصول على جميع صلاحيات المستخدم من خلال أدواره
        /// </summary>
        public async Task<List<string>> GetUserPermissionsAsync(int userId)
        {
            var permissions = await _context.UserRoles
                .Where(ur => ur.UserId == userId)
                .Join(_context.RolePermissions,
                    ur => ur.RoleId,
                    rp => rp.RoleId,
                    (ur, rp) => rp.PermissionId)
                .Join(_context.Permissions,
                    permissionId => permissionId,
                    p => p.Id,
                    (permissionId, p) => p.Name)
                .Distinct()
                .ToListAsync();

            return permissions;
        }

        /// <summary>
        /// الحصول على صلاحيات دور معين
        /// </summary>
        public async Task<List<string>> GetRolePermissionsAsync(int roleId)
        {
            var permissions = await _context.RolePermissions
                .Where(rp => rp.RoleId == roleId)
                .Join(_context.Permissions,
                    rp => rp.PermissionId,
                    p => p.Id,
                    (rp, p) => p.Name)
                .ToListAsync();

            return permissions;
        }

        /// <summary>
        /// التحقق من امتلاك المستخدم لصلاحية معينة
        /// </summary>
        public async Task<bool> UserHasPermissionAsync(int userId, string permission)
        {
            var hasPermission = await _context.UserRoles
                .Where(ur => ur.UserId == userId)
                .Join(_context.RolePermissions,
                    ur => ur.RoleId,
                    rp => rp.RoleId,
                    (ur, rp) => rp.PermissionId)
                .Join(_context.Permissions,
                    permissionId => permissionId,
                    p => p.Id,
                    (permissionId, p) => p.Name)
                .AnyAsync(p => p == permission);

            return hasPermission;
        }

        public async Task<List<ApplicationRole>> GetAllRolesAsync()
        {
            return await _context.Roles.ToListAsync();
        }

        public async Task<ApplicationRole?> GetRoleByIdAsync(int roleId)
        {
            return await _context.Roles.FindAsync(roleId);
        }

        public async Task<bool> CreateRoleAsync(string roleName, string? description)
        {
            var role = new ApplicationRole { Name = roleName, Description = description };
            role.NormalizedName = roleName.ToUpper();
            
            await _context.Roles.AddAsync(role);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateRoleAsync(int roleId, string roleName, string? description)
        {
            var role = await _context.Roles.FindAsync(roleId);
            if (role == null) return false;

            role.Name = roleName;
            role.NormalizedName = roleName.ToUpper();
            role.Description = description;

            _context.Roles.Update(role);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteRoleAsync(int roleId)
        {
            var role = await _context.Roles.FindAsync(roleId);
            if (role == null) return false;

            _context.Roles.Remove(role);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<List<Permission>> GetAllPermissionsAsync()
        {
            return await _context.Permissions.ToListAsync();
        }

        public async Task<bool> UpdateRolePermissionsAsync(int roleId, List<int> permissionIds)
        {
            var role = await _context.Roles.FindAsync(roleId);
            if (role == null) return false;

            // Remove existing permissions
            var existingPermissions = await _context.RolePermissions
                .Where(rp => rp.RoleId == roleId)
                .ToListAsync();
            
            _context.RolePermissions.RemoveRange(existingPermissions);

            // Add new permissions
            var newPermissions = permissionIds.Select(pid => new RolePermission
            {
                RoleId = roleId,
                PermissionId = pid
            });

            await _context.RolePermissions.AddRangeAsync(newPermissions);

            return await _context.SaveChangesAsync() > 0;
        }
    }
}
