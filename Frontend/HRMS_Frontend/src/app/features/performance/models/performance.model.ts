// ── Generic Result wrapper ────────────────────────────────────────────────
export interface Result<T> {
    succeeded: boolean;
    message: string;
    data: T;
    errors: string[];
}

// ── Appraisal Cycle ───────────────────────────────────────────────────────
export interface AppraisalCycle {
    cycleId: number;
    cycleNameAr: string;
    startDate: string;
    endDate: string;
    isActive: number;
}

export interface CreateCycleCommand {
    cycleName: string;
    startDate: string;
    endDate: string;
    status: string;
}

// ── KPI Library ───────────────────────────────────────────────────────────
export interface KpiLibrary {
    kpiId: number;
    kpiNameAr: string;
    kpiDescription?: string;
    category?: string;
    measurementUnit?: string;
    weight: number;
    targetJobType?: string;
}

export interface CreateKpiCommand {
    kpiNameAr: string;
    kpiDescription?: string;
    category?: string;
    measurementUnit?: string;
    weight: number;
    targetJobType?: string;
}

// ── Appraisal Detail ──────────────────────────────────────────────────────
export interface AppraisalDetail {
    detailId?: number;
    kpiId: number;
    kpiName: string;
    kpiCategory?: string;
    weight: number;
    targetValue?: number;
    actualValue?: number;
    score: number;
    employeeScore: number;
    managerScore: number;
    finalScore: number;
    comments?: string;
}

// ── Employee Appraisal ────────────────────────────────────────────────────
export interface EmployeeAppraisal {
    appraisalId: number;
    employeeId: number;
    employeeName: string;
    cycleId: number;
    cycleName: string;
    cycleStartDate?: string;
    cycleEndDate?: string;
    evaluatorId: number;
    evaluatorName: string;
    appraisalDate: string;
    finalScore: number;
    grade: string;
    status: string;
    employeeComment?: string;
    comments?: string;
    details: AppraisalDetail[];
}

// ── Commands ──────────────────────────────────────────────────────────────
export interface InitiateAppraisalCommand {
    employeeId: number;
    cycleId: number;
    evaluatorId?: number;
}

export interface SubmitSelfAppraisalCommand {
    scores:
     { detailId: number; 
        employeeScore: number; 
        comments?: string }[];
    employeeComment?: string;
}

export interface SubmitManagerAppraisalCommand {
    scores: { detailId: number; managerScore: number; comments?: string }[];
    managerComment?: string;
}

export interface FinalizeAppraisalCommand {
    scores: { detailId: number; finalScore: number }[];
}

// ── Violations ────────────────────────────────────────────────────────────
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
    status: string;
    isExecuted: boolean;
    executionDate?: string;
}

export interface RegisterViolationCommand {
    employeeId: number;
    violationTypeId: number;
    actionId?: number;
    description?: string;
    violationDate: string | Date;
}

export interface UpdateViolationCommand {
    violationId: number;
    violationTypeId: number;
    actionId?: number;
    description?: string;
    violationDate: string | Date;
}

// ── Dashboard / Results ───────────────────────────────────────────────────
export interface KpiScoreChart {
    kpiName: string;
    category?: string;
    weight: number;
    employeeScore: number;
    managerScore: number;
    finalScore: number;
    weightedContribution: number;
}

// ── Legacy (kept for backward compat) ────────────────────────────────────
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
