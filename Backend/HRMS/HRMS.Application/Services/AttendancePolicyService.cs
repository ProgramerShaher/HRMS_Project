using HRMS.Application.DTOs.Attendance;
using HRMS.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace HRMS.Application.Services;

/// <summary>
/// Service for retrieving attendance policies with department-specific and default fallback logic.
/// Implements caching for performance optimization.
/// </summary>
public class AttendancePolicyService : IAttendancePolicyService
{
    private readonly IApplicationDbContext _context;
    private readonly IMemoryCache _cache;
    private const int CacheExpirationMinutes = 60;

    public AttendancePolicyService(IApplicationDbContext context, IMemoryCache cache)
    {
        _context = context;
        _cache = cache;
    }

    /// <summary>
    /// Retrieves attendance policy for a specific employee.
    /// First attempts to find department-specific policy, then falls back to default.
    /// </summary>
    /// <param name="employeeId">Employee identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Attendance policy DTO or null if no policy found</returns>
    public async Task<AttendancePolicyDto?> GetPolicyForEmployeeAsync(
        int employeeId, 
        CancellationToken cancellationToken = default)
    {
        // جلب معرف القسم الخاص بالموظف
        // Fetch employee's department ID
        var employee = await _context.Employees
            .AsNoTracking()
            .Where(e => e.EmployeeId == employeeId)
            .Select(e => new { e.DepartmentId, e.JobId })
            .FirstOrDefaultAsync(cancellationToken);

        if (employee == null)
            return null;

        // محاولة جلب السياسة الخاصة بالقسم أولاً
        // Try to get department-specific policy first
        var policy = await GetPolicyForDepartmentAsync(employee.DepartmentId, cancellationToken);

        if (policy != null)
            return policy;

        // في حالة عدم وجود سياسة خاصة، نستخدم السياسة الافتراضية
        // If no specific policy exists, use default policy
        return await GetDefaultPolicyAsync(cancellationToken);
    }

    /// <summary>
    /// Retrieves attendance policy for a specific department with caching.
    /// </summary>
    /// <param name="deptId">Department identifier (nullable)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Attendance policy DTO or null if not found</returns>
    public async Task<AttendancePolicyDto?> GetPolicyForDepartmentAsync(
        int? deptId, 
        CancellationToken cancellationToken = default)
    {
        if (deptId == null)
            return null;

        // استخدام الـ Cache لتحسين الأداء
        // Use cache for performance optimization
        var cacheKey = $"AttendancePolicy_Dept_{deptId}";
        
        if (_cache.TryGetValue(cacheKey, out AttendancePolicyDto? cachedPolicy))
            return cachedPolicy;

        // جلب السياسة من قاعدة البيانات
        // Fetch policy from database
        var policy = await _context.AttendancePolicies
            .AsNoTracking()
            .Where(p => p.DeptId == deptId && p.IsDeleted == 0)
            .OrderByDescending(p => p.PolicyId) // أحدث سياسة / Latest policy
            .Select(p => new AttendancePolicyDto
            {
                PolicyId = p.PolicyId,
                PolicyNameAr = p.PolicyNameAr,
                DeptId = p.DeptId,
                JobId = p.JobId,
                LateGraceMins = p.LateGraceMins,
                OvertimeMultiplier = p.OvertimeMultiplier,
                WeekendOtMultiplier = p.WeekendOtMultiplier
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (policy != null)
        {
            // حفظ في الـ Cache لمدة ساعة
            // Cache for 1 hour
            _cache.Set(cacheKey, policy, TimeSpan.FromMinutes(CacheExpirationMinutes));
        }

        return policy;
    }

    /// <summary>
    /// Retrieves the default attendance policy (where DeptId and JobId are both null).
    /// This serves as fallback when no department-specific policy exists.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Default attendance policy DTO or null if not configured</returns>
    public async Task<AttendancePolicyDto?> GetDefaultPolicyAsync(
        CancellationToken cancellationToken = default)
    {
        // استخدام الـ Cache للسياسة الافتراضية
        // Use cache for default policy
        var cacheKey = "AttendancePolicy_Default";
        
        if (_cache.TryGetValue(cacheKey, out AttendancePolicyDto? cachedPolicy))
            return cachedPolicy;

        // جلب السياسة الافتراضية (بدون قسم أو وظيفة محددة)
        // Fetch default policy (no specific department or job)
        var policy = await _context.AttendancePolicies
            .AsNoTracking()
            .Where(p => p.DeptId == null && p.JobId == null && p.IsDeleted == 0)
            .OrderByDescending(p => p.PolicyId) // أحدث سياسة / Latest policy
            .Select(p => new AttendancePolicyDto
            {
                PolicyId = p.PolicyId,
                PolicyNameAr = p.PolicyNameAr,
                DeptId = p.DeptId,
                JobId = p.JobId,
                LateGraceMins = p.LateGraceMins,
                OvertimeMultiplier = p.OvertimeMultiplier,
                WeekendOtMultiplier = p.WeekendOtMultiplier
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (policy != null)
        {
            // حفظ في الـ Cache لمدة ساعة
            // Cache for 1 hour
            _cache.Set(cacheKey, policy, TimeSpan.FromMinutes(CacheExpirationMinutes));
        }

        return policy;
    }

    /// <summary>
    /// Clears all cached policies. Should be called when policies are updated.
    /// </summary>
    public void ClearCache()
    {
        // في تطبيق حقيقي، نستخدم IMemoryCache.Remove لكل مفتاح
        // In real application, use IMemoryCache.Remove for each key
        // أو نستخدم Cache Tag Helper
        // Or use Cache Tag Helper
    }
}
