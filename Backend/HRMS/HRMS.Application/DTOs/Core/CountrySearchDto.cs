namespace HRMS.Application.DTOs.Core;

/// <summary>
/// معايير البحث والفلترة للدول
/// </summary>
public class CountrySearchDto
{
    /// <summary>
    /// البحث في الاسم (عربي أو إنجليزي)
    /// </summary>
    public string? SearchTerm { get; set; }
    
    /// <summary>
    /// فلترة حسب رمز ISO
    /// </summary>
    public string? IsoCode { get; set; }
    
    /// <summary>
    /// فلترة حسب الحالة
    /// </summary>
    public bool? IsActive { get; set; }
    
    /// <summary>
    /// رقم الصفحة (يبدأ من 1)
    /// </summary>
    public int PageNumber { get; set; } = 1;
    
    /// <summary>
    /// عدد العناصر في الصفحة
    /// </summary>
    public int PageSize { get; set; } = 10;
    
    /// <summary>
    /// الترتيب حسب (CountryNameAr, CountryNameEn, IsoCode, CreatedAt)
    /// </summary>
    public string SortBy { get; set; } = "CountryNameAr";
    
    /// <summary>
    /// اتجاه الترتيب (asc, desc)
    /// </summary>
    public string SortDirection { get; set; } = "asc";
}
