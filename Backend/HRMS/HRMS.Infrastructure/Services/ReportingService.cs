using HRMS.Application.DTOs.Reports.Analytics;
using HRMS.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using HRMS.Core.Entities.Attendance;
using System.Linq;

namespace HRMS.Infrastructure.Services;

public class ReportingService : IReportingService
{
    private readonly IApplicationDbContext _context;

    public ReportingService(IApplicationDbContext context)
    {
        _context = context;
    }

    // ═══════════════════════════════════════════════════════════
    // 1. HR & Personnel Analytics
    // ═══════════════════════════════════════════════════════════
    public async Task<AnalyticsHROverviewDto> GetHROverviewAsync()
    {
        var today = DateTime.UtcNow.Date;
        var firstDayOfMonth = new DateTime(today.Year, today.Month, 1);

        // 1. Headcount & Basics
        var totalEmployees = await _context.Employees
            .CountAsync(e => e.IsActive && e.TerminationDate == null);

        var totalDepartments = await _context.Departments.CountAsync();

        var newHires = await _context.Employees
            .CountAsync(e => e.HireDate >= firstDayOfMonth);

        // 2. Department Distribution
        var deptDist = await _context.Employees
            .Where(e => e.IsActive && e.TerminationDate == null)
            .GroupBy(e => e.Department != null ? e.Department.DeptNameAr : "غير معرف")
            .Select(g => new AnalyticsDepartmentDistributionDto
            {
                DepartmentName = g.Key,
                EmployeeCount = g.Count()
            })
            .ToListAsync();

        // 3. Document Expirations (Next 30 Days)
        var thirtyDaysFromNow = today.AddDays(30);
        var expirations = await _context.EmployeeDocuments
            .Include(d => d.Employee)
            .Include(d => d.DocumentType)
            .Where(d => d.ExpiryDate != null && d.ExpiryDate >= today && d.ExpiryDate <= thirtyDaysFromNow)
            .Select(d => new AnalyticsDocumentExpiryDto
            {
                EmployeeId = d.EmployeeId,
                EmployeeName = d.Employee.FirstNameAr + " " + d.Employee.LastNameAr,
                DocumentType = d.DocumentType != null ? d.DocumentType.DocumentTypeNameAr : "مستند",
                ExpiryDate = d.ExpiryDate!.Value,
                DaysRemaining = (d.ExpiryDate!.Value - today).Days
            })
            .OrderBy(d => d.DaysRemaining)
            .Take(10)
            .ToListAsync();

        return new AnalyticsHROverviewDto
        {
            TotalEmployees = totalEmployees,
            TotalDepartments = totalDepartments,
            NewHiresThisMonth = newHires,
            DepartmentDistribution = deptDist,
            UpcomingDocumentExpirations = expirations
        };
    }

    // ═══════════════════════════════════════════════════════════
    // 2. Attendance Analytics
    // ═══════════════════════════════════════════════════════════
    public async Task<AnalyticsAttendanceStatsDto> GetAttendanceStatsAsync(DateTime startDate, DateTime endDate)
    {
        var logs = await _context.DailyAttendances
            .Include(d => d.Employee)
            .ThenInclude(e => e.Department)
            .Where(d => d.AttendanceDate >= startDate && d.AttendanceDate <= endDate)
            .ToListAsync();

        var totalDays = (endDate - startDate).Days + 1;

        var totalPresent = logs.Count(l => (l.Status ?? "").ToUpper() == "PRESENT" || (l.Status ?? "").ToUpper() == "LATE");
        var totalAbsent = logs.Count(l => (l.Status ?? "").ToUpper() == "ABSENT");
        var totalLate = logs.Count(l => (l.Status ?? "").ToUpper() == "LATE");

        // 1. Daily Trend
        var trend = logs
            .GroupBy(l => l.AttendanceDate.Date)
            .Select(g => new AnalyticsDailyAttendanceSummaryDto
            {
                Date = g.Key,
                PresentCount = g.Count(x => (x.Status ?? "").ToUpper() == "PRESENT" || (x.Status ?? "").ToUpper() == "LATE"),
                AbsentCount = g.Count(x => (x.Status ?? "").ToUpper() == "ABSENT"),
                LateCount = g.Count(x => (x.Status ?? "").ToUpper() == "LATE")
            })
            .OrderBy(x => x.Date)
            .ToList();

        // 2. Top Late Employees
        var topLate = logs
            .Where(l => (l.Status ?? "").ToUpper() == "LATE" && l.LateMinutes > 0)
            .GroupBy(l => l.Employee)
            .Select(g => new AnalyticsEmployeeAttendanceRankingDto
            {
                EmployeeName = g.Key.FirstNameAr + " " + g.Key.LastNameAr,
                Department = g.Key.Department != null ? g.Key.Department.DeptNameAr : "-",
                Count = g.Sum(x => x.LateMinutes)
            })
            .OrderByDescending(x => x.Count)
            .Take(5)
            .ToList();

        return new AnalyticsAttendanceStatsDto
        {
            TotalWorkingDays = totalDays,
            TotalPresent = totalPresent,
            TotalAbsent = totalAbsent,
            TotalLate = totalLate,
            AbsenteeismRate = (totalPresent + totalAbsent) > 0 ? ((double)totalAbsent / (totalPresent + totalAbsent)) * 100 : 0,
            DailyTrend = trend,
            TopLateEmployees = topLate
        };
    }

