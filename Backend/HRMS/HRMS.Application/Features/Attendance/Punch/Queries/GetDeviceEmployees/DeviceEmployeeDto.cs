namespace HRMS.Application.Features.Attendance.Punch.Queries.GetDeviceEmployees;

public class DeviceEmployeeDto
{
    public int EmployeeId { get; set; }
    public string EmployeeNumber { get; set; } = string.Empty;
    public string FullNameAr { get; set; } = string.Empty;
    public string JobTitle { get; set; } = string.Empty;
    public string CurrentShift { get; set; } = string.Empty;
    public DateTime? LastPunchIn { get; set; }
    public DateTime? LastPunchOut { get; set; }
    public string Status { get; set; } = "Out"; // "In" or "Out"
}
