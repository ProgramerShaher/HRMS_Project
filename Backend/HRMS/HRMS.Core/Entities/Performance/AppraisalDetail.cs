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

        [Required]
        [Column("SCORE", TypeName = "decimal(5, 2)")]
        public decimal Score { get; set; }

        [MaxLength(200)]
        [Column("COMMENTS")]
        public string? Comments { get; set; }

        // Navigation Properties
        public virtual EmployeeAppraisal Appraisal { get; set; } = null!;
        public virtual KpiLibrary Kpi { get; set; } = null!;
    }
}
