using Microsoft.EntityFrameworkCore;
using HRMS.Core.Entities.Core;
using HRMS.Core.Entities.Personnel;
using Microsoft.EntityFrameworkCore.Infrastructure; // 👈 ضروري لاستخدام DatabaseFacade
using HRMS.Core.Entities.Leaves;
using HRMS.Core.Entities.Payroll;
using HRMS.Core.Entities.Performance;
using HRMS.Core.Entities.Recruitment;
using Microsoft.EntityFrameworkCore.Infrastructure; // 👈 ضروري جداً
using HRMS.Core.Entities.Identity;

namespace HRMS.Application.Interfaces;

/// <summary>
/// واجهة سياق قاعدة البيانات - تطبيق Dependency Inversion Principle
/// </summary>
/// <remarks>
/// هذه الواجهة تسمح لطبقة Application بالاستقلال عن Infrastructure
/// وتسهل عمل Unit Testing باستخدام Mock Objects
/// </remarks>
public interface IApplicationDbContext
{
    #region Core Entities

    /// <summary>
    /// الدول
    /// </summary>
    DbSet<Country> Countries { get; }

    /// <summary>
    /// المدن
    /// </summary>
    DbSet<City> Cities { get; }

    /// <summary>
    /// البنوك
    /// </summary>
    DbSet<Bank> Banks { get; }

    /// <summary>
    /// الفروع
    /// </summary>
    DbSet<Branch> Branches { get; }

    /// <summary>
    /// الأقسام
    /// </summary>
    DbSet<Department> Departments { get; }

    /// <summary>
    /// الوظائف
    /// </summary>
    DbSet<Job> Jobs { get; }

    /// <summary>
    /// الدرجات الوظيفية
    /// </summary>
    DbSet<JobGrade> JobGrades { get; }

    /// <summary>
    /// أنواع الوثائق
    /// </summary>
    DbSet<DocumentType> DocumentTypes { get; }

    /// <summary>
    /// إعدادات النظام
    /// </summary>
    DbSet<SystemSetting> SystemSettings { get; }

    #endregion

    #region Personnel Entities

    /// <summary>
    /// الموظفون
    /// </summary>
    DbSet<Employee> Employees { get; }

    /// <summary>
    /// الحسابات البنكية للموظفين
    /// </summary>
    DbSet<EmployeeBankAccount> EmployeeBankAccounts { get; }

    /// <summary>
    /// وثائق الموظفين
    /// </summary>
    DbSet<EmployeeDocument> EmployeeDocuments { get; }

    /// <summary>
    /// المعالون
    /// </summary>
    /// <summary>
    /// المعالون
    /// </summary>
    DbSet<Dependent> Dependents { get; }

    /// <summary>
    /// العقود
    /// </summary>
    DbSet<Contract> Contracts { get; }

    /// <summary>
    /// تجديد العقود
    /// </summary>
    DbSet<ContractRenewal> ContractRenewals { get; }

    /// <summary>
    /// المؤهلات العلمية
    /// </summary>
    DbSet<EmployeeQualification> Qualifications { get; }

    /// <summary>
    /// الخبرات السابقة
    /// </summary>
    DbSet<EmployeeExperience> Experiences { get; }

    /// <summary>
    /// الشهادات المهنية
    /// </summary>
    DbSet<EmployeeCertification> Certifications { get; }

    /// <summary>
    /// عناوين الموظفين
    /// </summary>
    DbSet<EmployeeAddress> Addresses { get; }

    /// <summary>
    /// جهات الاتصال للطوارئ
    /// </summary>
    DbSet<EmergencyContact> EmergencyContacts { get; }

    /// <summary>
    /// البدلات والبيانات المالية
    /// </summary>
    DbSet<EmployeeCompensation> EmployeeCompensations { get; }

    // EmployeeDocuments already exists (line 80) but let's double check

    #endregion

    #region Leaves Entities

    /// <summary>
    /// أنواع الإجازات
    /// </summary>
    DbSet<LeaveType> LeaveTypes { get; }

    /// <summary>
    /// أرصدة إجازات الموظفين
    /// </summary>
    DbSet<EmployeeLeaveBalance> LeaveBalances { get; }
    /// <summary>
    /// طلبات الإجازات
    /// </summary>
    DbSet<LeaveRequest> LeaveRequests { get; }

    /// <summary>
    /// العطل الرسمية
    /// </summary>
    DbSet<PublicHoliday> PublicHolidays { get; }

    /// <summary>
    /// حركات الإجازات
    /// </summary>
    DbSet<LeaveTransaction> LeaveTransactions { get; }

    #endregion

    #region Payroll Entities

    /// <summary>
    /// هياكل رواتب الموظفين
    /// </summary>
    DbSet<EmployeeSalaryStructure> SalaryStructures { get; }
    #endregion

    #region Performance Entities

    /// <summary>
    /// دورات التقييم
    /// </summary>
    DbSet<AppraisalCycle> AppraisalCycles { get; }

    /// <summary>
    /// تقييمات الموظفين
    /// </summary>
    DbSet<EmployeeAppraisal> EmployeeAppraisals { get; }

    #endregion

    #region Recruitment Entities

    /// <summary>
    /// الوظائف الشاغرة
    /// </summary>
    DbSet<JobVacancy> JobVacancies { get; }

    /// <summary>
    /// المتقدمون للوظائف
    /// </summary>
    DbSet<Candidate> Candidates { get; }

    #endregion

 
    #region Methods

    /// <summary>
    /// حفظ التغييرات في قاعدة البيانات (Async)
    /// </summary>
    /// <param name="cancellationToken">رمز الإلغاء</param>
    /// <returns>عدد السجلات المتأثرة</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// حفظ التغييرات في قاعدة البيانات (Sync)
    /// </summary>
    /// <returns>عدد السجلات المتأثرة</returns>
    int SaveChanges();

    #endregion

    DatabaseFacade Database { get; }
}
