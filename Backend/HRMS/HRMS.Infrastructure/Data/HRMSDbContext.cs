using Microsoft.EntityFrameworkCore;
using HRMS.Core.Entities.Core;
using HRMS.Core.Entities.Personnel;
using HRMS.Core.Entities.Attendance;
using HRMS.Core.Entities.Leaves;
using HRMS.Core.Entities.Payroll;
using HRMS.Core.Entities.Recruitment;
using HRMS.Core.Entities.Performance;

namespace HRMS.Infrastructure.Data
{
    public class HRMSDbContext : DbContext
    {
        public HRMSDbContext(DbContextOptions<HRMSDbContext> options) : base(options)
        {
        }

        // ===================================
        // HR_CORE Schema
        // ===================================
        public DbSet<Country> Countries { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Job> Jobs { get; set; }
        public DbSet<JobGrade> JobGrades { get; set; }
        public DbSet<DocumentType> DocumentTypes { get; set; }
        public DbSet<Bank> Banks { get; set; }
        public DbSet<SystemSetting> SystemSettings { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<WorkflowApproval> WorkflowApprovals { get; set; }
        public DbSet<ReportTemplate> ReportTemplates { get; set; }

        // ===================================
        // HR_PERSONNEL Schema
        // ===================================
        public DbSet<Employee> Employees { get; set; }
        public DbSet<EmployeeDocument> EmployeeDocuments { get; set; }
        public DbSet<Contract> Contracts { get; set; }
        public DbSet<ContractRenewal> ContractRenewals { get; set; }
        public DbSet<Dependent> Dependents { get; set; }
        public DbSet<EmployeeQualification> Qualifications { get; set; }
        public DbSet<EmployeeExperience> Experiences { get; set; }
        public DbSet<EmployeeCertification> Certifications { get; set; }
        public DbSet<EmployeeAddress> Addresses { get; set; }
        public DbSet<EmergencyContact> EmergencyContacts { get; set; }
        public DbSet<EmployeeBankAccount> BankAccounts { get; set; }

        // ===================================
        // HR_ATTENDANCE Schema
        // ===================================
        public DbSet<ShiftType> ShiftTypes { get; set; }
        public DbSet<RosterPeriod> RosterPeriods { get; set; }
        public DbSet<EmployeeRoster> EmployeeRosters { get; set; }
        public DbSet<RawPunchLog> RawPunchLogs { get; set; }
        public DbSet<DailyAttendance> DailyAttendances { get; set; }
        public DbSet<ShiftSwapRequest> ShiftSwapRequests { get; set; }
        public DbSet<OvertimeRequest> OvertimeRequests { get; set; }
        public DbSet<AttendancePolicy> AttendancePolicies { get; set; }

        // ===================================
        // HR_LEAVES Schema
        // ===================================
        public DbSet<LeaveType> LeaveTypes { get; set; }
        public DbSet<EmployeeLeaveBalance> LeaveBalances { get; set; }
        public DbSet<LeaveRequest> LeaveRequests { get; set; }
        public DbSet<PublicHoliday> PublicHolidays { get; set; }
        public DbSet<LeaveAccrualRule> LeaveAccrualRules { get; set; }
        public DbSet<LeaveEncashment> LeaveEncashments { get; set; }
        public DbSet<LeaveTransaction> LeaveTransactions { get; set; }

        // ===================================
        // HR_PAYROLL Schema
        // ===================================
        public DbSet<SalaryElement> SalaryElements { get; set; }
        public DbSet<EmployeeSalaryStructure> SalaryStructures { get; set; }
        public DbSet<PayrollRun> PayrollRuns { get; set; }
        public DbSet<Payslip> Payslips { get; set; }
        public DbSet<PayslipDetail> PayslipDetails { get; set; }
        public DbSet<Loan> Loans { get; set; }
        public DbSet<LoanInstallment> LoanInstallments { get; set; }
        public DbSet<EndOfServiceCalc> EndOfServiceCalcs { get; set; }
        public DbSet<PayrollAdjustment> PayrollAdjustments { get; set; }

        // ===================================
        // HR_RECRUITMENT Schema
        // ===================================
        public DbSet<JobVacancy> JobVacancies { get; set; }
        public DbSet<Candidate> Candidates { get; set; }
        public DbSet<JobApplication> JobApplications { get; set; }
        public DbSet<Interview> Interviews { get; set; }
        public DbSet<JobOffer> JobOffers { get; set; }

        // ===================================
        // HR_PERFORMANCE Schema
        // ===================================
        public DbSet<KpiLibrary> KpiLibraries { get; set; }
        public DbSet<AppraisalCycle> AppraisalCycles { get; set; }
        public DbSet<EmployeeAppraisal> EmployeeAppraisals { get; set; }
        public DbSet<AppraisalDetail> AppraisalDetails { get; set; }
        public DbSet<ViolationType> ViolationTypes { get; set; }
        public DbSet<DisciplinaryAction> DisciplinaryActions { get; set; }
        public DbSet<EmployeeViolation> EmployeeViolations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // يمكنك إضافة تكوينات إضافية هنا (Fluent API) إذا لزم الأمر
            // ولكننا اعتمدنا بشكل كبير على Data Annotations في الـ Entities
        }
    }
}
