using System.ComponentModel.DataAnnotations;

namespace HRMS.Application.DTOs.Payroll.Configuration;

public class SalaryElementDto
{
    public int ElementId { get; set; }

    [Required(ErrorMessage = "اسم العنصر مطلوب")]
    public string ElementNameAr { get; set; } = string.Empty;

    [Required(ErrorMessage = "نوع العنصر مطلوب")]
    public string ElementType { get; set; } = "EARNING"; // EARNING, DEDUCTION

    public bool IsTaxable { get; set; }
    public bool IsGosiBase { get; set; }
    public bool IsRecurring { get; set; }
    public bool IsBasic { get; set; } // NEW
    public decimal? DefaultPercentage { get; set; }
}
