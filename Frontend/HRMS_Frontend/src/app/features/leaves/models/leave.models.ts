// ═══════════════════════════════════════════════════════
// Leave Type Model
// ═══════════════════════════════════════════════════════
export interface LeaveType {
  leaveTypeId: number;
  leaveTypeNameAr: string;
  leaveTypeNameEn?: string;
  defaultDays: number;
  isDeductible: number;
  requiresAttachment: number;
  isActive?: boolean;
}

// ═══════════════════════════════════════════════════════
// Leave Request Model
// ═══════════════════════════════════════════════════════
export interface LeaveRequest {
  requestId?: number;
  employeeId: number;
  leaveTypeId: number;
  startDate: string;
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
  employeeNumber?: string;
  departmentName?: string;
  approverComments?: string;
}

export type LeaveRequestStatus =
  | 'PENDING'
  | 'MANAGER_APPROVED'
  | 'HR_APPROVED'
  | 'APPROVED'
  | 'REJECTED'
  | 'CANCELLED';

// ═══════════════════════════════════════════════════════
// Leave Balance Models
// ═══════════════════════════════════════════════════════
export interface LeaveBalance {
  balanceId: number;
  employeeId: number;
  leaveTypeId: number;
  leaveTypeName: string;
  currentBalance: number;
  year: number;
  entitlementDays?: number;
  consumedDays?: number;
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

// ═══════════════════════════════════════════════════════
// Balance Management DTOs
// ═══════════════════════════════════════════════════════
export interface InitializeBalancesDto {
  leaveTypeId?: number;
  year: number;
  departmentId?: number;
  customDays?: number;
  enableProration?: boolean;
}

export interface AdjustBalanceCommand {
  employeeId: number;
  leaveTypeId: number;
  year?: number;
  adjustmentDays: number;
  reason: string;
}

// ═══════════════════════════════════════════════════════
// Leave Transaction Model
// ═══════════════════════════════════════════════════════
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

export type TransactionType =
  | 'ACCRUAL'
  | 'DEDUCTION'
  | 'ADJUSTMENT'
  | 'CANCELLATION'
  | 'CARRY_FORWARD';

// ═══════════════════════════════════════════════════════
// Public Holiday Model
// ═══════════════════════════════════════════════════════
export interface PublicHoliday {
  holidayId?: number;
  holidayNameAr: string;
  startDate: string;
  endDate: string;
  year: number;
}

// ═══════════════════════════════════════════════════════
// API Response Wrapper
// ═══════════════════════════════════════════════════════
export interface ApiResponse<T> {
  data: T;
  succeeded: boolean;
  message: string;
  errors?: string[];
  statusCode: number;
}

// ═══════════════════════════════════════════════════════
// Create/Action DTOs
// ═══════════════════════════════════════════════════════
export interface CreateLeaveRequestDto {
  employeeId: number;
  leaveTypeId: number;
  startDate: string;
  endDate: string;
  reason: string;
  attachmentPath?: string;
}

export interface LeaveRequestFilter {
  employeeId?: number;
  status?: LeaveRequestStatus;
  fromDate?: string;
  toDate?: string;
  leaveTypeId?: number;
}

// ═══════════════════════════════════════════════════════
// Dashboard Stats
// ═══════════════════════════════════════════════════════
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
