using HRMS.Application.DTOs.Payroll.Processing;
using HRMS.Application.Interfaces;
using HRMS.Core.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace HRMS.Application.Features.Payroll.Processing.Queries.ExportBankFile;

public class ExportBankFileQuery : IRequest<Result<List<BankFileRecordDto>>>
{
    public int Month { get; set; }
    public int Year { get; set; }
}

public class ExportBankFileQueryHandler : IRequestHandler<ExportBankFileQuery, Result<List<BankFileRecordDto>>>
{
    private readonly IApplicationDbContext _context;

    public ExportBankFileQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<BankFileRecordDto>>> Handle(ExportBankFileQuery request, CancellationToken cancellationToken)
    {
        // 1. Find the PayrollRun for this Month/Year
        var payrollRun = await _context.PayrollRuns
            .FirstOrDefaultAsync(r => r.Month == request.Month && r.Year == request.Year, cancellationToken);

        if (payrollRun == null)
            return Result<List<BankFileRecordDto>>.Failure("لا توجد مسيرة رواتب لهذا الشهر/السنة");

        // Only export if status is COMPLETED or APPROVED
        if (payrollRun.Status != "COMPLETED" && payrollRun.Status != "APPROVED")
            return Result<List<BankFileRecordDto>>.Failure($"حالة المسيرة غير صالحة للتصدير: {payrollRun.Status}");

        // 2. Fetch Payslips with Employee and Bank Account details
        var payslips = await _context.Payslips
            .Include(p => p.Employee)
                .ThenInclude(e => e.BankAccounts.Where(b => b.IsPrimary == 1 && b.IsActive == 1))
                    .ThenInclude(b => b.Bank)
            .Where(p => p.RunId == payrollRun.RunId)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        if (!payslips.Any())
            return Result<List<BankFileRecordDto>>.Failure("لا توجد قسائم رواتب في هذه المسيرة");

        // 3. Map to BankFileRecordDto
        var records = new List<BankFileRecordDto>();
        var monthName = CultureInfo.GetCultureInfo("en-US").DateTimeFormat.GetMonthName(request.Month);
        var paymentRef = $"Salary_{monthName}_{request.Year}";

        foreach (var payslip in payslips)
        {
            var primaryAccount = payslip.Employee.BankAccounts.FirstOrDefault();
            
            // Skip employees without bank accounts (log warning in production)
            if (primaryAccount == null) continue;

            records.Add(new BankFileRecordDto
            {
                EmployeeNameAr = payslip.Employee.FullNameAr,
                EmployeeNameEn = payslip.Employee.FullNameEn ?? payslip.Employee.FullNameAr,
                AccountNumber = primaryAccount.AccountNumber,
                Iban = primaryAccount.Iban,
                BankName = primaryAccount.Bank.BankNameAr,
                NetSalary = payslip.NetSalary ?? 0,
                Currency = "YER",
                PaymentReference = paymentRef
            });
        }

        return Result<List<BankFileRecordDto>>.Success(records);
    }
}
