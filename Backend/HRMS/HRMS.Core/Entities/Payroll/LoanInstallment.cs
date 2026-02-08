using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HRMS.Core.Entities.Common;

namespace HRMS.Core.Entities.Payroll
{
    /// <summary>
    /// كيان أقساط القروض - تفاصيل سداد السلف
    /// </summary>
    [Table("LOAN_INSTALLMENTS", Schema = "HR_PAYROLL")]
    public class LoanInstallment : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long InstallmentId { get; set; }

        [Required]
        [Column("LOAN_ID")]
        [ForeignKey(nameof(Loan))]
        public int LoanId { get; set; }

        [Required]
        public short InstallmentNumber { get; set; }

        [Required]
        public DateTime DueDate { get; set; }

        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal Amount { get; set; }

        /// <summary>
        /// حالة القسط: UNPAID (غير مدفوع), PAID (مدفوع عبر الرواتب), SETTLED_MANUALLY (مسدد يدوياً)
        /// </summary>
        [MaxLength(20)]
        public string Status { get; set; } = "UNPAID";

        /// <summary>
        /// تاريخ الدفع الفعلي
        /// </summary>
        public DateTime? PaidDate { get; set; }

        /// <summary>
        /// معرف مسير الرواتب الذي تم فيه الخصم
        /// </summary>
        [Column("PAID_IN_PAYROLL_RUN")]
        public int? PaidInPayrollRun { get; set; }

        /// <summary>
        /// ملاحظات التسوية اليدوية
        /// </summary>
        [MaxLength(500)]
        public string? SettlementNotes { get; set; }

        [Obsolete("Use Status field instead")]
        public byte IsPaid { get; set; } = 0;

        // Navigation Properties
        public virtual Loan Loan { get; set; } = null!;
    }
}
