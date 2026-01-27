using System.ComponentModel.DataAnnotations;
using HRMS.Application.DTOs.Leaves;

namespace HRMS.Application.DTOs.Leaves;

/// <summary>
/// نقل بيانات طلب الإجازة
/// </summary>
public class LeaveRequestDto
{
    public int RequestId { get; set; }
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;

    public int LeaveTypeId { get; set; }
    public string LeaveTypeName { get; set; } = string.Empty;

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int DaysCount { get; set; }
    public string? Reason { get; set; }
    public string? AttachmentPath { get; set; }
    public string Status { get; set; } = "PENDING";
    public string? RejectionReason { get; set; }
    
    public DateTime CreatedAt { get; set; }
}
