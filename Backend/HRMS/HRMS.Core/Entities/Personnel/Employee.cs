using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HRMS.Core.Entities.Common;
using HRMS.Core.Entities.Core;

namespace HRMS.Core.Entities.Personnel;

/// <summary>
/// الكيان الأساسي للموظف (بيانات شخصية ووظيفية)
/// </summary>
[Table("EMPLOYEES", Schema = "HR_PERSONNEL")]
public class Employee : BaseEntity
{
    [Key]
    [Column("EMPLOYEE_ID")]
    public int EmployeeId { get; set; }

    [Column("EMPLOYEE_NUMBER")]
    [MaxLength(20)]
    public required string EmployeeNumber { get; set; } // رقم وظيفي فريد

    // --- البيانات الشخصية ---
    [Column("FIRST_NAME_AR")] 
    [MaxLength(50)]
    public required string FirstNameAr { get; set; }
    
    [Column("SECOND_NAME_AR")]
    [MaxLength(50)]
    public required string SecondNameAr { get; set; }
    
    [Column("THIRD_NAME_AR")]
    [MaxLength(50)]
    public required string ThirdNameAr { get; set; }

    [Column("LAST_NAME_AR")]
    [MaxLength(50)]
    public required string LastNameAr { get; set; }

    [Column("FULL_NAME_EN")]
    [MaxLength(200)]
    public required string FullNameEn { get; set; }

    /// <summary>
    /// الاسم الكامل باللغة العربية (Computed)
    /// </summary>
    [NotMapped]
    public string FullNameAr => string.Join(" ", new[] { FirstNameAr, SecondNameAr, ThirdNameAr, LastNameAr }.Where(s => !string.IsNullOrWhiteSpace(s)));

    [Column("BIRTH_DATE")]
    public DateTime BirthDate { get; set; }

    [Column("GENDER")]
    [MaxLength(1)] 
    public string Gender { get; set; } = "M"; // M/F

    [Column("MARITAL_STATUS")]
    [MaxLength(20)]
    public string MaritalStatus { get; set; } = "Single";

    [Column("MOBILE")]
    [MaxLength(20)] 
    public required string Mobile { get; set; }

    [Column("EMAIL")]
    [MaxLength(100)]
    public required string Email { get; set; }

    // --- البيانات الوظيفية ---
    [Column("HIRE_DATE")]
    public DateTime HireDate { get; set; }

    [Column("DEPARTMENT_ID")]
    public int DepartmentId { get; set; }

    [Column("JOB_ID")]
    public int JobId { get; set; }

    [Column("NATIONALITY_ID")] // نفترض وجود جدول للجنسيات مستقبلاً أو نستخدم الدولة حالياً
    public int? NationalityId { get; set; } 

    [Column("NATIONAL_ID")]
    [MaxLength(20)]
    public required string NationalId { get; set; }

    // --- البيانات الطبية Professional Details ---
    [Column("LICENSE_NUMBER")]
    [MaxLength(50)]
    public string? LicenseNumber { get; set; }

    [Column("LICENSE_EXPIRY_DATE")]
    public DateTime? LicenseExpiryDate { get; set; }

    [Column("SPECIALTY")]
    [MaxLength(100)]
    public string? Specialty { get; set; }

    [Column("MANAGER_ID")]
    public int? ManagerId { get; set; }

    [Column("USER_ID")] // للربط مع Identity
    [MaxLength(450)]
    public string? UserId { get; set; }

    // --- العلاقات Navigation Properties ---
    public virtual Department Department { get; set; } = null!;
    public virtual Job Job { get; set; } = null!;
    [ForeignKey(nameof(NationalityId))]
    public virtual Country? Country { get; set; }
    public virtual Employee? Manager { get; set; }

    // One-to-One
    public virtual EmployeeCompensation? Compensation { get; set; }

    // One-to-Many
    // One-to-Many
    public virtual ICollection<EmployeeDocument> Documents { get; set; } = new List<EmployeeDocument>();
    public virtual ICollection<EmployeeQualification> Qualifications { get; set; } = new List<EmployeeQualification>();
    public virtual ICollection<EmployeeExperience> Experiences { get; set; } = new List<EmployeeExperience>();
    public virtual ICollection<EmergencyContact> EmergencyContacts { get; set; } = new List<EmergencyContact>();
    public virtual ICollection<Contract> Contracts { get; set; } = new List<Contract>();
    public virtual ICollection<EmployeeCertification> Certifications { get; set; } = new List<EmployeeCertification>();
    public virtual ICollection<EmployeeBankAccount> BankAccounts { get; set; } = new List<EmployeeBankAccount>();
    public virtual ICollection<Dependent> Dependents { get; set; } = new List<Dependent>();
    public virtual ICollection<EmployeeAddress> Addresses { get; set; } = new List<EmployeeAddress>();
    public virtual ICollection<Employee> Subordinates { get; set; } = new List<Employee>();
}
