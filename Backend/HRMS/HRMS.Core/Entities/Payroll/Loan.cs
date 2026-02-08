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

        /// <summary>
        /// حالة القرض: PENDING (قيد الانتظار), APPROVED (موافق عليه), ACTIVE (نشط), SETTLED (مسدد مبكراً), CLOSED (مغلق)
        /// </summary>
        [MaxLength(20)]
        public string? Status { get; set; } = "PENDING";

        /// <summary>
        /// تاريخ الموافقة على القرض
        /// </summary>
        public DateTime? ApprovalDate { get; set; }

        /// <summary>
        /// معرف الموظف الذي وافق على القرض
        /// </summary>
        [Column("APPROVED_BY")]
        public int? ApprovedBy { get; set; }

        /// <summary>
        /// تاريخ التسوية المبكرة (إن وجدت)
        /// </summary>
        public DateTime? SettlementDate { get; set; }

        /// <summary>
        /// ملاحظات التسوية
        /// </summary>
        [MaxLength(500)]
        public string? SettlementNotes { get; set; }

        // Navigation Properties
        public virtual Employee Employee { get; set; } = null!;
        public virtual ICollection<LoanInstallment> Installments { get; set; } = new List<LoanInstallment>();
    }
}
