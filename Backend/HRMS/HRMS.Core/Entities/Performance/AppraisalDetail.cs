using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HRMS.Core.Entities.Common;

namespace HRMS.Core.Entities.Performance
{
    /// <summary>
    /// كيان تفاصيل التقييم - درجة كل مؤشر على حدة
    /// </summary>
    [Table("APPRAISAL_DETAILS", Schema = "HR_PERFORMANCE")]
    public class AppraisalDetail : BaseEntity
    {
        [Key]
        [Column("DETAIL_ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long DetailId { get; set; }

        [Required]
        [Column("APPRAISAL_ID")]
        [ForeignKey(nameof(Appraisal))]
        public int AppraisalId { get; set; }

        [Required]
        [Column("KPI_ID")]
        [ForeignKey(nameof(Kpi))]
        public int KpiId { get; set; }

        [Column("TARGET_VALUE", TypeName = "decimal(10, 2)")]
        public decimal? TargetValue { get; set; }

        [Column("ACTUAL_VALUE", TypeName = "decimal(10, 2)")]
        public decimal? ActualValue { get; set; }

        // Renaming/Upgrading the single Score field to support split scoring
        
        [Column("EMPLOYEE_SCORE", TypeName = "decimal(5, 2)")]
        public decimal EmployeeScore { get; set; }

        [Column("MANAGER_SCORE", TypeName = "decimal(5, 2)")]
        public decimal ManagerScore { get; set; }

        [Column("FINAL_SCORE", TypeName = "decimal(5, 2)")]
        public decimal FinalScore { get; set; }

        // Keeping Score for backward compatibility if needed, but logic should use FinalScore
        [NotMapped]
        public decimal Score 
        { 
            get => FinalScore; 
            set => FinalScore = value; 
        }

        [MaxLength(200)]
        [Column("COMMENTS")]
        public string? Comments { get; set; }

        // Navigation Properties
        public virtual EmployeeAppraisal Appraisal { get; set; } = null!;
        public virtual KpiLibrary Kpi { get; set; } = null!;
    }
}
