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
    public async Task<HROverviewDto> GetHROverviewAsync()
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
        // Group by DeptNameAr (String Key)
        var deptDist = await _context.Employees
            .Where(e => e.IsActive && e.TerminationDate == null)
            .GroupBy(e => e.Department.DeptNameAr) 
            .Select(g => new DepartmentDistributionDto
            {
                DepartmentName = g.Key, // Key IS the DeptNameAr (String)
                EmployeeCount = g.Count()
            })
            .ToListAsync();

        // 3. Document Expirations (Next 30 Days)
        var thirtyDaysFromNow = today.AddDays(30);
        var expirations = await _context.EmployeeDocuments
            .Include(d => d.Employee)
            .Include(d => d.DocumentType)
            .Where(d => d.ExpiryDate != null && d.ExpiryDate >= today && d.ExpiryDate <= thirtyDaysFromNow)
            .Select(d => new DocumentExpiryDto
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

        return new HROverviewDto
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
    public async Task<AttendanceStatsDto> GetAttendanceStatsAsync(DateTime startDate, DateTime endDate)
    {
        var logs = await _context.DailyAttendances
            .Include(d => d.Employee)
            .ThenInclude(e => e.Department)
            .Where(d => d.AttendanceDate >= startDate && d.AttendanceDate <= endDate)
            .ToListAsync();

        var totalDays = (endDate - startDate).Days + 1;
        
        var totalPresent = logs.Count(l => l.Status == "Present" || l.Status == "Late");
        var totalAbsent = logs.Count(l => l.Status == "Absent");
        var totalLate = logs.Count(l => l.Status == "Late");

        // 1. Daily Trend
        var trend = logs
            .GroupBy(l => l.AttendanceDate.Date)
            .Select(g => new DailyAttendanceSummaryDto
            {
                Date = g.Key,
                PresentCount = g.Count(x => x.Status == "Present" || x.Status == "Late"),
                AbsentCount = g.Count(x => x.Status == "Absent"),
                LateCount = g.Count(x => x.Status == "Late")
            })
            .OrderBy(x => x.Date)
            .ToList();

        // 2. Top Late Employees
        var topLate = logs
            .Where(l => l.Status == "Late" && l.LateMinutes > 0)
            .GroupBy(l => l.Employee) // Group by Employee Object
            .Select(g => new EmployeeAttendanceRankingDto
            {
                EmployeeName = g.Key.FirstNameAr + " " + g.Key.LastNameAr,
                // Accessing Department via g.Key (Employee) -> Department -> DeptNameAr
                Department = g.Key.Department != null ? g.Key.Department.DeptNameAr : "-",
                Count = g.Sum(x => x.LateMinutes)
            })
            .OrderByDescending(x => x.Count)
            .Take(5)
            .ToList();

        return new AttendanceStatsDto
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
    public async Task<PayrollStatsDto> GetPayrollStatsAsync(int month, int year)
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

        // Cost by Department (String Key)
        var deptCost = slips
            .GroupBy(p => p.Employee.Department != null ? p.Employee.Department.DeptNameAr : "غير معرف")
            .Select(g => new PayrollBreakdownDto
            {
                Category = g.Key,
                Amount = g.Sum(p => p.NetSalary ?? 0)
            })
            .OrderByDescending(x => x.Amount)
            .ToList();

        return new PayrollStatsDto
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
            Nationality = e.NationalityId != null ? e.NationalityId.ToString() : "-", // Using ID as fallback since Entity doesn't verify easily
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
            Reason = l.Reason ?? "-",
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
}
