using System;

namespace HRMS.Application.DTOs.Reports.Analytics
{
    public class MonthlyAnalyticsDto
    {
        public int TotalPresent { get; set; }
        public int TotalAbsent { get; set; }
        public int TotalLate { get; set; }
        public int TotalLeaves { get; set; }
        public double AttendanceRate { get; set; }
        public int NewHires { get; set; }
        public int Resignations { get; set; }

        // Payroll Metrics for the month
        public decimal TotalNetSalary { get; set; }
        public decimal TotalBasicSalary { get; set; }
        public decimal TotalAllowances { get; set; }
        public decimal TotalDeductions { get; set; }
        public Dictionary<string, decimal> SalaryByDepartment { get; set; } = new();
    }
}
