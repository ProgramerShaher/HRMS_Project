using HRMS.Application.DTOs.Payroll.Processing;
using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Application.Features.Payroll.Processing.Queries.GetAllEmployeesSalaries;

/// <summary>
/// الحصول على جدول شامل برواتب جميع الموظفين
/// Get comprehensive table of all employees' salaries
/// </summary>
public class GetAllEmployeesSalariesQuery : IRequest<Result<List<EmployeeSalaryDetailDto>>>
{
    public int? DepartmentId { get; set; }
    public bool? IsActive { get; set; }
    public string? SearchTerm { get; set; }
}

public class GetAllEmployeesSalariesQueryHandler : IRequestHandler<GetAllEmployeesSalariesQuery, Result<List<EmployeeSalaryDetailDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetAllEmployeesSalariesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<EmployeeSalaryDetailDto>>> Handle(GetAllEmployeesSalariesQuery request, CancellationToken cancellationToken)
    {
        // جلب جميع الموظفين مع معلوماتهم الأساسية
        var employeesQuery = _context.Employees
            .Include(e => e.Department)
            .Include(e => e.Job)
            .AsNoTracking();

        // تطبيق الفلاتر
        if (request.DepartmentId.HasValue)
        {
            employeesQuery = employeesQuery.Where(e => e.DepartmentId == request.DepartmentId.Value);
        }

        if (request.IsActive.HasValue)
        {
            employeesQuery = employeesQuery.Where(e => e.IsActive == (request.IsActive.Value ? true : false));
        }

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.ToLower();
            employeesQuery = employeesQuery.Where(e => 
                e.FullNameAr.ToLower().Contains(searchTerm) ||
                e.EmployeeNumber.ToLower().Contains(searchTerm));
        }

        var employees = await employeesQuery.ToListAsync(cancellationToken);

        var result = new List<EmployeeSalaryDetailDto>();

        foreach (var employee in employees)
        {
            var dto = new EmployeeSalaryDetailDto
            {
                EmployeeId = employee.EmployeeId,
                EmployeeCode = employee.EmployeeNumber,
                EmployeeNameAr = employee.FullNameAr ?? "Unknown",
                DepartmentName = employee.Department?.DeptNameAr,
                JobTitle = employee.Job?.JobTitleAr,
                IsActive = employee.IsActive == true
            };

            // جلب هيكل الراتب
            var structure = await _context.SalaryStructures
                .Include(s => s.SalaryElement)
                .Where(s => s.EmployeeId == employee.EmployeeId && s.IsActive == 1)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            dto.HasSalaryStructure = structure.Any();

            if (structure.Any())
            {
                // الراتب الأساسي
                var basicElement = structure.FirstOrDefault(s => s.SalaryElement.IsBasic == 1);
                dto.BasicSalary = basicElement?.Amount ?? 0;

                // البدلات
                var allowances = structure.Where(s => s.SalaryElement.ElementType == "EARNING" && s.SalaryElement.IsBasic == 0).ToList();
                dto.AllowancesCount = allowances.Count;
                dto.TotalAllowances = allowances.Sum(s => s.Amount);

                // الاستقطاعات
                var deductions = structure.Where(s => s.SalaryElement.ElementType == "DEDUCTION").ToList();
                dto.DeductionsCount = deductions.Count;
                dto.TotalDeductions = deductions.Sum(s => s.Amount);

                // إضافة GOSI تلقائياً إذا لم يكن موجود
                if (!structure.Any(s => s.SalaryElement.ElementNameAr.Contains("تأمينات")))
                {
                    dto.TotalDeductions += Math.Round(dto.BasicSalary * 0.09m, 2);
                }

                // الإجمالي
                dto.GrossSalary = dto.BasicSalary + dto.TotalAllowances;
            }

            // جلب السلف النشطة
            var activeLoans = await _context.Loans
                .Where(l => l.EmployeeId == employee.EmployeeId && l.Status == "ACTIVE")
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            dto.ActiveLoansCount = activeLoans.Count;

            // حساب إجمالي الأقساط الشهرية
            foreach (var loan in activeLoans)
            {
                var monthlyInstallment = loan.InstallmentCount > 0 ? loan.LoanAmount / loan.InstallmentCount : 0;
                dto.MonthlyLoanDeduction += monthlyInstallment;
            }

            // الصافي التقريبي (بدون تأثير الحضور)
            dto.NetSalary = dto.GrossSalary - dto.TotalDeductions - dto.MonthlyLoanDeduction;

            // آخر تاريخ راتب (من آخر payslip)
            var lastPayslip = await _context.Payslips
                .Include(p => p.PayrollRun)
                .Where(p => p.EmployeeId == employee.EmployeeId)
                .OrderByDescending(p => p.PayslipId)
                .FirstOrDefaultAsync(cancellationToken);

            if (lastPayslip?.PayrollRun != null)
            {
                dto.LastPayrollDate = new DateTime(lastPayslip.PayrollRun.Year, lastPayslip.PayrollRun.Month, 1);
            }

            result.Add(dto);
        }

        return Result<List<EmployeeSalaryDetailDto>>.Success(result);
    }
}