    // ═══════════════════════════════════════════════════════════
    // 3. Payroll Analytics
    // ═══════════════════════════════════════════════════════════
    public async Task<AnalyticsPayrollStatsDto> GetPayrollStatsAsync(int month, int year)
    {
        var slips = await _context.Payslips
            .Include(p => p.Employee)
            .ThenInclude(e => e.Department)
            .Include(p => p.PayrollRun)
            .Where(p => p.PayrollRun.Month == month && p.PayrollRun.Year == year)
            .ToListAsync();

        var totalNet = slips.Sum(p => p.NetSalary ?? 0);
        var totalBasic = slips.Sum(p => p.BasicSalary ?? 0);
        var totalAllowances = slips.Sum(p => p.TotalAllowances ?? 0);
        var totalDeductions = slips.Sum(p => p.TotalDeductions ?? 0);

        // Cost by Department
        var deptCost = slips
            .GroupBy(p => p.Employee.Department != null ? p.Employee.Department.DeptNameAr : "غير معرف")
            .Select(g => new AnalyticsPayrollBreakdownDto
            {
                Category = g.Key,
                Amount = g.Sum(p => p.NetSalary ?? 0)
            })
            .OrderByDescending(x => x.Amount)
            .ToList();

        return new AnalyticsPayrollStatsDto
        {
            Month = month,
            Year = year,
            TotalNetPay = totalNet,
            TotalBasicSalary = totalBasic,
            TotalAllowances = totalAllowances,
            TotalDeductions = totalDeductions,
            DepartmentCost = deptCost
        };
    }

    // ═══════════════════════════════════════════════════════════
    // 4. Operational Reports
    // ═══════════════════════════════════════════════════════════
    public async Task<List<EmployeeCensusDto>> GetEmployeeCensusReportAsync(int? departmentId = null, string? status = null)
    {
        var query = _context.Employees
            .Include(e => e.Department)
            .Include(e => e.Job)
            .Include(e => e.Compensation)
            .AsNoTracking()
            .AsQueryable();

        if (departmentId.HasValue)
            query = query.Where(e => e.DepartmentId == departmentId);

        if (!string.IsNullOrEmpty(status))
        {
            if (status == "Active")
                query = query.Where(e => e.IsActive && e.TerminationDate == null);
            else if (status == "Terminated")
                query = query.Where(e => !e.IsActive || e.TerminationDate != null);
        }

        return await query.Select(e => new EmployeeCensusDto
        {
            EmployeeId = e.EmployeeId,
            EmployeeNumber = e.EmployeeNumber,
            FullNameAr = e.FirstNameAr + " " + e.LastNameAr,
            Department = e.Department != null ? e.Department.DeptNameAr : "-",
            JobTitle = e.Job != null ? e.Job.JobTitleAr : "-",
            HireDate = e.HireDate,
            Status = (e.IsActive && e.TerminationDate == null) ? "نشط" : "منتهي",
            Nationality = e.NationalityId != null ? e.NationalityId.ToString() : "-",
            MobileNumber = e.Mobile ?? "-",
            Email = e.Email ?? "-",
            BasicSalary = e.Compensation != null ? e.Compensation.BasicSalary : 0
        }).ToListAsync();
    }

