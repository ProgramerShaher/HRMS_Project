using Microsoft.EntityFrameworkCore;
using HRMS.Core.Entities.Core;
using HRMS.Core.Entities.Personnel;
using Microsoft.EntityFrameworkCore.Infrastructure;
using HRMS.Core.Entities.Leaves;
using HRMS.Core.Entities.Payroll;
using HRMS.Core.Entities.Performance;
using HRMS.Core.Entities.Recruitment;
using HRMS.Core.Entities.Identity;
using HRMS.Core.Entities.Attendance;
using HRMS.Core.Entities.Accounting;

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

    /// <summary>
    /// سجلات التدقيق
    /// </summary>
    DbSet<AuditLog> AuditLogs { get; }

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
    /// أرصدة إجازات الموظفين (Alias)
    /// </summary>
    DbSet<EmployeeLeaveBalance> EmployeeLeaveBalances { get; }
    
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

    /// <summary>
    /// سجل تاريخ اعتماد الإجازات
    /// </summary>
    DbSet<LeaveApprovalHistory> LeaveApprovalHistory { get; }

    /// <summary>
    /// الموافقات على سير العمل
    /// </summary>
    DbSet<WorkflowApproval> WorkflowApprovals { get; }

    #endregion

    #region Attendance Entities

    /// <summary>
    /// جداول الموظفين
    /// </summary>
    DbSet<EmployeeRoster> EmployeeRosters { get; }

    /// <summary>
    /// الحضور اليومي
    /// </summary>
    DbSet<DailyAttendance> DailyAttendances { get; }

    /// <summary>
    /// سجلات البصمة الخام
    /// </summary>
    DbSet<RawPunchLog> RawPunchLogs { get; }

    /// <summary>
    /// أنواع المناوبات
    /// </summary>
    DbSet<ShiftType> ShiftTypes { get; }

    /// <summary>
    /// تصحيحات الحضور
    /// </summary>
    DbSet<AttendanceCorrection> AttendanceCorrections { get; }

    /// <summary>
    /// طلبات تبديل المناوبات
    /// </summary>
    DbSet<ShiftSwapRequest> ShiftSwapRequests { get; }

    /// <summary>
    /// طلبات العمل الإضافي
    /// </summary>
    DbSet<OvertimeRequest> OvertimeRequests { get; }

    /// <summary>
    /// سياسات الحضور
    /// </summary>
    DbSet<AttendancePolicy> AttendancePolicies { get; }

    /// <summary>
    /// فترات الروستر
    /// </summary>
    /// <summary>
    /// طلبات الاستئذان
    /// </summary>
    DbSet<PermissionRequest> PermissionRequests { get; }

    #endregion

    #region Performance Entities

    /// <summary>
    /// المخالفات الإدارية
    /// </summary>
    DbSet<EmployeeViolation> EmployeeViolations { get; }

    /// <summary>
    /// لائحة الجزاءات
    /// </summary>
    DbSet<DisciplinaryAction> DisciplinaryActions { get; }

    /// <summary>
    /// تقييمات الموظفين
    /// </summary>
    DbSet<EmployeeAppraisal> EmployeeAppraisals { get; }

    /// <summary>
    /// تفاصيل التقييمات
    /// </summary>
    DbSet<AppraisalDetail> AppraisalDetails { get; }

    /// <summary>
    /// مكتبة مؤشرات الأداء
    /// </summary>
    DbSet<KpiLibrary> KpiLibraries { get; }

    /// <summary>
    /// أنواع المخالفات
    /// </summary>
    DbSet<ViolationType> ViolationTypes { get; }

    /// <summary>
    /// فترات التقييم
    /// </summary>
    DbSet<AppraisalCycle> AppraisalCycles { get; }

    #endregion

    #region Recruitment Entities

    /// <summary>
    /// المرشحين
    /// </summary>
    DbSet<Candidate> Candidates { get; }

    /// <summary>
    /// طلبات التوظيف
    /// </summary>
    DbSet<JobApplication> JobApplications { get; }

    /// <summary>
    /// عروض العمل
    /// </summary>
    DbSet<JobOffer> JobOffers { get; }

    /// <summary>
    /// الوظائف الشاغرة
    /// </summary>
    DbSet<JobVacancy> JobVacancies { get; }

    /// <summary>
    /// المقابلات
    /// </summary>
    DbSet<Interview> Interviews { get; }

    #endregion

    #region Payroll Entities

    /// <summary>
    /// عناصر الراتب
    /// </summary>
    DbSet<SalaryElement> SalaryElements { get; }

    /// <summary>
    /// هياكل رواتب الموظفين
    /// </summary>
    DbSet<EmployeeSalaryStructure> SalaryStructures { get; }

    /// <summary>
    /// مسيرات الرواتب
    /// </summary>
    DbSet<PayrollRun> PayrollRuns { get; }

    /// <summary>
    /// تفاصيل قسائم الرواتب
    /// </summary>
    DbSet<PayslipDetail> PayslipDetails { get; }

    /// <summary>
    /// السلف
    /// </summary>
    DbSet<Loan> Loans { get; }

    /// <summary>
    /// أقساط السلف
    /// </summary>
    DbSet<LoanInstallment> LoanInstallments { get; }

    /// <summary>
    /// قسائم الرواتب
    /// </summary>
    DbSet<Payslip> Payslips { get; }

    /// <summary>
    /// هيكل رواتب الموظفين
    /// </summary>
    DbSet<EmployeeSalaryStructure> EmployeeSalaryStructures { get; }

    /// <summary>
    /// التعديلات على الرواتب (مكافآت/خصومات)
    /// </summary>
    DbSet<PayrollAdjustment> PayrollAdjustments { get; }

    #endregion

    #region Accounting Entities

    /// <summary>
    /// الحسابات - دليل الحسابات
    /// </summary>
    DbSet<Account> Accounts { get; }

    /// <summary>
    /// قيود اليومية
    /// </summary>
    DbSet<JournalEntry> JournalEntries { get; }

    /// <summary>
    /// سطور قيود اليومية
    /// </summary>
    DbSet<JournalEntryLine> JournalEntryLines { get; }

    #endregion

    #region Common Entities

    /// <summary>
    /// التنبيهات والإشعارات
    /// </summary>
    DbSet<HRMS.Core.Entities.Notifications.Notification> Notifications { get; }

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
