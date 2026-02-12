export interface Result<T> {
    data: T;
    succeeded: boolean;
    message: string;
    errors: string[];
    statusCode: number;
}

// Enums
export enum ShiftType {
    Morning = 'Morning',
    Evening = 'Evening',
    Night = 'Night'
}

export enum AttendanceStatus {
    Present = 'Present',
    Absent = 'Absent',
    Late = 'Late',
    Leave = 'Leave',
    Holiday = 'Holiday',
    Off = 'Off'
}

export enum RequestStatus {
    Pending = 'Pending',
    Approved = 'Approved',
    Rejected = 'Rejected',
    Cancelled = 'Cancelled'
}

// DTOs
export interface RegisterPunchCommand {
    employeeId: number;
    punchType: 'IN' | 'OUT';
    punchTime: string; // ISO Date
    deviceId?: string;
    locationCoordinates?: string;
}

export interface UpdateShiftSwapCommand {
    requestId: number;
    targetEmployeeId: number;
    reason: string;
}

export interface RevokeShiftSwapCommand {
    requestId: number;
}

export interface UpdateOvertimeCommand {
    requestId: number;
    hoursRequested: number;
    reason: string;
}

export interface AttendanceStatsDto {
    totalLateMinutes: number;
    totalOTMinutes: number;
    totalAbsenceDays: number;
    totalPresentDays: number;
    attendancePercentage: number;
}

export interface PermissionRequestDto {
    permissionRequestId: number;
    employeeId: number;
    employeeName: string;
    permissionDate: string;
    permissionType: string;
    hours: number;
    reason: string;
    status: string;
    rejectionReason?: string;
    createdAt: string;
    approverId?: number;
    approverName?: string;
}

export interface OvertimeRequestDto {
    overtimeRequestId: number;
    employeeId: number;
    employeeName: string;
    requestDate: string;
    hoursRequested: number;
    hoursApproved?: number;
    reason: string;
    status: string;
    managerComment?: string;
    createdAt: string;
    managerId?: number;
    managerName?: string;
}

export interface ShiftSwapRequestDto {
    swapRequestId: number;
    requesterId: number;
    requesterName: string;
    targetEmployeeId: number;
    targetEmployeeName: string;
    rosterDate: string;
    reason: string;
    status: string;
    managerComment?: string;
    createdAt: string;
}

export interface TimesheetDayDto {
  date: string;
  dayName: string;
  status: string;
  inTime?: string;
  outTime?: string;
  lateMinutes: number;
  otMinutes: number;
  remarks?: string;
}

export interface LiveStatusDto {
    totalEmployees: number;
    currentlyIn: number;
    checkedOut: number;
    notYetIn: number;
    onLeave: number;
    absent: number;
}

export interface AttendanceExceptionDto {
    recordId: number;
    employeeId: number;
    employeeName: string;
    date: string;
    inTime?: string;
    outTime?: string;
    issue: string;
}

export interface ManualCorrectionCommand {
    dailyAttendanceId: number;
    correctionType: string;
    newValue: string;
    auditNote: string;
}

export interface PayrollAttendanceSummaryDto {
    employeeId: number;
    employeeName: string;
    fullNameAr: string;
    departmentName: string;
    totalDeepLateMinutes: number;
    totalShortLateMinutes: number;
    totalAbsenceDays: number;
    totalSickLeaveDays: number;
    totalUnpaidLeaveDays: number;
    totalOvertimeMinutes: number;
    proposedDeductionAmount: number;
}

export interface CreatePermissionRequestCommand {
    employeeId: number;
    permissionDate: string;
    permissionType: string;
    hours: number;
    reason: string;
}

export interface ApproveRejectPermissionRequestCommand {
    permissionRequestId: number;
    action: 'Approve' | 'Reject';
    rejectionReason?: string;
    approverId: number;
}

export interface MyRosterDto {
    date: string;
    dayName: string;
    shiftName: string;
    startTime: string;
    endTime: string;
    isOffDay: boolean;
    actualInTime?: string;
    actualOutTime?: string;
    status: string;
}

export interface ShiftTypeDto {
    shiftId: number;
    shiftNameAr: string;
    startTime: string;
    endTime: string;
    hoursCount: number;
    isCrossDay: number;
}

export interface CreateShiftTypeCommand {
    shiftNameAr: string;
    startTime: string;
    endTime: string;
    isCrossDay: number;
}

export interface InitializeRosterCommand {
    employeeId?: number; // Optional for bulk
    startDate: string;
    endDate: string;
    shiftId: number;
}

export interface AssignShiftCommand {
    employeeId: number;
    shiftId: number;
    month: number;
    year: number;
    offDays: number[];
}

export interface CreateShiftSwapCommand {
    requesterId: number;
    targetEmployeeId: number;
    rosterDate: string;
    reason: string;
}

export interface ActionShiftSwapCommand {
    requestId: number;
    managerId: number;
    action: 'Approve' | 'Reject';
    comment?: string;
}

export interface ApplyOvertimeCommand {
    employeeId: number;
    workDate: string;
    hoursRequested: number;
    reason: string;
}

export interface ActionOvertimeCommand {
    requestId: number;
    managerId: number;
    action: 'Approve' | 'Reject';
    approvedHours?: number;
    comment?: string;
}


export interface ShiftTypeDto {
    shiftId: number;
    shiftNameAr: string;
    startTime: string;
    endTime: string;
    hoursCount: number;
    isCrossDay: number;
}

export interface CreateShiftTypeCommand {
    shiftNameAr: string;
    startTime: string;
    endTime: string;
    isCrossDay: number;
    gracePeriodMins?: number;
}

export interface InitializeRosterCommand {
    employeeId?: number; // Optional for bulk
    startDate: string;
    endDate: string;
    shiftId: number;
}

export interface AssignShiftCommand {
    employeeId: number;
    shiftId: number;
    month: number;
    year: number;
    offDays: number[];
}

export interface CreateShiftSwapCommand {
    requesterId: number;
    targetEmployeeId: number;
    rosterDate: string;
    reason: string;
}

export interface ActionShiftSwapCommand {
    requestId: number;
    managerId: number;
    action: 'Approve' | 'Reject';
    comment?: string;
}

export interface ApplyOvertimeCommand {
    employeeId: number;
    workDate: string;
    hoursRequested: number;
    reason: string;
}

export interface ActionOvertimeCommand {
    requestId: number;
    managerId: number;
    action: 'Approve' | 'Reject';
    approvedHours?: number;
    comment?: string;
}

export interface MonthlyClosingResultDto {
    totalEmployeesProcessed: number;
    totalLateMinutesCharged: number;
    totalOvertimeMinutes: number;
    lockedPeriodId: number;
    closedAt: string;
}

export interface ProcessAttendanceCommand {
    targetDate: string;
}