    public async Task<List<DailyAttendanceDetailsDto>> GetDetailedAttendanceReportAsync(DateTime startDate, DateTime endDate, int? departmentId = null)
    {
        var query = _context.DailyAttendances
            .Include(d => d.Employee)
            .ThenInclude(e => e.Department)
            .Include(d => d.PlannedShift)
            .AsNoTracking()
            .Where(d => d.AttendanceDate >= startDate && d.AttendanceDate <= endDate);

        if (departmentId.HasValue)
            query = query.Where(d => d.Employee.DepartmentId == departmentId);

        return await query.Select(d => new DailyAttendanceDetailsDto
        {
            RecordId = d.RecordId,
            Date = d.AttendanceDate,
            EmployeeId = d.EmployeeId,
            EmployeeName = d.Employee.FirstNameAr + " " + d.Employee.LastNameAr,
            Department = d.Employee.Department != null ? d.Employee.Department.DeptNameAr : "-",
            PlannedShift = d.PlannedShift != null ? d.PlannedShift.ShiftNameAr : "-",
            InTime = d.ActualInTime.HasValue ? d.ActualInTime.Value.ToString("HH:mm") : "-",
            OutTime = d.ActualOutTime.HasValue ? d.ActualOutTime.Value.ToString("HH:mm") : "-",
            Status = d.Status ?? "-",
            LateMinutes = d.LateMinutes,
            OvertimeMinutes = d.OvertimeMinutes
        })
        .OrderBy(d => d.Date)
        .ThenBy(d => d.EmployeeName)
        .ToListAsync();
    }

    public async Task<List<MonthlyPayslipReportDto>> GetMonthlyPayslipReportAsync(int month, int year, int? departmentId = null)
    {
        var query = _context.Payslips
            .Include(p => p.Employee)
            .ThenInclude(e => e.Department)
            .Include(p => p.PayrollRun)
            .AsNoTracking()
            .Where(p => p.PayrollRun.Month == month && p.PayrollRun.Year == year);

        if (departmentId.HasValue)
            query = query.Where(p => p.Employee.DepartmentId == departmentId);

        return await query.Select(p => new MonthlyPayslipReportDto
        {
            PayslipId = p.PayslipId,
            EmployeeName = p.Employee.FirstNameAr + " " + p.Employee.LastNameAr,
            Department = p.Employee.Department != null ? p.Employee.Department.DeptNameAr : "-",
            BasicSalary = p.BasicSalary ?? 0,
            TotalAllowances = p.TotalAllowances ?? 0,
            TotalDeductions = p.TotalDeductions ?? 0,
            NetSalary = p.NetSalary ?? 0,
            AbsenceDays = p.AbsenceDays,
            LateMinutes = p.TotalLateMinutes,
            PaymentStatus = p.PayrollRun.Status ?? "DRAFT"
        })
        .OrderBy(p => p.EmployeeName)
        .ToListAsync();
    }

    // ══ RESTORED MISSING METHODS START ══
    public async Task<List<LeaveHistoryDto>> GetLeaveHistoryReportAsync(DateTime startDate, DateTime endDate, int? departmentId = null)
    {
        var query = _context.LeaveRequests
            .Include(l => l.Employee)
            .ThenInclude(e => e.Department)
            .Include(l => l.LeaveType)
            .AsNoTracking()
            .Where(l => l.StartDate >= startDate && l.StartDate <= endDate);

        if (departmentId.HasValue)
            query = query.Where(l => l.Employee.DepartmentId == departmentId);

        return await query.Select(l => new LeaveHistoryDto
        {
            RequestId = l.RequestId,
            EmployeeName = l.Employee.FirstNameAr + " " + l.Employee.LastNameAr,
            Department = l.Employee.Department != null ? l.Employee.Department.DeptNameAr : "-",
            LeaveType = l.LeaveType.LeaveNameAr,
            StartDate = l.StartDate,
            EndDate = l.EndDate,
            Days = l.DaysCount,
            Status = l.Status,
            Reason = l.Reason ?? "-"
        })
        .OrderByDescending(l => l.RequestDate)
        .ToListAsync();
    }

