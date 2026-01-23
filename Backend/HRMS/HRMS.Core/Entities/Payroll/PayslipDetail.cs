using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HRMS.Core.Entities.Common;

namespace HRMS.Core.Entities.Payroll
{
    /// <summary>
    /// كيان تفاصيل قسيمة الراتب - تفاصيل كل بند في الراتب
    /// </summary>
    [Table("PAYSLIP_DETAILS", Schema = "HR_PAYROLL")]
    public class PayslipDetail : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long DetailId { get; set; }

        [Required]
        [Column("PAYSLIP_ID")]
        [ForeignKey(nameof(Payslip))]
        public long PayslipId { get; set; }

        [Required]
        [Column("ELEMENT_ID")]
        [ForeignKey(nameof(SalaryElement))]
        public int ElementId { get; set; }

        [MaxLength(100)]
        public string? ElementNameAr { get; set; }

        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal Amount { get; set; }

        [MaxLength(10)]
        public string? Type { get; set; } // EARNING, DEDUCTION

        // Navigation Properties
        public virtual Payslip Payslip { get; set; } = null!;
        public virtual SalaryElement SalaryElement { get; set; } = null!;
    }
}
