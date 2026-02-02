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
        public int ElementId { get; set; }

        [Required(ErrorMessage = "اسم العنصر مطلوب")]
        [MaxLength(100, ErrorMessage = "الاسم لا يمكن أن يتجاوز 100 حرف")]
        public string ElementNameAr { get; set; } = string.Empty;

        [MaxLength(10)]
        public string? ElementType { get; set; } // EARNING, DEDUCTION

        public byte IsTaxable { get; set; } = 0;

        public byte IsGosiBase { get; set; } = 0;

        [Column(TypeName = "decimal(5, 2)")]
        public decimal? DefaultPercentage { get; set; }

        public byte IsRecurring { get; set; } = 1;

        public byte IsBasic { get; set; } = 0; // NEW: Flag for Basic Salary

        // Navigation Properties
        public virtual ICollection<EmployeeSalaryStructure> EmployeeStructures { get; set; } = new List<EmployeeSalaryStructure>();
    }
}
