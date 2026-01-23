using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HRMS.Core.Entities.Common;

namespace HRMS.Core.Entities.Performance
{
    /// <summary>
    /// كيان دورات التقييم - تحديد الفترات الزمنية للتقييم السنوي
    /// </summary>
    [Table("APPRAISAL_CYCLES", Schema = "HR_PERFORMANCE")]
    public class AppraisalCycle : BaseEntity
    {
        [Key]
        [Column("CYCLE_ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CycleId { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("CYCLE_NAME_AR")]
        public string CycleNameAr { get; set; } = string.Empty;

        [Required]
        [Column("START_DATE")]
        public DateTime StartDate { get; set; }

        [Required]
        [Column("END_DATE")]
        public DateTime EndDate { get; set; }

        [Column("IS_ACTIVE")]
        public byte IsActive { get; set; } = 1;

        // Navigation Properties
        public virtual ICollection<EmployeeAppraisal> Appraisals { get; set; } = new List<EmployeeAppraisal>();
    }
}
