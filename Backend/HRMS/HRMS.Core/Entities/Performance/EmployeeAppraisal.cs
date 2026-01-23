using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HRMS.Core.Entities.Common;
using HRMS.Core.Entities.Personnel;

namespace HRMS.Core.Entities.Performance
{
    /// <summary>
    /// كيان تقييم الموظف - سجل تقييم الأداء السنوي
    /// </summary>
    [Table("EMPLOYEE_APPRAISALS", Schema = "HR_PERFORMANCE")]
    public class EmployeeAppraisal : BaseEntity
    {
        [Key]
        [Column("APPRAISAL_ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AppraisalId { get; set; }

        [Required]
        [Column("EMPLOYEE_ID")]
        [ForeignKey(nameof(Employee))]
        public int EmployeeId { get; set; }

        [Required]
        [Column("CYCLE_ID")]
        [ForeignKey(nameof(Cycle))]
        public int CycleId { get; set; }

        [Required]
        [Column("EVALUATOR_ID")]
        [ForeignKey(nameof(Evaluator))]
        public int EvaluatorId { get; set; }

        [Column("APPRAISAL_DATE")]
        public DateTime AppraisalDate { get; set; } = DateTime.Now;

        [Column("FINAL_SCORE", TypeName = "decimal(5, 2)")]
        public decimal? FinalScore { get; set; }

        [MaxLength(20)]
        [Column("GRADE")]
        public string? Grade { get; set; }

        [MaxLength(20)]
        [Column("STATUS")]
        public string? Status { get; set; } = "DRAFT";

        [MaxLength(500)]
        [Column("EMPLOYEE_COMMENT")]
        public string? EmployeeComment { get; set; }

        // Navigation Properties
        public virtual Employee Employee { get; set; } = null!;
        public virtual AppraisalCycle Cycle { get; set; } = null!;
        public virtual Employee Evaluator { get; set; } = null!;
        public virtual ICollection<AppraisalDetail> Details { get; set; } = new List<AppraisalDetail>();
    }
}
