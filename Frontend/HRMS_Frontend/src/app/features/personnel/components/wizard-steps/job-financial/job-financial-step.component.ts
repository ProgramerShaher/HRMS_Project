import { Component, EventEmitter, Input, Output, inject, OnInit, computed, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { InputTextModule } from 'primeng/inputtext';
import { InputNumberModule } from 'primeng/inputnumber';
import { DatePickerModule } from 'primeng/datepicker';
import { SelectModule } from 'primeng/select';
import { CreateEmployeeDto } from '../../../models/create-employee.dto';
import { LookupService, Department, Job, JobGrade, EmployeeListItem } from '../../../../../core/services/lookup.service';

@Component({
  selector: 'app-job-financial-step',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    InputTextModule,
    InputNumberModule,
    DatePickerModule,
    SelectModule
  ],
  templateUrl: './job-financial-step.component.html'
})
export class JobFinancialStepComponent implements OnInit {
  @Input() data!: CreateEmployeeDto;
  @Output() dataChange = new EventEmitter<Partial<CreateEmployeeDto>>();

  private lookupService = inject(LookupService);

  departments = signal<Department[]>([]);
  jobs = signal<Job[]>([]);
  jobGrades = signal<JobGrade[]>([]);
  managers = signal<EmployeeListItem[]>([]);
  
  // Reactivity: Use getters for real-time template updates
  get totalAllowances(): number {
    return (this.data.housingAllowance || 0) + 
           (this.data.transportAllowance || 0) + 
           (this.data.medicalAllowance || 0);
  }

  get grandTotal(): number {
    return (this.data.basicSalary || 0) + this.totalAllowances;
  }

  ngOnInit() {
    this.loadLookups();
  }

  loadLookups() {
    this.lookupService.getDepartments().subscribe(depts => this.departments.set(depts));
    this.lookupService.getJobs().subscribe(jobs => this.jobs.set(jobs));
    this.lookupService.getJobGrades().subscribe(grades => this.jobGrades.set(grades));
    this.lookupService.getActiveEmployees().subscribe(emps => this.managers.set(emps));
  }

  onDataChange() {
    // We emit a copy to ensure parent signal sees a change if it checks references
    this.dataChange.emit({ ...this.data });
  }

  // Suggest default grade when job changes (optional enhancement)
  onJobChange() {
    const selectedJob = this.jobs().find(j => j.jobId === this.data.jobId);
    if (selectedJob && selectedJob.jobGradeId) {
      if (!this.data.jobGradeId) {
        this.data.jobGradeId = selectedJob.jobGradeId;
      }
    }
    this.onDataChange();
  }
}
