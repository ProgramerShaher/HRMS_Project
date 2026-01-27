using System;
using System.ComponentModel.DataAnnotations;

namespace HRMS.Application.DTOs.Leaves;

/// <summary>
/// نقل بيانات العطل الرسمية
/// </summary>
public class PublicHolidayDto
{
    /// <summary>
    /// معرف العطلة - فارغ عند الإنشاء
    /// </summary>
    public int? HolidayId { get; set; }

    /// <summary>
    /// اسم العطلة بالعربية
    /// </summary>
    [Required]
    public string HolidayNameAr { get; set; } = string.Empty;

    /// <summary>
    /// تاريخ البداية
    /// </summary>
    [Required]
    public DateTime StartDate { get; set; }

    /// <summary>
    /// تاريخ النهاية
    /// </summary>
    [Required]
    public DateTime EndDate { get; set; }

    /// <summary>
    /// السنة
    /// </summary>
    public short Year { get; set; }
}
