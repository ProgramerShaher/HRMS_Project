using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HRMS.Core.Entities.Common;
using HRMS.Core.Entities.Core;

namespace HRMS.Core.Entities.Personnel
{
    /// <summary>
    /// كيان الموظفين - يمثل الجدول الرئيسي في النظام
    /// يحتوي على البيانات الشخصية، الوظيفية، ومعلومات الاتصال
    /// </summary>
    [Table("EMPLOYEES", Schema = "HR_PERSONNEL")]
    public class Employee : BaseEntity
    {
        /// <summary>
        /// المعرف الفريد للموظف
        /// </summary>
        [Key]
        [Column("EMPLOYEE_ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EmployeeId { get; set; }

        /// <summary>
        /// الرقم الوظيفي (فريد)
        /// </summary>
        [Required(ErrorMessage = "الرقم الوظيفي مطلوب")]
        [MaxLength(20, ErrorMessage = "الرقم الوظيفي لا يمكن أن يتجاوز 20 حرف")]
        [Column("EMPLOYEE_NUMBER")]
        public string EmployeeNumber { get; set; } = string.Empty;

        /// <summary>
        /// الاسم الأول بالعربية
        /// </summary>
        [Required(ErrorMessage = "الاسم الأول بالعربية مطلوب")]
        [MaxLength(50)]
        [Column("FIRST_NAME_AR")]
        public string FirstNameAr { get; set; } = string.Empty;

        /// <summary>
        /// الاسم الثاني (الأب) بالعربية
        /// </summary>
        [MaxLength(50)]
        [Column("SECOND_NAME_AR")]
        public string? SecondNameAr { get; set; }

        /// <summary>
        /// الاسم الثالث (الجد) بالعربية
        /// </summary>
        [MaxLength(50)]
        [Column("THIRD_NAME_AR")]
        public string? ThirdNameAr { get; set; }

        /// <summary>
        /// اسم العائلة بالعربية
        /// </summary>
        [Required(ErrorMessage = "اسم العائلة بالعربية مطلوب")]
        [MaxLength(50)]
        [Column("HIJRI_LAST_NAME_AR")]
        public string HijriLastNameAr { get; set; } = string.Empty;

        /// <summary>
        /// الاسم الكامل بالإنجليزية
        /// </summary>
        [Required(ErrorMessage = "الاسم الكامل بالإنجليزية مطلوب")]
        [MaxLength(200)]
        [Column("FULL_NAME_EN")]
        public string FullNameEn { get; set; } = string.Empty;

        /// <summary>
        /// الجنس (M=ذكر، F=أنثى)
        /// </summary>
        [MaxLength(1)]
        [Column("GENDER")]
        public string? Gender { get; set; }

        /// <summary>
        /// تاريخ الميلاد
        /// </summary>
        [Required(ErrorMessage = "تاريخ الميلاد مطلوب")]
        [Column("BIRTH_DATE")]
        public DateTime BirthDate { get; set; }

        /// <summary>
        /// الحالة الاجتماعية
        /// </summary>
        [MaxLength(20)]
        [Column("MARITAL_STATUS")]
        public string? MaritalStatus { get; set; }

        /// <summary>
        /// معرف الجنسية (FK)
        /// </summary>
        [Required(ErrorMessage = "الجنسية مطلوبة")]
        [Column("NATIONALITY_ID")]
        [ForeignKey(nameof(Nationality))]
        public int NationalityId { get; set; }

        /// <summary>
        /// معرف الوظيفة (FK)
        /// </summary>
        [Required(ErrorMessage = "الوظيفة مطلوبة")]
        [Column("JOB_ID")]
        [ForeignKey(nameof(Job))]
        public int JobId { get; set; }

        /// <summary>
        /// معرف القسم (FK)
        /// </summary>
        [Required(ErrorMessage = "القسم مطلوب")]
        [Column("DEPT_ID")]
        [ForeignKey(nameof(Department))]
        public int DeptId { get; set; }

        /// <summary>
        /// معرف المدير المباشر (FK)
        /// </summary>
        [Column("MANAGER_ID")]
        [ForeignKey(nameof(Manager))]
        public int? ManagerId { get; set; }

        /// <summary>
        /// تاريخ الالتحاق
        /// </summary>
        [Required(ErrorMessage = "تاريخ الالتحاق مطلوب")]
        [Column("JOINING_DATE")]
        public DateTime JoiningDate { get; set; }

        /// <summary>
        /// حالة الموظف (ACTIVE, ON_LEAVE, TERMINATED, RESIGNED)
        /// </summary>
        [MaxLength(20)]
        [Column("STATUS")]
        public string? Status { get; set; } = "ACTIVE";

        /// <summary>
        /// البريد الإلكتروني
        /// </summary>
        [EmailAddress(ErrorMessage = "البريد الإلكتروني غير صحيح")]
        [MaxLength(100)]
        [Column("EMAIL")]
        public string? Email { get; set; }

        /// <summary>
        /// رقم الجوال
        /// </summary>
        [MaxLength(20)]
        [Column("MOBILE")]
        public string? Mobile { get; set; }

        // ═══════════════════════════════════════════════════════════
        // Navigation Properties - العلاقات
        // ═══════════════════════════════════════════════════════════

        /// <summary>
        /// الدولة (الجنسية)
        /// </summary>
        public virtual Country? Nationality { get; set; }

        /// <summary>
        /// الوظيفة الحالية
        /// </summary>
        public virtual Job? Job { get; set; }

        /// <summary>
        /// القسم الحالي
        /// </summary>
        public virtual Department? Department { get; set; }

        /// <summary>
        /// المدير المباشر
        /// </summary>
        public virtual Employee? Manager { get; set; }

        /// <summary>
        /// الموظفين التابعين لهذا المدير
        /// </summary>
        [InverseProperty(nameof(Manager))]
        public virtual ICollection<Employee> Subordinates { get; set; } = new List<Employee>();

        /// <summary>
        /// وثائق الموظف
        /// </summary>
        public virtual ICollection<EmployeeDocument> Documents { get; set; } = new List<EmployeeDocument>();

        /// <summary>
        /// العقود الوظيفية
        /// </summary>
        public virtual ICollection<Contract> Contracts { get; set; } = new List<Contract>();

        /// <summary>
        /// التابعين (المعالين)
        /// </summary>
        public virtual ICollection<Dependent> Dependents { get; set; } = new List<Dependent>();

        /// <summary>
        /// المؤهلات العلمية
        /// </summary>
        public virtual ICollection<EmployeeQualification> Qualifications { get; set; } = new List<EmployeeQualification>();

        /// <summary>
        /// الخبرات العملية
        /// </summary>
        public virtual ICollection<EmployeeExperience> Experiences { get; set; } = new List<EmployeeExperience>();

        /// <summary>
        /// العناوين
        /// </summary>
        public virtual ICollection<EmployeeAddress> Addresses { get; set; } = new List<EmployeeAddress>();

        /// <summary>
        /// جهات الاتصال للطوارئ
        /// </summary>
        public virtual ICollection<EmergencyContact> EmergencyContacts { get; set; } = new List<EmergencyContact>();

        /// <summary>
        /// الحسابات البنكية
        /// </summary>
        public virtual ICollection<EmployeeBankAccount> BankAccounts { get; set; } = new List<EmployeeBankAccount>();
    }
}
