using HRMS.Application.DTOs.Payroll.Processing;
using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Payroll.Reports.Queries.GetMonthlySummary;

/// <summary>
/// الحصول على ملخص شهري للرواتب
/// Get monthly payroll summary report
/// </summary>
public class GetMonthlySummaryQuery : IRequest<Result<MonthlyPayrollSummaryDto>>
{
    public int Month { get; set; }
    public int Year { get; set; }
    public int? DepartmentId { get; set; }
}

public class GetMonthlySummaryQueryHandler : IRequestHandler<GetMonthlySummaryQuery, Result<MonthlyPayrollSummaryDto>>
{
    private readonly IApplicationDbContext _context;

    public GetMonthlySummaryQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<MonthlyPayrollSummaryDto>> Handle(GetMonthlySummaryQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // البحث عن مسير الرواتب للشهر المحدد
            // Find payroll run for specified month
            var payrollRun = await _context.PayrollRuns
                .Include(r => r.Payslips)
                    .ThenInclude(p => p.Employee)
                        .ThenInclude(e => e.Department)
                .FirstOrDefaultAsync(r => r.Month == request.Month && r.Year == request.Year, cancellationToken);

            MonthlyPayrollSummaryDto summary = null;

            if (payrollRun != null)
            {
                // مسجل حضور معتمد - جلب البيانات كما هي في التاريخ
                // Finalized payroll run exists - use historical data
                var payslips = payrollRun.Payslips.AsEnumerable();
                if (request.DepartmentId.HasValue)
                {
                    payslips = payslips.Where(p => p.Employee.DepartmentId == request.DepartmentId.Value);
                }

                var payslipsList = payslips.ToList();

                summary = new MonthlyPayrollSummaryDto
                {
                    Month = request.Month,
                    Year = request.Year,
                    IsFinalized = true,
                    TotalEmployees = payslipsList.Count,
                    TotalBasicSalaries = payslipsList.Sum(p => p.BasicSalary ?? 0),
                    TotalAllowances = payslipsList.Sum(p => p.TotalAllowances ?? 0),
                    TotalDeductions = payslipsList.Sum(p => p.TotalDeductions ?? 0),
                    TotalOvertimePayments = payslipsList.Sum(p => p.OvertimeEarnings),
                    TotalNetSalaries = payslipsList.Sum(p => p.NetSalary ?? 0),
                    DepartmentBreakdown = payslipsList
                        .GroupBy(p => new { p.Employee.DepartmentId, p.Employee.Department.DeptNameAr })
                        .Select(g => new DepartmentPayrollSummary
                        {
                            DepartmentId = g.Key.DepartmentId,
                            DepartmentName = g.Key.DeptNameAr ?? "غير محدد",
                            EmployeeCount = g.Count(),
                            TotalBasicSalaries = g.Sum(p => p.BasicSalary ?? 0),
                            TotalAllowances = g.Sum(p => p.TotalAllowances ?? 0),
                            TotalDeductions = g.Sum(p => p.TotalDeductions ?? 0),
                            TotalNetSalaries = g.Sum(p => p.NetSalary ?? 0)
                        })
                        .ToList()
                };
            }
            else
            {
                // لا يوجد مسير رواتب معتمد بعد - نقوم بعمل "عرض توزيعه تقديري" (Live Projection)
                // No finalized payroll yet - perform live projection based on current active structures
                
                var employeesQuery = _context.Employees
                    .Include(e => e.Department)
                    .Where(e => e.IsActive && e.IsDeleted == 0);

                if (request.DepartmentId.HasValue)
                    employeesQuery = employeesQuery.Where(e => e.DepartmentId == request.DepartmentId.Value);

                var employees = await employeesQuery.ToListAsync(cancellationToken);
                
                summary = new MonthlyPayrollSummaryDto
                {
                    Month = request.Month,
                    Year = request.Year,
                    IsFinalized = false,
                    TotalEmployees = employees.Count
                };

                var deptSummaries = new Dictionary<int, DepartmentPayrollSummary>();

                foreach (var emp in employees)
                {
                    // جلب هيكل الراتب النشط
                    var structure = await _context.SalaryStructures
                        .Include(s => s.SalaryElement)
                        .Where(s => s.EmployeeId == emp.EmployeeId && s.IsActive == 1)
                        .AsNoTracking()
                        .ToListAsync(cancellationToken);

                    decimal empBasic = structure.FirstOrDefault(s => s.SalaryElement.IsBasic == 1)?.Amount ?? 0;
                    decimal empAllowances = structure.Where(s => s.SalaryElement.ElementType == "EARNING" && s.SalaryElement.IsBasic == 0).Sum(s => s.Amount);
                    decimal empDeductions = structure.Where(s => s.SalaryElement.ElementType == "DEDUCTION").Sum(s => s.Amount);

                    // حساب أقساط السلف النشطة لهذا الشهر
                    var activeLoans = await _context.Loans
                        .Where(l => l.EmployeeId == emp.EmployeeId && l.Status == "ACTIVE")
                        .ToListAsync(cancellationToken);
                    
                    decimal empLoanDeductions = 0;
                    foreach (var loan in activeLoans)
                    {
                        empLoanDeductions += loan.InstallmentCount > 0 ? loan.LoanAmount / loan.InstallmentCount : 0;
                    }
                    
                    decimal empTotalDeductions = empDeductions + empLoanDeductions;
                    decimal empNet = (empBasic + empAllowances) - empTotalDeductions;

                    // تحديث الإجمالي الكلي
                    summary.TotalBasicSalaries += empBasic;
                    summary.TotalAllowances += empAllowances;
                    summary.TotalDeductions += empTotalDeductions;
                    summary.TotalNetSalaries += empNet;

                    // تحديث تفصيل الأقسام
                    int deptId = emp.DepartmentId;
                    if (!deptSummaries.ContainsKey(deptId))
                    {
                        deptSummaries[deptId] = new DepartmentPayrollSummary
                        {
                            DepartmentId = deptId,
                            DepartmentName = emp.Department?.DeptNameAr ?? "غير محدد",
                            EmployeeCount = 0
                        };
                    }

                    var d = deptSummaries[deptId];
                    d.EmployeeCount++;
                    d.TotalBasicSalaries += empBasic;
                    d.TotalAllowances += empAllowances;
                    d.TotalDeductions += empTotalDeductions;
                    d.TotalNetSalaries += empNet;
                }

                summary.DepartmentBreakdown = deptSummaries.Values.ToList();
            }

            return Result<MonthlyPayrollSummaryDto>.Success(summary);
        }
        catch (Exception ex)
        {
            return Result<MonthlyPayrollSummaryDto>.Failure($"خطأ في إنشاء ملخص الرواتب: {ex.Message}");
        }
    }
}
