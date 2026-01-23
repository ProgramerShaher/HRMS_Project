using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HRMS.Core.Entities.Common;

namespace HRMS.Core.Entities.Performance
{
    /// <summary>
    /// كيان لائحة الجزاءات - تعريف العقوبات المعتمدة
    /// </summary>
    [Table("DISCIPLINARY_ACTIONS", Schema = "HR_PERFORMANCE")]
    public class DisciplinaryAction : BaseEntity
    {
        [Key]
        [Column("ACTION_ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ActionId { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("ACTION_NAME_AR")]
        public string ActionNameAr { get; set; } = string.Empty;

        [Column("DEDUCTION_DAYS", TypeName = "decimal(3, 1)")]
        public decimal DeductionDays { get; set; } = 0;

        [Column("IS_TERMINATION")]
        public byte IsTermination { get; set; } = 0;

        // Navigation Properties
        public virtual ICollection<EmployeeViolation> Violations { get; set; } = new List<EmployeeViolation>();
    }
}
