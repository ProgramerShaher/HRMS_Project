// Matches HRMS.Application.DTOs.Core.JobGradeDto
export interface JobGradeDto {
  jobGradeId: number;
  gradeCode: string;
  gradeNameAr: string;
  gradeNameEn: string;
  gradeLevel: number;
  minSalary: number;
  maxSalary: number;
  benefitsConfig?: string | null;
  description?: string | null;
}

// Matches HRMS.Application.Features.Core.JobGrades.Commands.CreateJobGrade.CreateJobGradeCommand
export interface CreateJobGradeCommand {
  gradeCode: string;
  gradeNameAr: string;
  gradeNameEn: string;
  gradeLevel: number;
  minSalary: number;
  maxSalary: number;
  benefitsConfig?: string | null;
  description?: string | null;
}

// Matches HRMS.Application.Features.Core.JobGrades.Commands.UpdateJobGrade.UpdateJobGradeCommand
export interface UpdateJobGradeCommand {
  jobGradeId: number;
  gradeCode: string;
  gradeNameAr: string;
  gradeNameEn: string;
  gradeLevel: number;
  minSalary: number;
  maxSalary: number;
  benefitsConfig?: string | null;
  description?: string | null;
}
