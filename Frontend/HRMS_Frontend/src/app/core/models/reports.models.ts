export interface ComprehensiveDashboardDto {
  reportDate: string;
  attendanceMetrics: AttendanceMetricsDto;
  personnelMetrics: PersonnelMetricsDto;
  requestsMetrics: RequestsMetricsDto;
  financialMetrics: FinancialMetricsDto;
  holidayMetrics: HolidayMetricDto[];
  weeklyMetrics: WeeklyAnalyticsDto;
  monthlyMetrics: MonthlyAnalyticsDto;
  performanceMetrics: PerformanceMetricsDto;
  setupMetrics: SetupMetricsDto;
}

export interface AttendanceMetricsDto {
  totalPresent: number;
  totalAbsent: number;
  totalLeaves: number;
  totalLate: number;
  shiftDistribution: { [key: string]: number };
}

export interface PersonnelMetricsDto {
  totalEmployees: number;
  activeEmployees: number;
  inactiveEmployees: number;
  expiringDocumentsCount: number;
  departmentStats: DepartmentStatDto[];
}

export interface DepartmentStatDto {
  departmentId: number;
  departmentName: string;
  employeeCount: number;
}

export interface RequestsMetricsDto {
  pendingLeaveRequests: number;
  pendingOvertimeRequests: number;
  pendingLoanRequests: number;
  pendingShiftSwaps: number;
  pendingPermissions: number;
}

export interface FinancialMetricsDto {
  totalPendingSalaries: number;
  pendingSalariesByDepartment: { [key: string]: number };
  totalActiveLoansAmount: number;
  activeLoansCount: number;
}

export interface HolidayMetricDto {
  holidayType: string;
  daysCount: number;
  holidaysCount: number;
}

export interface WeeklyAnalyticsDto {
  attendanceTrend: DailyAttendanceSummaryDto[];
  totalHoursWorked: number;
  totalOvertimeMinutes: number;
}

export interface DailyAttendanceSummaryDto {
  date: string;
  presentCount: number;
  absentCount: number;
  lateCount: number;
}

export interface MonthlyAnalyticsDto {
  totalPresent: number;
  totalAbsent: number;
  totalLate: number;
  totalLeaves: number;
  attendanceRate: number;
  newHires: number;
  resignations: number;
  totalNetSalary: number;
  totalBasicSalary: number;
  totalAllowances: number;
  totalDeductions: number;
  salaryByDepartment: { [key: string]: number };
}

export interface PerformanceMetricsDto {
  activeAppraisalCycles: number;
  pendingEvaluations: number;
  averageCompanyRating: number;
}

export interface SetupMetricsDto {
  totalDepartments: number;
  totalJobTitles: number;
  totalShiftTypes: number;
  totalActiveUsers: number;
}
