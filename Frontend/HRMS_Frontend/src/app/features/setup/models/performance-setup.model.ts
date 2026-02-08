export interface ViolationType {
  violationTypeId: number;
  violationNameAr: string;
  violationNameEn: string;
  description?: string; // Optional if not always required
  severityLevel: number;
}

export interface DisciplinaryAction {
  actionId: number;
  actionNameAr: string;
  deductionDays: number;
  isTermination: number; // Using number as per user schema (0/1 likely) or boolean if mapped
}

export interface Kpi {
  kpiId: number;
  kpiNameAr: string;
  kpiDescription?: string;
  category: string;
  measurementUnit: string;
}

export interface AppraisalCycle {
  cycleId: number;
  cycleName: string;
  startDate: string; // ISO Date
  endDate: string; // ISO Date
  status: string;
}

// Result Wrapper (assuming standard wrapper based on controller)
export interface Result<T> {
  data: T;
  succeeded: boolean;
  message: string;
  errors: string[];
  statusCode: number;
}
