using ClosedXML.Excel;
using ClosedXML.Graphics; 
using ClosedXML.Excel.Drawings; // Added for XLPictureFormat
using HRMS.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Drawing; // For Size structure

namespace HRMS.Application.Features.Payroll.Processing.Services;

public class BankFileExportService
{
    private readonly IApplicationDbContext _context;

    // Static constructor to setup the permission bypass once
    static BankFileExportService()
    {
        // CRITICAL: Use our custom "Zero-Scan" engine.
        // This engine returns fixed values and NEVER touches C:\Windows\Fonts.
        LoadOptions.DefaultGraphicEngine = new FallbackGraphicEngine();
    }

    public BankFileExportService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<byte[]> ExportPayrollToExcelAsync(int month, int year)
    {
        // 1. Fetch Data - Get the LATEST RunId for this month/year to ensure fresh data
        var latestRun = await _context.PayrollRuns
            .Where(r => r.Month == month && r.Year == year)
            .OrderByDescending(r => r.RunId)
            .FirstOrDefaultAsync();

        if (latestRun == null)
            throw new Exception($"No Payroll Run found for {month}/{year}");

        var payslips = await _context.Payslips
            .Include(p => p.Employee)
                .ThenInclude(e => e.BankAccounts)
                    .ThenInclude(b => b.Bank)
            .Where(p => p.RunId == latestRun.RunId)
            .ToListAsync();

        using (var workbook = new XLWorkbook())
        {
            var worksheet = workbook.Worksheets.Add("مسير الرواتب");
            worksheet.RightToLeft = true; // RTL
            worksheet.Style.Font.FontName = "Arial"; // Safe default

            // 2. Header
            var headers = new string[] { "اسم الموظف", "رقم الحساب/IBAN", "البنك", "صافي الراتب", "العملة" };
            for (int i = 0; i < headers.Length; i++)
            {
                var cell = worksheet.Cell(1, i + 1);
                cell.Value = headers[i];
                cell.Style.Font.Bold = true;
                cell.Style.Fill.BackgroundColor = XLColor.LightBlue;
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            }

            // 3. Data Rows
            int row = 2;
            foreach (var item in payslips)
            {
                var bankAccount = item.Employee.BankAccounts.FirstOrDefault(b => b.IsPrimary == 1);
                
                worksheet.Cell(row, 1).Value = item.Employee.FullNameAr;
                // Prioritize IBAN
                worksheet.Cell(row, 2).Value = !string.IsNullOrEmpty(bankAccount?.Iban) ? bankAccount.Iban : (bankAccount?.AccountNumber ?? "لا يوجد");
                worksheet.Cell(row, 3).Value = bankAccount?.Bank?.BankNameAr ?? "غير محدد";
                
                // Net Salary
                var netSalaryCell = worksheet.Cell(row, 4);
                netSalaryCell.Value = item.NetSalary ?? 0;
                netSalaryCell.Style.NumberFormat.Format = "#,##0.00"; 
                
                worksheet.Cell(row, 5).Value = "YER";
                row++;
            }

            // 4. Control Total
            worksheet.Cell(row, 3).Value = "إجمالي المسير:";
            worksheet.Cell(row, 3).Style.Font.Bold = true;
            
            var lastRowIndex = row - 1;
            if (lastRowIndex >= 2)
            {
                worksheet.Cell(row, 4).FormulaA1 = $"SUM(D2:D{lastRowIndex})";
                worksheet.Cell(row, 4).Style.Font.Bold = true;
                worksheet.Cell(row, 4).Style.NumberFormat.Format = "#,##0.00";
            }

            // Fixed Widths (Mandatory since we can't measure text)
            worksheet.Column(1).Width = 35; // Name
            worksheet.Column(2).Width = 30; // IBAN
            worksheet.Column(3).Width = 25; // Bank
            worksheet.Column(4).Width = 15; // Net Salary
            worksheet.Column(5).Width = 10; // Currency

            using (var stream = new MemoryStream())
            {
                workbook.SaveAs(stream);
                return stream.ToArray();
            }
        }
    }
}

// Custom Engine to Bypass Font Scanning
public class FallbackGraphicEngine : IXLGraphicEngine
{
    public double GetTextWidth(string text, IXLFontBase font, double dpi)
    {
        return (text ?? "").Length * 7.0 * (font.FontSize / 11.0); 
    }

    public double GetTextHeight(IXLFontBase font, double dpi)
    {
        return font.FontSize * 1.3; 
    }

    public double GetFontAscent(IXLFontBase font, double dpi)
    {
        return font.FontSize; 
    }

    public double GetFontDescent(IXLFontBase font, double dpi)
    {
        return font.FontSize * 0.3;
    }
    
    public double GetDescent(IXLFontBase font, double dpi)
    {
        return font.FontSize * 0.3;
    }

    public double GetMaxDigitWidth(IXLFontBase font, double dpi)
    {
        return 7.0 * (font.FontSize / 11.0); 
    }

    public GlyphBox GetGlyphBox(ReadOnlySpan<int> glyphs, IXLFontBase font, Dpi dpi)
    {
        // GlyphBox constructor adjustment to 4 arguments (xMin, yMin, xMax, yMax) or similar
        float size = (float)GetTextHeight(font, dpi.Y);
        return new GlyphBox(0, 0, size); 
    }

    public XLPictureInfo GetPictureInfo(Stream stream, XLPictureFormat format)
    {
        // Compiler indicated 3rd argument should be Size, not Dpi
        return new XLPictureInfo(format, new Size(100, 100), new Size(96, 96));
    }
}