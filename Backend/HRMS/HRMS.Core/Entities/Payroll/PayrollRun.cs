using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HRMS.Core.Entities.Common;

namespace HRMS.Core.Entities.Payroll
{
    /// <summary>
    /// كيان مسير الرواتب - يمثل عملية صرف راتب شهرية
    /// </summary>
    [Table("PAYROLL_RUNS", Schema = "HR_PAYROLL")]
    public class PayrollRun : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RunId { get; set; }

        [Required]
        public short Year { get; set; }

        [Required]
        public byte Month { get; set; }

        public DateTime RunDate { get; set; } = DateTime.Now;

        [MaxLength(20)]
        public string? Status { get; set; } = "DRAFT"; // DRAFT, APPROVED, PAID

        [Column(TypeName = "decimal(15, 2)")]
        public decimal? TotalPayout { get; set; }

        [MaxLength(200)]
        public string? Notes { get; set; }

        // Navigation Properties
        public virtual ICollection<Payslip> Payslips { get; set; } = new List<Payslip>();
    }
}
