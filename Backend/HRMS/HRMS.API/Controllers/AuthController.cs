using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HRMS.Application.DTOs.Auth;
using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;

namespace HRMS.API.Controllers
{
    /// <summary>
    /// تحكم المصادقة والتفويض (Authentication & Authorization)
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// تسجيل مستخدم جديد
        /// </summary>
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<Result<AuthResponse>>> Register([FromBody] RegisterRequest request)
        {
            var result = await _authService.RegisterAsync(request);

            if (!result.Succeeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// تسجيل الدخول
        /// </summary>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<Result<AuthResponse>>> Login([FromBody] LoginRequest request)
        {
            var result = await _authService.LoginAsync(request);

            if (!result.Succeeded)
            {
                return Unauthorized(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// تحديث الرمز (Refresh Token)
        /// </summary>
        [HttpPost("refresh-token")]
        [AllowAnonymous]
        public async Task<ActionResult<Result<AuthResponse>>> RefreshToken([FromBody] string refreshToken)
        {
            var result = await _authService.RefreshTokenAsync(refreshToken);

            if (!result.Succeeded)
            {
                return Unauthorized(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// تسجيل الخروج
        /// </summary>
        [HttpPost("logout")]
        [Authorize]
        public async Task<ActionResult<Result<bool>>> Logout()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized(Result<bool>.Failure("المستخدم غير مصرح له"));
            }

            var userId = int.Parse(userIdClaim.Value);
            var result = await _authService.LogoutAsync(userId);

            if (!result)
            {
                return BadRequest(Result<bool>.Failure("فشل تسجيل الخروج"));
            }

            return Ok(Result<bool>.Success(true, "تم تسجيل الخروج بنجاح"));
        }

        /// <summary>
        /// الحصول على معلومات المستخدم الحالي
        /// </summary>
        [HttpGet("me")]
        [Authorize]
        public ActionResult<Result<object>> GetCurrentUser()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var userName = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;
            var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
            var roles = User.FindAll(System.Security.Claims.ClaimTypes.Role).Select(c => c.Value).ToList();

            if (userId == null)
            {
                return Unauthorized(Result<object>.Failure("User not found in context"));
            }

            return Ok(Result<object>.Success(new
            {
                UserId = userId,
                UserName = userName,
                Email = email,
                Roles = roles
            }, "تم جلب معلومات المستخدم بنجاح"));
        }

        /// <summary>
        /// إضافة دور لمستخدم (Admin فقط)
        /// </summary>
        [HttpPost("add-role")]
        [Authorize(Roles = "System_Admin")]
        public async Task<ActionResult<Result<bool>>> AddUserToRole([FromQuery] int userId, [FromQuery] string roleName)
        {
            var result = await _authService.AddUserToRoleAsync(userId, roleName);

            if (!result)
            {
                return BadRequest(Result<bool>.Failure("فشل إضافة الدور"));
            }

            return Ok(Result<bool>.Success(true, $"تم إضافة الدور {roleName} بنجاح"));
        }

        /// <summary>
        /// إزالة دور من مستخدم (Admin فقط)
        /// </summary>
        [HttpPost("remove-role")]
        [Authorize(Roles = "System_Admin")]
        public async Task<ActionResult<Result<bool>>> RemoveUserFromRole([FromQuery] int userId, [FromQuery] string roleName)
        {
            var result = await _authService.RemoveUserFromRoleAsync(userId, roleName);

            if (!result)
            {
                return BadRequest(Result<bool>.Failure("فشل إزالة الدور"));
            }

            return Ok(Result<bool>.Success(true, $"تم إزالة الدور {roleName} بنجاح"));
        }

        /// <summary>
        /// الحصول على أدوار مستخدم
        /// </summary>
        [HttpGet("user-roles/{userId}")]
        [Authorize(Roles = "System_Admin,HR_Manager")]
        public async Task<ActionResult<Result<List<string>>>> GetUserRoles(int userId)
        {
            var roles = await _authService.GetUserRolesAsync(userId);
            return Ok(Result<List<string>>.Success(roles, "تم جلب الأدوار بنجاح"));
        }
    }
}
