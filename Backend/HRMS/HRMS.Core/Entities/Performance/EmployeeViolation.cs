using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HRMS.Core.Entities.Common;
using HRMS.Core.Entities.Personnel;

namespace HRMS.Core.Entities.Performance
{
    /// <summary>
    /// كيان مخالفات الموظفين - سجل الجزاءات والمخالفات
    /// </summary>
    [Table("EMPLOYEE_VIOLATIONS", Schema = "HR_PERFORMANCE")]
    public class EmployeeViolation : BaseEntity
    {
        [Key]
        [Column("VIOLATION_ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ViolationId { get; set; }

        [Required]
        [Column("EMPLOYEE_ID")]
        [ForeignKey(nameof(Employee))]
        public int EmployeeId { get; set; }

        [Required]
        [Column("VIOLATION_TYPE_ID")]
        [ForeignKey(nameof(ViolationType))]
        public int ViolationTypeId { get; set; }

        [Required]
        [Column("VIOLATION_DATE")]
        public DateTime ViolationDate { get; set; } = DateTime.Now;

        [MaxLength(500)]
        [Column("DESCRIPTION")]
        public string? Description { get; set; }

        [Column("ACTION_ID")]
        [ForeignKey(nameof(Action))]
        public int? ActionId { get; set; }

        [Column("IS_EXECUTED")]
        public byte IsExecuted { get; set; } = 0;

        [MaxLength(20)]
        [Column("STATUS")]
        public string? Status { get; set; } = "PENDING"; // PENDING, INVESTIGATION, APPROVED, CANCELLED

        [Column("INVESTIGATION_NOTES")]
        public string? InvestigationNotes { get; set; }

        // Navigation Properties
        public virtual Employee Employee { get; set; } = null!;
        public virtual ViolationType ViolationType { get; set; } = null!;
        public virtual DisciplinaryAction? Action { get; set; }
    }
}
