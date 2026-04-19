// ═══════════════════════════════════════════════════════════
// RECRUITMENT MODULE - TYPE-SAFE INTERFACES
// Matching exactly: HRMS.Application.DTOs.Recruitment
// ═══════════════════════════════════════════════════════════

// ── Config / Lookups ──────────────────────────────────────
export interface JobGrade {
  jobGradeId: number;
  gradeNameAr: string;
  gradeNameEn: string;
  minSalary: number;
  maxSalary: number;
}

export interface LookupItem {
  id: number;
  nameAr: string;
  nameEn: string;
}

// ── Candidate (CandidateDto) ──────────────────────────────
export interface Candidate {
  candidateId: number;
  firstNameAr: string;
  familyNameAr?: string;
  fullNameEn: string;
  email: string;
  phone?: string;
  nationalityId?: number;
  nationalityName?: string;
  resume?: string;
  linkedinProfile?: string;
  applicationSource: string;
  status: string;
}

export interface CreateCandidateCommand {
  fullNameEn: string;
  firstNameAr?: string;
  familyNameAr?: string;
  email: string;
  phone?: string;
  nationalityId?: number;
  linkedinProfile?: string;
  cvFilePath?: string;
}

export interface UpdateCandidateCommand {
  candidateId: number;
  fullNameEn: string;
  firstNameAr?: string;
  familyNameAr?: string;
  email: string;
  phone?: string;
  nationalityId?: number;
  linkedinProfile?: string;
  cvFilePath?: string;
}

// ── Vacancy (JobVacancyDto) ───────────────────────────────
export interface Vacancy {
  vacancyId: number;
  vacancyTitle: string;
  jobId: number;
  jobTitle: string;
  deptId: number;
  departmentName: string;
  positionsCount: number;
  requirements : string;
  postedDate: string;
  closingDate?: string;
  status: string;
  minSalary?: number;
  maxSalary?: number;
  description?: string;
}

export interface CreateVacancyCommand {
  jobId: number;
  departmentId: number;
  numberOfPositions: number;
  description: string;
  requirements: string;
  closingDate: string;
}

export interface UpdateVacancyCommand {
  vacancyId: number;
  jobId: number;
  departmentId: number;
  numberOfPositions: number;
  description: string;
  requirements: string;
  closingDate: string;
}

// ── Application (JobApplicationDto) ──────────────────────
export interface JobApplication {
  applicationId: number;
  vacancyId: number;
  vacancyTitle: string;
  candidateId: number;
  candidateName: string;
  applicationDate: string;
  status: string; // APPLIED | SCREENING | SHORTLISTED | INTERVIEW | OFFERED | HIRED | REJECTED
  coverLetter?: string;
}

export interface SubmitApplicationCommand {
  vacancyId: number;
  candidateId: number;
  source?: string;
}

export interface ChangeApplicationStatusCommand {
  appId: number;
  status: string;
  notes?: string;
}

// ── Interview (InterviewDto) ──────────────────────────────
export interface Interview {
  interviewId: number;
  applicationId: number;
  candidateId: number;
  candidateName: string;
  interviewDate: string;
  interviewType?: string;
  interviewerId?: number;
  interviewerName?: string;
  status: string;
  score?: number;
  feedback?: string;
}

export interface RecruitmentSummary {
  totalCandidates: number;
  openVacancies: number;
  activeApplications: number;
  hiredThisMonth: number;
  upcomingInterviews: number;
}

export interface ChartItem {
  label: string;
  value: number;
  color?: string;
}

export interface RecruitmentReports {
  summary: RecruitmentSummary;
  candidatesByStatus: ChartItem[];
  candidatesBySource: ChartItem[];
  vacanciesByDepartment: ChartItem[];
  applicationPipeline: ChartItem[];
}

export interface ScheduleInterviewCommand {
  appId: number;
  interviewerId?: number;
  scheduledTime: string;
  interviewType: string; // IN_PERSON | ONLINE | PHONE
}

export interface RecordInterviewResultCommand {
  interviewId: number;
  result: string;    // PASSED | FAILED | NO_SHOW
  notes?: string;
  rating?: number;   // 1–5
}

export interface AcceptOfferCommand {
  offerId: number;
  joiningDate?: string;
  employeeNumber?: string;
  nationalId?: string;
  mobile?: string;
  birthDate?: string;
  gender?: string;         // M | F
  maritalStatus?: string;
}

// ── Offer (JobOfferDto) ───────────────────────────────────
export interface JobOffer {
  offerId: number;
  applicationId: number;
  candidateId: number;
  candidateName: string;
  vacancyId: number;
  vacancyTitle: string;
  offerDate: string;
  expiryDate?: string;
  basicSalary?: number;
  housingAllowance?: number;
  transportAllowance?: number;
  totalPackage?: number;
  status: string; // PENDING | SENT | ACCEPTED | REJECTED | WITHDRAWN
  joiningDate?: string;
}

export interface CreateOfferCommand {
  appId: number;
  jobGradeId: number;
  basicSalary: number;
  housingAllowance: number;
  transportAllowance: number;
  medicalAllowance: number;
  otherAllowances: number;
  joiningDate: string;
  offerDate: string;
  expiryDate: string;
  terms?: string;
}

// Application Status Flow (for Kanban / Pipeline)
export const APPLICATION_STATUSES = [
  { key: 'APPLIED',     labelAr: 'طلب جديد',         color: 'info' },
  { key: 'SCREENING',   labelAr: 'فحص أولي',          color: 'info' },
  { key: 'SHORTLISTED', labelAr: 'قائمة المرشحين',    color: 'warn' },
  { key: 'INTERVIEW',   labelAr: 'مرحلة المقابلة',    color: 'warn' },
  { key: 'OFFERED',     labelAr: 'تم تقديم عرض',     color: 'secondary' },
  { key: 'HIRED',       labelAr: 'تم التوظيف',        color: 'success' },
  { key: 'REJECTED',    labelAr: 'مرفوض',             color: 'danger' },
] as const;