    public async Task<List<RecruitmentReportDto>> GetRecruitmentReportAsync(DateTime startDate, DateTime endDate, string? status = null)
    {
        var query = _context.JobApplications
            .Include(a => a.Candidate)
            .Include(a => a.Vacancy)
            .ThenInclude(v => v.Job)
            .AsNoTracking()
            .Where(a => a.ApplicationDate >= startDate && a.ApplicationDate <= endDate);

        if (!string.IsNullOrEmpty(status))
            query = query.Where(a => a.Status == status);

        return await query.Select(a => new RecruitmentReportDto
        {
            CandidateId = a.CandidateId,
            CandidateName = a.Candidate.FirstNameAr + " " + a.Candidate.FamilyNameAr,
            JobTitle = a.Vacancy.Job != null ? a.Vacancy.Job.JobTitleAr : "-",
            Email = a.Candidate.Email,
            MobileNumber = a.Candidate.Phone ?? "-",
            Status = a.Status ?? "-",
            ApplicationDate = a.ApplicationDate
        })
        .OrderBy(a => a.ApplicationDate)
        .ToListAsync();
    }

    public async Task<List<PerformanceReportDto>> GetPerformanceReportAsync(int cycleId, int? departmentId = null)
    {
        var query = _context.EmployeeAppraisals
            .Include(a => a.Employee)
            .ThenInclude(e => e.Department)
            .Include(a => a.Cycle)
            .Include(a => a.Evaluator)
            .AsNoTracking()
            .Where(a => a.CycleId == cycleId);

        if (departmentId.HasValue)
            query = query.Where(a => a.Employee.DepartmentId == departmentId);

        return await query.Select(a => new PerformanceReportDto
        {
            AppraisalId = a.AppraisalId,
            EmployeeName = a.Employee.FirstNameAr + " " + a.Employee.LastNameAr,
            Department = a.Employee.Department != null ? a.Employee.Department.DeptNameAr : "-",
            CycleName = a.Cycle.CycleNameAr,
            OverallScore = a.TotalScore ?? 0,
            Rating = a.Grade ?? "-",
            EvaluatorName = a.Evaluator != null ? (a.Evaluator.FirstNameAr + " " + a.Evaluator.LastNameAr) : "-",
            Status = a.Status ?? "-"
        })
        .OrderByDescending(a => a.OverallScore)
        .ToListAsync();
    }
    // ══ RESTORED MISSING METHODS END ══

