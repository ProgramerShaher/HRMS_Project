using System.ComponentModel.DataAnnotations;

namespace HRMS.Application.DTOs.Leaves;

/// <summary>
/// نقل بيانات أنواع الإجازات
/// </summary>
public class LeaveTypeDto
{
    /// <summary>
    /// معرف نوع الإجازة
    /// </summary>
    public int LeaveTypeId { get; set; }

    /// <summary>
    /// الاسم بالعربية
    /// </summary>
    [Required]
    public string LeaveNameAr { get; set; } = string.Empty;

    /// <summary>
    /// الاسم بالإنجليزية
    /// </summary>
    public string? NameEn { get; set; } // User requested NameEn in DTO, checking Entity again... Entity doesn't have NameEn. I must add it to Entity too if requested "Fields: Id, NameAr, NameEn...".
    // Wait, the user request says: "Fields: Id, NameAr, NameEn, DefaultDays, and CRITICAL FIELD: IsDeductible (bool)."
    // The Entity currently lacks NameEn and DefaultDays. I need to add those to Entity first!

    /// <summary>
    /// عدد الأيام الافتراضي
    /// </summary>
    public int DefaultDays { get; set; }

    /// <summary>
    /// هل يخصم من الرصيد
    /// </summary>
    public bool IsDeductible { get; set; }
}
