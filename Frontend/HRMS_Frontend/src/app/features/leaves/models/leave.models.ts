// Leave Type Model
export interface LeaveType {
  leaveTypeId: number;
  leaveTypeNameAr: string;
  nameEn?: string;
  defaultDays: number;
  isDeductible: number;
  requiresAttachment: number;
}

// Leave Request Model
export interface LeaveRequest {
  requestId?: number;
  employeeId: number;
  leaveTypeId: number;
  startDate: string; // ISO date string
  endDate: string;
  daysCount: number;
  reason: string;
  status: LeaveRequestStatus;
  isPostedToBalance?: number;
  rejectionReason?: string;
  attachmentPath?: string;
  createdAt?: string;
  
  // Navigation properties (populated from backend)
  leaveTypeName?: string;
  employeeName?: string;
}

export enum LeaveRequestStatus {
  PENDING = 'PENDING',
  MANAGER_APPROVED = 'MANAGER_APPROVED',
  HR_APPROVED = 'HR_APPROVED',
  APPROVED = 'APPROVED',
  REJECTED = 'REJECTED',
  CANCELLED = 'CANCELLED'
}

// Leave Balance Model
export interface LeaveBalance {
  balanceId: number;
  employeeId: number;
  leaveTypeId: number;
  leaveTypeName: string;
  currentBalance: number;
  year: number;
}

export interface EmployeeLeaveTypeBalance {
  employeeId: number;
  employeeNumber: string;
  employeeNameAr: string;
  departmentId: number;
  departmentNameAr: string;
  leaveTypeId: number;
  leaveTypeNameAr: string;
  year: number;
  entitlementDays: number;
  consumedDays: number;
  remainingDays: number;
}

// Leave Transaction Model
export interface LeaveTransaction {
  transactionId: number;
  employeeId: number;
  employeeName: string;
  leaveTypeId: number;
  leaveTypeName: string;
  transactionType: TransactionType;
  days: number;
  transactionDate: string;
  notes?: string;
  referenceId?: number;
}

export enum TransactionType {
  ACCRUAL = 'ACCRUAL',
  DEDUCTION = 'DEDUCTION',
  ADJUSTMENT = 'ADJUSTMENT',
  CANCELLATION = 'CANCELLATION',
  CARRY_FORWARD = 'CARRY_FORWARD'
}

// Public Holiday Model
export interface PublicHoliday {
  holidayId?: number;
  holidayNameAr: string;
  startDate: string;
  endDate: string;
  year: number;
}

// API Response Wrapper
export interface ApiResponse<T> {
  data: T;
  succeeded: boolean;
  message: string;
  errors?: string[];
  statusCode: number;
}

// Create Leave Request DTO
export interface CreateLeaveRequestDto {
  employeeId: number;
  leaveTypeId: number;
  startDate: string;
  endDate: string;
  reason: string;
  attachmentPath?: string;
}

// Leave Request Filter
export interface LeaveRequestFilter {
  employeeId?: number;
  status?: LeaveRequestStatus;
  fromDate?: string;
  toDate?: string;
  leaveTypeId?: number;
}

// Dashboard Stats
export interface LeaveDashboardStats {
  totalEntitlement: number;
  totalRequestedDays: number;
  consumedDays: number;
  remainingDays: number;
  pendingRequestsCount: number;
  approvedRequestsCount: number;
  rejectedRequestsCount: number;
  leaveTypeSummaries: LeaveTypeSummary[];
}

export interface LeaveTypeSummary {
  leaveTypeId: number;
  leaveTypeNameAr: string;
  leaveTypeNameEn: string;
  totalDays: number;
  consumedDays: number;
  remainingDays: number;
}
