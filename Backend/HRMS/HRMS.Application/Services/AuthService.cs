using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using HRMS.Application.DTOs.Auth;
using HRMS.Application.Interfaces;
using HRMS.Application.Settings;
using HRMS.Core.Entities.Identity;

namespace HRMS.Application.Services
{
    /// <summary>
    /// خدمة المصادقة والتفويض (Authentication & Authorization Service)
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly JwtSettings _jwtSettings;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            SignInManager<ApplicationUser> signInManager,
            IOptions<JwtSettings> jwtSettings)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _jwtSettings = jwtSettings.Value;
        }

        #region Register - التسجيل

        /// <summary>
        /// تسجيل مستخدم جديد
        /// </summary>
        public async Task<(bool Success, string Message, AuthResponse? Data)> RegisterAsync(RegisterRequest request)
        {
            // التحقق من تطابق كلمة المرور
            if (request.Password != request.ConfirmPassword)
            {
                return (false, "كلمة المرور وتأكيد كلمة المرور غير متطابقين", null);
            }

            // التحقق من عدم وجود المستخدم مسبقاً
            var existingUser = await _userManager.FindByNameAsync(request.UserName);
            if (existingUser != null)
            {
                return (false, "اسم المستخدم موجود مسبقاً", null);
            }

            var existingEmail = await _userManager.FindByEmailAsync(request.Email);
            if (existingEmail != null)
            {
                return (false, "البريد الإلكتروني مسجل مسبقاً", null);
            }

            // إنشاء المستخدم الجديد
            var user = new ApplicationUser
            {
                UserName = request.UserName,
                Email = request.Email,
                FullNameAr = request.FullNameAr,
                FullNameEn = request.FullNameEn,
                PhoneNumber = request.PhoneNumber,
                EmployeeId = request.EmployeeId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                EmailConfirmed = true // يمكن تغييره حسب المتطلبات
            };

            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return (false, $"فشل إنشاء المستخدم: {errors}", null);
            }

            // إضافة الدور الافتراضي
            await _userManager.AddToRoleAsync(user, "Employee");

            // إنشاء الرمز
            var authResponse = await GenerateAuthResponseAsync(user);

            return (true, "تم التسجيل بنجاح", authResponse);
        }

        #endregion

        #region Login - تسجيل الدخول

        /// <summary>
        /// تسجيل الدخول
        /// </summary>
        public async Task<(bool Success, string Message, AuthResponse? Data)> LoginAsync(LoginRequest request)
        {
            // البحث عن المستخدم
            var user = await _userManager.FindByNameAsync(request.UserName) 
                       ?? await _userManager.FindByEmailAsync(request.UserName);

            if (user == null)
            {
                return (false, "اسم المستخدم أو كلمة المرور غير صحيحة", null);
            }

            // التحقق من أن الحساب نشط
            if (!user.IsActive)
            {
                return (false, "الحساب معطل، يرجى الاتصال بالإدارة", null);
            }

            // التحقق من كلمة المرور
            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);

            if (result.IsLockedOut)
            {
                return (false, "الحساب مقفل مؤقتاً بسبب محاولات دخول فاشلة متعددة", null);
            }

            if (!result.Succeeded)
            {
                return (false, "اسم المستخدم أو كلمة المرور غير صحيحة", null);
            }

            // تحديث آخر تسجيل دخول
            user.LastLoginAt = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);

            // إنشاء الرمز
            var authResponse = await GenerateAuthResponseAsync(user);

            return (true, "تم تسجيل الدخول بنجاح", authResponse);
        }

        #endregion

        #region Refresh Token - تحديث الرمز

        /// <summary>
        /// تحديث الرمز باستخدام Refresh Token
        /// </summary>
        public async Task<(bool Success, string Message, AuthResponse? Data)> RefreshTokenAsync(string refreshToken)
        {
            var user = _userManager.Users.FirstOrDefault(u => u.RefreshToken == refreshToken);

            if (user == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return (false, "رمز التحديث غير صالح أو منتهي الصلاحية", null);
            }

            var authResponse = await GenerateAuthResponseAsync(user);

            return (true, "تم تحديث الرمز بنجاح", authResponse);
        }

        #endregion

        #region Logout - تسجيل الخروج

        /// <summary>
        /// تسجيل الخروج
        /// </summary>
        public async Task<bool> LogoutAsync(int userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) return false;

            // إلغاء Refresh Token
            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;
            await _userManager.UpdateAsync(user);

            await _signInManager.SignOutAsync();

            return true;
        }

        #endregion

        #region Role Management - إدارة الأدوار

        /// <summary>
        /// إضافة دور لمستخدم
        /// </summary>
        public async Task<bool> AddUserToRoleAsync(int userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) return false;

            if (!await _roleManager.RoleExistsAsync(roleName))
                return false;

            var result = await _userManager.AddToRoleAsync(user, roleName);
            return result.Succeeded;
        }

        /// <summary>
        /// إزالة دور من مستخدم
        /// </summary>
        public async Task<bool> RemoveUserFromRoleAsync(int userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) return false;

            var result = await _userManager.RemoveFromRoleAsync(user, roleName);
            return result.Succeeded;
        }

        /// <summary>
        /// الحصول على أدوار المستخدم
        /// </summary>
        public async Task<List<string>> GetUserRolesAsync(int userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) return new List<string>();

            var roles = await _userManager.GetRolesAsync(user);
            return roles.ToList();
        }

        #endregion

        #region Private Methods - الدوال الخاصة

        /// <summary>
        /// إنشاء استجابة المصادقة مع JWT Token
        /// </summary>
        private async Task<AuthResponse> GenerateAuthResponseAsync(ApplicationUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var token = await GenerateJwtTokenAsync(user, roles.ToList());
            var refreshToken = GenerateRefreshToken();

            // حفظ Refresh Token
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays);
            await _userManager.UpdateAsync(user);

            return new AuthResponse
            {
                UserId = user.Id,
                UserName = user.UserName!,
                Email = user.Email!,
                FullName = user.FullNameAr ?? user.FullNameEn,
                Token = token,
                RefreshToken = refreshToken,
                TokenExpiration = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
                Roles = roles.ToList(),
                EmployeeId = user.EmployeeId
            };
        }

        /// <summary>
        /// إنشاء JWT Token
        /// </summary>
        private async Task<string> GenerateJwtTokenAsync(ApplicationUser user, List<string> roles)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName!),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            // إضافة الأدوار
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            };

            // إضافة EmployeeId إذا كان موجوداً
            if (user.EmployeeId.HasValue)
            {
                claims.Add(new Claim("EmployeeId", user.EmployeeId.Value.ToString()));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// إنشاء Refresh Token عشوائي
        /// </summary>
        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        #endregion
    }
}
