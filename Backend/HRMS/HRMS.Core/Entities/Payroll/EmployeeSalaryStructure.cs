using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HRMS.Core.Entities.Common;
using HRMS.Core.Entities.Personnel;

namespace HRMS.Core.Entities.Payroll
{
    /// <summary>
    /// كيان هيكل راتب الموظف - يربط الموظف بعناصر الراتب
    /// </summary>
    [Table("EMPLOYEE_SALARY_STRUCTURE", Schema = "HR_PAYROLL")]
    public class EmployeeSalaryStructure : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int StructureId { get; set; }

        [Required]
        [Column("EMPLOYEE_ID")]
        [ForeignKey(nameof(Employee))]
        public int EmployeeId { get; set; }

        [Required]
        [Column("ELEMENT_ID")]
        [ForeignKey(nameof(SalaryElement))]
        public int ElementId { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal Amount { get; set; } = 0;

        [Column(TypeName = "decimal(5, 2)")]
        public decimal Percentage { get; set; } = 0;

        public byte IsActive { get; set; } = 1;

        // Navigation Properties
        public virtual Employee Employee { get; set; } = null!;
        public virtual SalaryElement SalaryElement { get; set; } = null!;
    }
}
