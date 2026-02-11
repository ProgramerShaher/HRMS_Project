using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HRMS.Core.Entities.Common;

namespace HRMS.Core.Entities.Payroll
{
    /// <summary>
    /// كيان عناصر الراتب - البدلات والخصومات
    /// </summary>
    [Table("SALARY_ELEMENTS", Schema = "HR_PAYROLL")]
    public class SalaryElement : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ELEMENT_ID")]
        public int ElementId { get; set; }

        [Required(ErrorMessage = "اسم العنصر مطلوب")]
        [MaxLength(100, ErrorMessage = "الاسم لا يمكن أن يتجاوز 100 حرف")]
        [Column("ELEMENT_NAME_AR")]
        public string ElementNameAr { get; set; } = string.Empty;

        [MaxLength(10)]
        [Column("ELEMENT_TYPE")]
        public string? ElementType { get; set; } // EARNING, DEDUCTION

        [Column("IS_TAXABLE")]
        public byte IsTaxable { get; set; } = 0;

        [Column("IS_GOSI_BASE")]
        public byte IsGosiBase { get; set; } = 0;

        [Column("DEFAULT_PERCENTAGE", TypeName = "decimal(5, 2)")]
        public decimal? DefaultPercentage { get; set; }

        [Column("IS_RECURRING")]
        public byte IsRecurring { get; set; } = 1;

        [Column("IS_BASIC")]
        public byte IsBasic { get; set; } = 0; // NEW: Flag for Basic Salary

        // Navigation Properties
        public virtual ICollection<EmployeeSalaryStructure> EmployeeStructures { get; set; } = new List<EmployeeSalaryStructure>();
    }
}
