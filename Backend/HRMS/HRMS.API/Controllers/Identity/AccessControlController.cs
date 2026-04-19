using HRMS.Application.Interfaces;
using HRMS.Core.Entities.Identity;
using HRMS.Core.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.API.Controllers.Identity
{
    /// <summary>
    /// تحكم إدارة الصلاحيات والأدوار
    /// </summary>
    [Route("api/access-control")]
    [ApiController]
    [Authorize(Roles = "System_Admin")]
    public class AccessControlController : ControllerBase
    {
        private readonly IPermissionService _permissionService;
        private readonly IAuthService _authService;

        public AccessControlController(IPermissionService permissionService, IAuthService authService)
        {
            _permissionService = permissionService;
            _authService = authService;
        }

        #region Users Management

        /// <summary>
        /// الحصول على جميع المستخدمين
        /// </summary>
        [HttpGet("users")]
        public async Task<ActionResult<Result<List<ApplicationUser>>>> GetAllUsers()
        {
            var users = await _authService.GetAllUsersAsync();
            return Ok(Result<List<ApplicationUser>>.Success(users));
        }

        /// <summary>
        /// إضافة دور لمستخدم
        /// </summary>
        [HttpPost("users/{userId}/roles")]
        public async Task<ActionResult<Result<bool>>> AddUserToRole(int userId, [FromBody] string roleName)
        {
            var result = await _authService.AddUserToRoleAsync(userId, roleName);
            if (!result) return BadRequest(Result<bool>.Failure("فشل إضافة الدور للمستخدم"));
            return Ok(Result<bool>.Success(true, "تم إضافة الدور بنجاح"));
        }

        /// <summary>
        /// إزالة دور من مستخدم
        /// </summary>
        [HttpDelete("users/{userId}/roles/{roleName}")]
        public async Task<ActionResult<Result<bool>>> RemoveUserFromRole(int userId, string roleName)
        {
            var result = await _authService.RemoveUserFromRoleAsync(userId, roleName);
            if (!result) return BadRequest(Result<bool>.Failure("فشل إزالة الدور من المستخدم"));
            return Ok(Result<bool>.Success(true, "تم إزالة الدور بنجاح"));
        }

        #endregion

        #region Roles Management

        /// <summary>
        /// الحصول على جميع الأدوار
        /// </summary>
        [HttpGet("roles")]
        public async Task<ActionResult<Result<List<ApplicationRole>>>> GetAllRoles()
        {
            var roles = await _permissionService.GetAllRolesAsync();
            return Ok(Result<List<ApplicationRole>>.Success(roles));
        }

        /// <summary>
        /// الحصول على دور بواسطة المعرف
        /// </summary>
        [HttpGet("roles/{id}")]
        public async Task<ActionResult<Result<ApplicationRole>>> GetRoleById(int id)
        {
            var role = await _permissionService.GetRoleByIdAsync(id);
            if (role == null) return NotFound(Result<ApplicationRole>.Failure("لم يتم العثور على الدور"));
            return Ok(Result<ApplicationRole>.Success(role));
        }

        /// <summary>
        /// إنشاء دور جديد
        /// </summary>
        [HttpPost("roles")]
        public async Task<ActionResult<Result<bool>>> CreateRole([FromBody] CreateRoleDto dto)
        {
            var result = await _permissionService.CreateRoleAsync(dto.Name, dto.Description);
            if (!result) return BadRequest(Result<bool>.Failure("فشل إنشاء الدور"));
            return Ok(Result<bool>.Success(true, "تم إنشاء الدور بنجاح"));
        }

        /// <summary>
        /// تحديث دور
        /// </summary>
        [HttpPut("roles/{id}")]
        public async Task<ActionResult<Result<bool>>> UpdateRole(int id, [FromBody] UpdateRoleDto dto)
        {
            var result = await _permissionService.UpdateRoleAsync(id, dto.Name, dto.Description);
            if (!result) return BadRequest(Result<bool>.Failure("فشل تحديث الدور"));
            return Ok(Result<bool>.Success(true, "تم تحديث الدور بنجاح"));
        }

        /// <summary>
        /// حذف دور
        /// </summary>
        [HttpDelete("roles/{id}")]
        public async Task<ActionResult<Result<bool>>> DeleteRole(int id)
        {
            var result = await _permissionService.DeleteRoleAsync(id);
            if (!result) return BadRequest(Result<bool>.Failure("فشل حذف الدور"));
            return Ok(Result<bool>.Success(true, "تم حذف الدور بنجاح"));
        }

        #endregion

        #region Permissions Management

        /// <summary>
        /// الحصول على جميع الصلاحيات
        /// </summary>
        [HttpGet("permissions")]
        public async Task<ActionResult<Result<List<Permission>>>> GetAllPermissions()
        {
            var permissions = await _permissionService.GetAllPermissionsAsync();
            return Ok(Result<List<Permission>>.Success(permissions));
        }

        /// <summary>
        /// الحصول على صلاحيات دور معين
        /// </summary>
        [HttpGet("roles/{id}/permissions")]
        public async Task<ActionResult<Result<List<string>>>> GetRolePermissions(int id)
        {
            var permissions = await _permissionService.GetRolePermissionsAsync(id);
            return Ok(Result<List<string>>.Success(permissions));
        }

        /// <summary>
        /// تحديث صلاحيات دور
        /// </summary>
        [HttpPut("roles/{id}/permissions")]
        public async Task<ActionResult<Result<bool>>> UpdateRolePermissions(int id, [FromBody] List<int> permissionIds)
        {
            var result = await _permissionService.UpdateRolePermissionsAsync(id, permissionIds);
            if (!result) return BadRequest(Result<bool>.Failure("فشل تحديث الصلاحيات"));
            return Ok(Result<bool>.Success(true, "تم تحديث الصلاحيات بنجاح"));
        }

        #endregion
    }

    public class CreateRoleDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }

    public class UpdateRoleDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
