using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HRMS.Core.Entities.Common;
using HRMS.Core.Entities.Personnel;

namespace HRMS.Core.Entities.Payroll
{
    /// <summary>
    /// كيان قسيمة الراتب - تفاصيل راتب الموظف في شهر معين
    /// </summary>
    [Table("PAYSLIPS", Schema = "HR_PAYROLL")]
    public class Payslip : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long PayslipId { get; set; }

        [Required]
        [Column("RUN_ID")]
        [ForeignKey(nameof(PayrollRun))]
        public int RunId { get; set; }

        [Required]
        [Column("EMPLOYEE_ID")]
        [ForeignKey(nameof(Employee))]
        public int EmployeeId { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal? BasicSalary { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal? TotalAllowances { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal? TotalDeductions { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal? NetSalary { get; set; }

        // --- New Deduction Details ---
        [Column("TOTAL_VIOLATIONS", TypeName = "decimal(10, 2)")]
        public decimal TotalViolations { get; set; } = 0;

        [Column("OTHER_DEDUCTIONS", TypeName = "decimal(10, 2)")]
        public decimal OtherDeductions { get; set; } = 0;

        // --- Attendance & OT Breakdown ---
        [Column("TOTAL_LATE_MINUTES")]
        public int TotalLateMinutes { get; set; }

        [Column("ABSENCE_DAYS")]
        public int AbsenceDays { get; set; }

        [Column("TOTAL_OT_MINUTES")]
        public int TotalOvertimeMinutes { get; set; }

        [Column("OT_EARNINGS", TypeName = "decimal(10, 2)")]
        public decimal OvertimeEarnings { get; set; }

        // Navigation Properties
        public virtual PayrollRun PayrollRun { get; set; } = null!;
        public virtual Employee Employee { get; set; } = null!;
        public virtual ICollection<PayslipDetail> Details { get; set; } = new List<PayslipDetail>();
    }
}