    // ═══════════════════════════════════════════════════════════
    // 5. Comprehensive Dashboard
    // ═══════════════════════════════════════════════════════════
    public async Task<ComprehensiveDashboardDto> GetComprehensiveDashboardAsync()
    {
        var today = DateTime.UtcNow.Date;
        var startOfMonth = new DateTime(today.Year, today.Month, 1);

        // 1. Attendance Metrics (Live + Processed)
        var processedAttendance = await _context.DailyAttendances
            .AsNoTracking()
            .Where(d => d.AttendanceDate == today && d.IsDeleted == 0)
            .ToListAsync();

        var todayPunches = await _context.RawPunchLogs
            .AsNoTracking()
            .Where(p => p.PunchTime >= today && p.PunchTime < today.AddDays(1))
            .Select(p => new { p.EmployeeId, p.PunchType })
            .ToListAsync();

        var punchedInEmployeeIds = todayPunches
            .GroupBy(p => p.EmployeeId)
            .Where(g => g.Any(x => x.PunchType == "IN" || x.PunchType == "BREAK_IN"))
            .Select(g => g.Key)
            .Distinct()
            .ToList();

        var processedPresentIds = processedAttendance
            .Where(a => (a.Status ?? "").ToUpper() == "PRESENT" || (a.Status ?? "").ToUpper() == "LATE")
            .Select(a => a.EmployeeId)
            .ToList();

        var livePresentIds = punchedInEmployeeIds.Except(processedPresentIds).ToList();
        
        var totalPresent = processedPresentIds.Count + livePresentIds.Count;
        var totalLate = processedAttendance.Count(a => (a.Status ?? "").ToUpper() == "LATE");

        var totalEmployees = await _context.Employees.CountAsync(e => e.IsDeleted == 0 && e.IsActive);

        var peopleOnLeave = await _context.LeaveRequests
            .CountAsync(l => l.StartDate <= today && l.EndDate >= today && (l.Status == "APPROVED" || l.Status == "Approved") && l.IsDeleted == 0);

        var derivedAbsent = totalEmployees - (totalPresent + peopleOnLeave);
        if (derivedAbsent < 0) derivedAbsent = 0;

        var attendanceMetrics = new AttendanceMetricsDto
        {
            TotalPresent = totalPresent,
            TotalAbsent = derivedAbsent,
            TotalLate = totalLate,
            TotalLeaves = peopleOnLeave,
            AttendanceRate = totalEmployees > 0 ? Math.Round(((double)totalPresent / totalEmployees) * 100, 1) : 0,
            ShiftDistribution = new Dictionary<string, int>() 
        };

        // 2. Personnel Metrics
        var employees = await _context.Employees
            .Include(e => e.Department)
            .Where(e => e.IsDeleted == 0)
            .AsNoTracking()
            .ToListAsync();

        var thirtyDaysFromNow = today.AddDays(30);
        var expiringDocs = await _context.EmployeeDocuments
            .CountAsync(d => d.ExpiryDate != null && d.ExpiryDate >= today && d.ExpiryDate <= thirtyDaysFromNow && d.IsDeleted == 0);

        var personnelMetrics = new PersonnelMetricsDto
        {
            TotalEmployees = employees.Count,
            ActiveEmployees = employees.Count(e => e.IsActive && e.TerminationDate == null),
            InactiveEmployees = employees.Count(e => !e.IsActive || e.TerminationDate != null),
            ExpiringDocumentsCount = expiringDocs,
            NewHires = employees.Count(e => e.HireDate >= startOfMonth),
            ActiveContracts = await _context.Contracts.CountAsync(c => c.IsActive == true && c.IsDeleted == 1),
            DepartmentStats = employees
                 .Where(e => e.IsActive && e.TerminationDate == null)
                 .GroupBy(e => e.Department)
                 .Select(g => new DepartmentStatDto
                 {
                     DepartmentId = g?.Key?.DeptId ?? 0,
                     DepartmentName = g?.Key?.DeptNameAr ?? "غير معرف",
                     EmployeeCount = g?.Count() ?? 0
                 })
                 .ToList()
        };

        // 3. Requests Metrics
        var requestsMetrics = new RequestsMetricsDto
        {
            PendingLeaveRequests = await _context.LeaveRequests.CountAsync(x => x.Status.ToLower() == "pending" && x.IsDeleted == 0),
            PendingOvertimeRequests = await _context.OvertimeRequests.CountAsync(x => x.Status.ToLower() == "pending" && x.IsDeleted == 0),
            PendingLoanRequests = await _context.Loans.CountAsync(x => x.Status.ToLower() == "pending" && x.IsDeleted == 0),
            PendingShiftSwaps = await _context.ShiftSwapRequests.CountAsync(x => x.Status.ToLower() == "pending" && x.IsDeleted == 0),
            PendingPermissions = await _context.PermissionRequests.CountAsync(x => x.Status.ToLower() == "pending" && x.IsDeleted == 0)
        };

        // 3.5 Performance Metrics
        var activeCycles = await _context.AppraisalCycles
            .Where(c => c.StartDate <= today && c.EndDate >= today && c.IsDeleted == 0)
            .ToListAsync();
        
        var pendingAppraisals = await _context.EmployeeAppraisals
            .CountAsync(a => (a.Status.ToLower() == "pending" || a.Status.ToLower() == "in_progress") && a.IsDeleted == 0);

        var avgRating = await _context.EmployeeAppraisals
            .Where(a => a.Status.ToLower() == "completed" && a.TotalScore.HasValue && a.IsDeleted == 0)
            .Select(a => (double)a.TotalScore!.Value)
            .DefaultIfEmpty(0)
            .AverageAsync();

        var performanceMetrics = new PerformanceMetricsDto
        {
            ActiveAppraisalCycles = activeCycles.Count,
            PendingEvaluations = pendingAppraisals,
            AverageCompanyRating = Math.Round(avgRating, 1)
        };

        // 3.6 Setup Metrics
        var setupMetrics = new SetupMetricsDto
        {
            TotalDepartments = await _context.Departments.CountAsync(d => d.IsDeleted == 0),
            TotalJobTitles = await _context.Jobs.CountAsync(j => j.IsDeleted == 0),
            TotalShiftTypes = await _context.ShiftTypes.CountAsync(s => s.IsDeleted == 0),
            TotalActiveUsers = await _context.Employees.CountAsync(u => u.IsActive && u.UserId != null)
        };

        // 4. Financial Metrics
        var latestRun = await _context.PayrollRuns
            .AsNoTracking()
            .OrderByDescending(r => r.Year).ThenByDescending(r => r.Month)
            .FirstOrDefaultAsync(r => r.IsDeleted == 0);

        decimal totalNet = 0;
        decimal totalBasic = 0;
        decimal totalInfoDed = 0;

        if (latestRun != null && latestRun.Month == today.Month && latestRun.Year == today.Year)
        {
             var payslips = await _context.Payslips
                .Where(p => p.RunId == latestRun.RunId)
                .Select(p => new { p.NetSalary, p.BasicSalary, p.TotalDeductions })
                .ToListAsync();
            
            totalNet = payslips.Sum(p => p.NetSalary ?? 0);
            totalBasic = payslips.Sum(p => p.BasicSalary ?? 0);
            totalInfoDed = payslips.Sum(p => p.TotalDeductions ?? 0);
        }
        else
        {
            // PROJECTION: Sum up active salaries directly from structures
            // Load Active Employee Structures with their SalaryElements
            var activeStructures = await _context.EmployeeSalaryStructures
                .Include(s => s.SalaryElement)
                .Include(s => s.Employee)
                .Where(s => s.IsActive == 1 && s.Employee.IsActive && s.Employee.IsDeleted == 0 && s.SalaryElement != null)
                .Select(s => new 
                { 
                    s.Amount, 
                    s.SalaryElement.ElementType, // "EARNING", "DEDUCTION"
                    s.SalaryElement.IsBasic     // 1 or 0
                })
                .ToListAsync();

            var totalEarnings = activeStructures
                .Where(s => s.ElementType == "EARNING" || s.ElementType == "Addition") // flexible check
                .Sum(s => s.Amount);

            var totalDeductions = activeStructures
                .Where(s => s.ElementType == "DEDUCTION")
                .Sum(s => s.Amount);

            totalBasic = activeStructures
                .Where(s => s.IsBasic == 1)
                .Sum(s => s.Amount);
            
            totalNet = totalEarnings - totalDeductions;
            totalInfoDed = totalDeductions;
        }

        var activeLoans = await _context.Loans
            .Where(l => (l.Status.ToLower() == "active" || l.Status.ToLower() == "approved") && l.IsDeleted == 0)
            .ToListAsync();

        var pendingPayslips = await _context.Payslips
            .Include(p => p.PayrollRun)
            .Where(p => (p.PayrollRun.Status.ToLower() == "draft" || p.PayrollRun.Status.ToLower() == "pending") && p.PayrollRun.IsDeleted == 0)
            .ToListAsync();

        var financialMetrics = new FinancialMetricsDto
        {
            TotalNetSalary = (double)totalNet,
            TotalBasicSalary = (double)totalBasic,
            TotalDeductions = (double)totalInfoDed,
            PendingPayrollCount = 0,
            
            TotalPendingSalaries = pendingPayslips.Sum(p => p.NetSalary ?? 0),
            PendingSalariesByDepartment = new Dictionary<string, decimal>(), 
            ActiveLoansCount = activeLoans.Count,
            TotalActiveLoansAmount = activeLoans.Sum(l => l.LoanAmount)
        };

        // 5. Holiday Metrics
        var currentYear = (short)today.Year;
        var holidays = await _context.PublicHolidays
            .Where(h => h.Year == currentYear && h.IsDeleted == 0)
            .ToListAsync();

        var holidayMetrics = holidays
            .GroupBy(h => InferHolidayType(h.HolidayNameAr))
            .Select(g => new HolidayMetricDto
            {
                HolidayType = g.Key,
                HolidaysCount = g.Count(),
                DaysCount = g.Sum(h => (h.EndDate - h.StartDate).Days + 1)
            })
            .ToList();

        // 6. Weekly Metrics
        var sevenDaysAgo = today.AddDays(-6);
        var weeklyLogs = await _context.DailyAttendances
            .Where(d => d.AttendanceDate >= sevenDaysAgo && d.AttendanceDate <= today && d.IsDeleted == 0)
            .ToListAsync();

        var trendData = new List<AnalyticsDailyAttendanceSummaryDto>();
        for (int i = 0; i < 7; i++)
        {
            var date = sevenDaysAgo.AddDays(i);
            var dayLogs = weeklyLogs.Where(l => l.AttendanceDate.Date == date.Date).ToList();
            
            trendData.Add(new AnalyticsDailyAttendanceSummaryDto
            {
                Date = date,
                PresentCount = dayLogs.Count(x => (x.Status ?? "").ToUpper() == "PRESENT" || (x.Status ?? "").ToUpper() == "LATE"),
                AbsentCount = dayLogs.Count(x => (x.Status ?? "").ToUpper() == "ABSENT"),
                LateCount = dayLogs.Count(x => (x.Status ?? "").ToUpper() == "LATE")
            });
        }

        var weeklyMetrics = new WeeklyAnalyticsDto
        {
            AttendanceTrend = trendData,
            TotalHoursWorked = weeklyLogs.Sum(x => (x.ActualOutTime - x.ActualInTime)?.TotalHours ?? 0),
            TotalOvertimeMinutes = weeklyLogs.Sum(x => x.OvertimeMinutes)
        };

        // 7. Monthly Metrics
        var monthlyLogs = await _context.DailyAttendances
            .Where(d => d.AttendanceDate >= startOfMonth && d.AttendanceDate <= today && d.IsDeleted == 0)
            .ToListAsync();
            
        var monthlyPresent = monthlyLogs.Count(x => (x.Status ?? "").ToUpper() == "PRESENT" || (x.Status ?? "").ToUpper() == "LATE");
        var monthlyAbsent = monthlyLogs.Count(x => (x.Status ?? "").ToUpper() == "ABSENT");
        var monthlyTotal = monthlyPresent + monthlyAbsent;

        // Fetch payroll for the current month
        var monthlySlips = await _context.Payslips
            .Include(p => p.Employee)
            .ThenInclude(e => e.Department)
            .Include(p => p.PayrollRun)
            .Where(p => p.PayrollRun.Month == today.Month && p.PayrollRun.Year == today.Year)
            .ToListAsync();

        var monthlyMetrics = new MonthlyAnalyticsDto
        {
            TotalPresent = monthlyPresent,
            TotalAbsent = monthlyAbsent,
            TotalLate = monthlyLogs.Count(x => (x.Status ?? "").ToUpper() == "LATE"),
            TotalLeaves = monthlyLogs.Count(x => (x.Status ?? "").ToUpper() == "LEAVE"),
            AttendanceRate = monthlyTotal > 0 ? ((double)monthlyPresent / monthlyTotal) * 100 : 0,
            NewHires = employees.Count(e => e.HireDate >= startOfMonth && e.HireDate <= today.AddDays(1).AddSeconds(-1)),
            Resignations = employees.Count(e => e.TerminationDate != null && e.TerminationDate >= startOfMonth && e.TerminationDate <= today.AddDays(1).AddSeconds(-1)),
            
            TotalNetSalary = monthlySlips.Sum(p => p.NetSalary ?? 0),
            TotalBasicSalary = monthlySlips.Sum(p => p.BasicSalary ?? 0),
            TotalAllowances = monthlySlips.Sum(p => p.TotalAllowances ?? 0),
            TotalDeductions = monthlySlips.Sum(p => p.TotalDeductions ?? 0),
            SalaryByDepartment = monthlySlips
                .GroupBy(p => p.Employee?.Department != null ? p.Employee.Department.DeptNameAr : "غير معرف")
                .ToDictionary(g => g.Key, g => g.Sum(p => p.NetSalary ?? 0))
        };

        return new ComprehensiveDashboardDto
        {
            ReportDate = today,
            AttendanceMetrics = attendanceMetrics,
            PersonnelMetrics = personnelMetrics,
            RequestsMetrics = requestsMetrics,
            FinancialMetrics = financialMetrics,
            HolidayMetrics = holidayMetrics,
            WeeklyMetrics = weeklyMetrics,
            MonthlyMetrics = monthlyMetrics,
            PerformanceMetrics = performanceMetrics,
            SetupMetrics = setupMetrics
        };
    }

    private string InferHolidayType(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return "Other";
        name = name.ToLower();
        if (name.Contains("عيد") || name.Contains("fitr") || name.Contains("adha") || name.Contains("ramadan")) return "Religious";
        if (name.Contains("watani") || name.Contains("national") || name.Contains("tahsis") || name.Contains("flag")) return "National";
        return "Official";
    }
}
