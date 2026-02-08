using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HRMS.Core.Entities.Common;

namespace HRMS.Core.Entities.Performance
{
    /// <summary>
    /// كيان مكتبة مؤشرات الأداء - تعريف الأهداف والمؤشرات (KPIs)
    /// </summary>
    [Table("KPI_LIBRARIES", Schema = "HR_PERFORMANCE")]
    public class KpiLibrary : BaseEntity
    {
        [Key]
        [Column("KPI_ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int KpiId { get; set; }

        [Required]
        [MaxLength(200)]
        [Column("KPI_NAME_AR")]
        public string KpiNameAr { get; set; } = string.Empty;

        [MaxLength(500)]
        [Column("KPI_DESCRIPTION")]
        public string? KpiDescription { get; set; }

        [MaxLength(50)]
        [Column("CATEGORY")]
        public string? Category { get; set; }

        [MaxLength(20)]
        [Column("MEASUREMENT_UNIT")]
        public string? MeasurementUnit { get; set; }

        // Added Weight property to support weighted scoring logic
        [Column("WEIGHT", TypeName = "decimal(5, 2)")]
        public decimal Weight { get; set; } = 1.0m;

        // Navigation Properties
        public virtual ICollection<AppraisalDetail> AppraisalDetails { get; set; } = new List<AppraisalDetail>();
    }
}
