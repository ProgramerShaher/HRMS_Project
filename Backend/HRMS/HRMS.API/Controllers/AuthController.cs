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
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var result = await _authService.RegisterAsync(request);

            if (!result.Success)
            {
                return BadRequest(new Result<AuthResponse>
                {
                    Succeeded = false,
                    Message = result.Message
                });
            }

            return Ok(new Result<AuthResponse>
            {
                Data = result.Data,
                Succeeded = true,
                Message = result.Message
            });
        }

        /// <summary>
        /// تسجيل الدخول
        /// </summary>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var result = await _authService.LoginAsync(request);

            if (!result.Success)
            {
                return Unauthorized(new Result<AuthResponse>
                {
                    Succeeded = false,
                    Message = result.Message
                });
            }

            return Ok(new Result<AuthResponse>
            {
                Data = result.Data,
                Succeeded = true,
                Message = result.Message
            });
        }

        /// <summary>
        /// تحديث الرمز (Refresh Token)
        /// </summary>
        [HttpPost("refresh-token")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken([FromBody] string refreshToken)
        {
            var result = await _authService.RefreshTokenAsync(refreshToken);

            if (!result.Success)
            {
                return Unauthorized(new Result<AuthResponse>
                {
                    Succeeded = false,
                    Message = result.Message
                });
            }

            return Ok(new Result<AuthResponse>
            {
                Data = result.Data,
                Succeeded = true,
                Message = result.Message
            });
        }

        /// <summary>
        /// تسجيل الخروج
        /// </summary>
        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized(new Result<object>
                {
                    Succeeded = false,
                    Message = "المستخدم غير مصرح له"
                });
            }

            var userId = int.Parse(userIdClaim.Value);
            var result = await _authService.LogoutAsync(userId);

            if (!result)
            {
                return BadRequest(new Result<object>
                {
                    Succeeded = false,
                    Message = "فشل تسجيل الخروج"
                });
            }

            return Ok(new Result<object>
            {
                Succeeded = true,
                Message = "تم تسجيل الخروج بنجاح"
            });
        }

        /// <summary>
        /// الحصول على معلومات المستخدم الحالي
        /// </summary>
        [HttpGet("me")]
        [Authorize]
        public IActionResult GetCurrentUser()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var userName = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;
            var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
            var roles = User.FindAll(System.Security.Claims.ClaimTypes.Role).Select(c => c.Value).ToList();

            return Ok(new Result<object>
            {
                Data = new
                {
                    UserId = userId,
                    UserName = userName,
                    Email = email,
                    Roles = roles
                },
                Succeeded = true,
                Message = "تم جلب معلومات المستخدم بنجاح"
            });
        }

        /// <summary>
        /// إضافة دور لمستخدم (Admin فقط)
        /// </summary>
        [HttpPost("add-role")]
        [Authorize(Roles = "System_Admin")]
        public async Task<IActionResult> AddUserToRole([FromQuery] int userId, [FromQuery] string roleName)
        {
            var result = await _authService.AddUserToRoleAsync(userId, roleName);

            if (!result)
            {
                return BadRequest(new Result<object>
                {
                    Succeeded = false,
                    Message = "فشل إضافة الدور"
                });
            }

            return Ok(new Result<object>
            {
                Succeeded = true,
                Message = $"تم إضافة الدور {roleName} بنجاح"
            });
        }

        /// <summary>
        /// إزالة دور من مستخدم (Admin فقط)
        /// </summary>
        [HttpPost("remove-role")]
        [Authorize(Roles = "System_Admin")]
        public async Task<IActionResult> RemoveUserFromRole([FromQuery] int userId, [FromQuery] string roleName)
        {
            var result = await _authService.RemoveUserFromRoleAsync(userId, roleName);

            if (!result)
            {
                return BadRequest(new Result<object>
                {
                    Succeeded = false,
                    Message = "فشل إزالة الدور"
                });
            }

            return Ok(new Result<object>
            {
                Succeeded = true,
                Message = $"تم إزالة الدور {roleName} بنجاح"
            });
        }

        /// <summary>
        /// الحصول على أدوار مستخدم
        /// </summary>
        [HttpGet("user-roles/{userId}")]
        [Authorize(Roles = "System_Admin,HR_Manager")]
        public async Task<IActionResult> GetUserRoles(int userId)
        {
            var roles = await _authService.GetUserRolesAsync(userId);

            return Ok(new Result<List<string>>
            {
                Data = roles,
                Succeeded = true,
                Message = "تم جلب الأدوار بنجاح"
            });
        }
    }
}
