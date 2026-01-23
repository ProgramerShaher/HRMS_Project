using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HRMS.Core.Entities.Common;
using HRMS.Core.Entities.Personnel;

namespace HRMS.Core.Entities.Payroll
{
    /// <summary>
    /// كيان السلف (القروض) - إدارة السلف الممنوحة للموظفين
    /// </summary>
    [Table("LOANS", Schema = "HR_PAYROLL")]
    public class Loan : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LoanId { get; set; }

        [Required]
        [Column("EMPLOYEE_ID")]
        [ForeignKey(nameof(Employee))]
        public int EmployeeId { get; set; }

        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal LoanAmount { get; set; }

        public DateTime RequestDate { get; set; } = DateTime.Now;

        [Required]
        public short InstallmentCount { get; set; }

        [MaxLength(20)]
        public string? Status { get; set; } = "ACTIVE"; // PENDING, ACTIVE, CLOSED

        // Navigation Properties
        public virtual Employee Employee { get; set; } = null!;
        public virtual ICollection<LoanInstallment> Installments { get; set; } = new List<LoanInstallment>();
    }
}
