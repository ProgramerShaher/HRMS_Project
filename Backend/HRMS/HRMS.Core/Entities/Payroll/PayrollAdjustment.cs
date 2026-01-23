using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HRMS.Core.Entities.Common;
using HRMS.Core.Entities.Personnel;

namespace HRMS.Core.Entities.Payroll
{
    /// <summary>
    /// كيان التعديلات على الرواتب - المكافآت أو الخصومات اليدوية
    /// </summary>
    [Table("PAYROLL_ADJUSTMENTS", Schema = "HR_PAYROLL")]
    public class PayrollAdjustment : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AdjustmentId { get; set; }

        [Required]
        [Column("EMPLOYEE_ID")]
        [ForeignKey(nameof(Employee))]
        public int EmployeeId { get; set; }

        [Column("PAYROLL_RUN_ID")]
        [ForeignKey(nameof(PayrollRun))]
        public int? PayrollRunId { get; set; }

        [MaxLength(20)]
        public string? AdjustmentType { get; set; } // HONORARIUM, DEDUCTION, CORRECTION

        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal Amount { get; set; }

        [Required]
        [MaxLength(200)]
        public string Reason { get; set; } = string.Empty;

        public int? ApprovedBy { get; set; }

        // Navigation Properties
        public virtual Employee Employee { get; set; } = null!;
        public virtual PayrollRun? PayrollRun { get; set; }
    }
}
