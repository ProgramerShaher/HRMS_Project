using System;
using System.Collections.Generic;

namespace HRMS.Application.DTOs.Reports.Analytics
{
    public class WeeklyAnalyticsDto
    {
        public List<AnalyticsDailyAttendanceSummaryDto> AttendanceTrend { get; set; } = new();
        public double TotalHoursWorked { get; set; }
        public int TotalOvertimeMinutes { get; set; }
    }
}
