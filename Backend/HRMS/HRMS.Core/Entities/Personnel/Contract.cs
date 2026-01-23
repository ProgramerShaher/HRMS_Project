using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HRMS.Core.Entities.Common;

namespace HRMS.Core.Entities.Personnel
{
    /// <summary>
    /// كيان العقود - يحتوي على تفاصيل عقود الموظفين
    /// </summary>
    [Table("CONTRACTS", Schema = "HR_PERSONNEL")]
    public class Contract : BaseEntity
    {
        /// <summary>
        /// المعرف الفريد للعقد
        /// </summary>
        [Key]
        [Column("CONTRACT_ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ContractId { get; set; }

        /// <summary>
        /// معرف الموظف صاحب العقد
        /// </summary>
        [Required(ErrorMessage = "الموظف مطلوب")]
        [Column("EMPLOYEE_ID")]
        [ForeignKey(nameof(Employee))]
        public int EmployeeId { get; set; }

        /// <summary>
        /// نوع العقد (FULL_TIME, PART_TIME, LOCUM, VISITING)
        /// </summary>
        [MaxLength(50)]
        [Column("CONTRACT_TYPE")]
        public string? ContractType { get; set; }

        /// <summary>
        /// تاريخ بداية العقد
        /// </summary>
        [Required(ErrorMessage = "تاريخ بداية العقد مطلوب")]
        [Column("START_DATE")]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// تاريخ نهاية العقد
        /// </summary>
        [Column("END_DATE")]
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// هل العقد قابل للتجديد (1=نعم، 0=لا)
        /// </summary>
        [Column("IS_RENEWABLE")]
        public byte IsRenewable { get; set; } = 1;

        /// <summary>
        /// الراتب الأساسي
        /// </summary>
        [Required(ErrorMessage = "الراتب الأساسي مطلوب")]
        [Column("BASIC_SALARY", TypeName = "decimal(10, 2)")]
        public decimal BasicSalary { get; set; }

        /// <summary>
        /// بدل السكن
        /// </summary>
        [Column("HOUSING_ALLOWANCE", TypeName = "decimal(10, 2)")]
        public decimal HousingAllowance { get; set; } = 0;

        /// <summary>
        /// بدل المواصلات
        /// </summary>
        [Column("TRANSPORT_ALLOWANCE", TypeName = "decimal(10, 2)")]
        public decimal TransportAllowance { get; set; } = 0;

        /// <summary>
        /// بدلات أخرى
        /// </summary>
        [Column("OTHER_ALLOWANCES", TypeName = "decimal(10, 2)")]
        public decimal OtherAllowances { get; set; } = 0;

        /// <summary>
        /// رصيد الإجازة السنوي (بالأيام)
        /// </summary>
        [Column("VACATION_DAYS")]
        public short VacationDays { get; set; } = 30;

        /// <summary>
        /// عدد ساعات العمل اليومية
        /// </summary>
        [Column("WORKING_HOURS_DAILY")]
        public byte WorkingHoursDaily { get; set; } = 8;

        /// <summary>
        /// حالة العقد (ACTIVE, EXPIRED, TERMINATED)
        /// </summary>
        [MaxLength(20)]
        [Column("CONTRACT_STATUS")]
        public string? ContractStatus { get; set; } = "ACTIVE";

        // ═══════════════════════════════════════════════════════════
        // Navigation Properties - العلاقات
        // ═══════════════════════════════════════════════════════════

        /// <summary>
        /// الموظف صاحب العقد
        /// </summary>
        public virtual Employee Employee { get; set; } = null!;

        /// <summary>
        /// سجل تجديدات العقد
        /// </summary>
        public virtual ICollection<ContractRenewal> Renewals { get; set; } = new List<ContractRenewal>();
    }
}
