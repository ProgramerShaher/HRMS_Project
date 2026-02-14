export interface Result<T> {
    succeeded: boolean;
    message: string;
    data: T;
    errors: string[];
}

export interface EmployeeViolation {
    violationId: number;
    employeeId: number;
    employeeName: string;
    violationTypeId: number;
    violationTypeNameAr: string;
    actionId: number;
    actionNameAr: string;
    deductionDays: number;
    deductionAmount?: number;
    violationDate: string;
    description?: string;
    status: string; // PENDING, APPROVED, REJECTED
    isExecuted: boolean;
    executionDate?: string;
}

export interface RegisterViolationCommand {
    employeeId: number;
    violationTypeId: number;
    actionId?: number;
    description?: string;
    violationDate: string;
}

export interface EmployeeAppraisal {
    appraisalId: number;
    employeeId: number;
    employeeName: string;
    cycleId: number;
    cycleName: string;
    evaluatorId: number;
    evaluatorName: string;
    appraisalDate: string;
    finalScore: number;
    grade: string;
    status: string;
    employeeComment?: string;
    details: AppraisalDetail[];
}

export interface AppraisalDetail {
    kpiId: number;
    kpiName: string;
    weight?: number;
    score: number;
    comments?: string;
}

export interface SubmitAppraisalCommand {
    employeeId: number;
    cycleId: number;
    kpiDetails: KpiDetailCommand[];
    employeeComment?: string;
    manualEvaluatorId?: number;
}

export interface KpiDetailCommand {
    kpiId: number;
    score: number;
    comments?: string;
}
