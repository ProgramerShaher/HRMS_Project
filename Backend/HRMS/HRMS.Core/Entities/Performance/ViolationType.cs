using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HRMS.Core.Entities.Common;

namespace HRMS.Core.Entities.Performance
{
    /// <summary>
    /// كيان أنواع المخالفات - تصنيف المخالفات حسب الجسامة
    /// </summary>
    [Table("VIOLATION_TYPES", Schema = "HR_PERFORMANCE")]
    public class ViolationType : BaseEntity
    {
        [Key]
        [Column("VIOLATION_TYPE_ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ViolationTypeId { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("VIOLATION_NAME_AR")]
        public string ViolationNameAr { get; set; } = string.Empty;

        [Column("SEVERITY_LEVEL")]
        public byte SeverityLevel { get; set; } = 1; // 1=بسيط، 2=متوسط، 3=جسيم

        [MaxLength(500)]
        [Column("DESCRIPTION")]
        public string? Description { get; set; }

        // Navigation Properties
        public virtual ICollection<EmployeeViolation> Violations { get; set; } = new List<EmployeeViolation>();
    }
}
