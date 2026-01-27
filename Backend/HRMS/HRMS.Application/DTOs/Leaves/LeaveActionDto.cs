using System.ComponentModel.DataAnnotations;

namespace HRMS.Application.DTOs.Leaves;

/// <summary>
/// نقل بيانات إجراء الإجازة (موافقة/رفض)
/// </summary>
public class LeaveActionDto
{
    [Required]
    public int RequestId { get; set; }

    [Required]
    [RegularExpression("^(Approve|Reject)$", ErrorMessage = "Action must be Approve or Reject")]
    public string Action { get; set; } = string.Empty;

    public string? Comment { get; set; }

    /// <summary>
    /// ID of the Manager/HR performing the action (For manual input during dev)
    /// </summary>
    public int ManagerId { get; set; }
}
