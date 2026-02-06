using System;
using System.ComponentModel.DataAnnotations;

namespace HRMS.Application.DTOs.Personnel;

/// <summary>
/// كائن نقل البيانات لتجديد العقد
/// </summary>
public class RenewContractDto
{
    /// <summary>
    /// معرف العقد المراد تجديده
    /// </summary>
    [Required]
    public int ContractId { get; set; }

    /// <summary>
    /// تاريخ بداية العقد الجديد
    /// </summary>
    [Required]
    public DateTime NewStartDate { get; set; }

    /// <summary>
    /// تاريخ نهاية العقد الجديد
    /// </summary>
    [Required]
    public DateTime NewEndDate { get; set; }

    /// <summary>
    /// الراتب الأساسي الجديد (اختياري - إذا لم يتم تحديده يتم استخدام الراتب الحالي)
    /// </summary>
    [Range(0, double.MaxValue)]
    public decimal? NewBasicSalary { get; set; }

    /// <summary>
    /// بدل السكن الجديد (اختياري)
    /// </summary>
    [Range(0, double.MaxValue)]
    public decimal? NewHousingAllowance { get; set; }

    /// <summary>
    /// بدل المواصلات الجديد (اختياري)
    /// </summary>
    [Range(0, double.MaxValue)]
    public decimal? NewTransportAllowance { get; set; }

    /// <summary>
    /// البدلات الأخرى الجديدة (اختياري)
    /// </summary>
    [Range(0, double.MaxValue)]
    public decimal? NewOtherAllowances { get; set; }

    /// <summary>
    /// ملاحظات التجديد
    /// </summary>
    [MaxLength(500)]
    public string? Notes { get; set; }
}
