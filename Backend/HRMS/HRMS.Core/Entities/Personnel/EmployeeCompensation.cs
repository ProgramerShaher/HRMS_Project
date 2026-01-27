using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HRMS.Core.Entities.Common;
using HRMS.Core.Entities.Core;

namespace HRMS.Core.Entities.Personnel;

/// <summary>
/// البيانات المالية للموظف (1:1 مع Employee)
/// </summary>
[Table("EMPLOYEE_COMPENSATIONS", Schema = "HR_PERSONNEL")]
public class EmployeeCompensation : BaseEntity
{
    [Key]
    [Column("COMPENSATION_ID")]
    public int CompensationId { get; set; }

    [Column("EMPLOYEE_ID")]
    public int EmployeeId { get; set; }

    [Column("BASIC_SALARY", TypeName = "decimal(18,2)")]
    public decimal BasicSalary { get; set; }

    [Column("HOUSING_ALLOWANCE", TypeName = "decimal(18,2)")]
    public decimal HousingAllowance { get; set; }

    [Column("TRANSPORT_ALLOWANCE", TypeName = "decimal(18,2)")]
    public decimal TransportAllowance { get; set; }

    // بدلات المستشفيات الخاصة
    [Column("MEDICAL_ALLOWANCE", TypeName = "decimal(18,2)")]
    public decimal MedicalAllowance { get; set; } = 0; // بدل عدوى/خطر

    [Column("OTHER_ALLOWANCES", TypeName = "decimal(18,2)")]
    public decimal OtherAllowances { get; set; } = 0;

    [Column("BANK_ID")]
    public int? BankId { get; set; }

    [Column("IBAN_NUMBER")]
    [MaxLength(30)]
    public string? IbanNumber { get; set; }

    // Navigation
    [ForeignKey(nameof(EmployeeId))]
    public virtual Employee Employee { get; set; } = null!;

    [ForeignKey(nameof(BankId))]
    public virtual Bank? Bank { get; set; }
}
