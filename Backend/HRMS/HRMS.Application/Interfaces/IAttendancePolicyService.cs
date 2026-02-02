using HRMS.Application.DTOs.Attendance;

namespace HRMS.Application.Interfaces;

/// <summary>
/// Interface for attendance policy retrieval service.
/// Provides methods to fetch policies with department-specific and default fallback logic.
/// </summary>
public interface IAttendancePolicyService
{
    /// <summary>
    /// Retrieves attendance policy for a specific employee.
    /// </summary>
    Task<AttendancePolicyDto?> GetPolicyForEmployeeAsync(int employeeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves attendance policy for a specific department.
    /// </summary>
    Task<AttendancePolicyDto?> GetPolicyForDepartmentAsync(int? deptId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the default attendance policy.
    /// </summary>
    Task<AttendancePolicyDto?> GetDefaultPolicyAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Clears all cached policies.
    /// </summary>
    void ClearCache();
}
