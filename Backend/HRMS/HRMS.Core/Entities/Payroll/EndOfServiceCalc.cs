using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HRMS.Core.Entities.Common;
using HRMS.Core.Entities.Personnel;

namespace HRMS.Core.Entities.Payroll
{
    /// <summary>
    /// كيان مكافأة نهاية الخدمة - حساب مستحقات الموظف عند المغادرة
    /// </summary>
    [Table("END_OF_SERVICE_CALC", Schema = "HR_PAYROLL")]
    public class EndOfServiceCalc : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EosId { get; set; }

        [Required]
        [Column("EMPLOYEE_ID")]
        [ForeignKey(nameof(Employee))]
        public int EmployeeId { get; set; }

        [Required]
        public DateTime TerminationDate { get; set; }

        [Required]
        [Column(TypeName = "decimal(5, 2)")]
        public decimal ServiceYears { get; set; }

        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal LastBasicSalary { get; set; }

        [Required]
        [Column(TypeName = "decimal(12, 2)")]
        public decimal TotalAmount { get; set; }

        [MaxLength(500)]
        public string? CalculationNotes { get; set; }

        public byte IsPaid { get; set; } = 0;

        // Navigation Properties
        public virtual Employee Employee { get; set; } = null!;
    }
}
