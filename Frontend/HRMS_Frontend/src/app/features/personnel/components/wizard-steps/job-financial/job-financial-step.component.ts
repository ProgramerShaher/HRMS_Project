import { Component, Input, Output, EventEmitter, OnInit, inject, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { SelectModule } from 'primeng/select';
import { InputNumberModule } from 'primeng/inputnumber';
import { InputTextModule } from 'primeng/inputtext';
import { DatePickerModule } from 'primeng/datepicker';
import { ButtonModule } from 'primeng/button';
import { DepartmentService } from '../../../../setup/services/department.service';
import { SetupService } from '../../../../setup/services/setup.service';
import { Job } from '../../../../setup/models/setup.models';
import { CreateEmployeeDto } from '../../../models/create-employee.dto';
import { MessageService } from 'primeng/api';

@Component({
  selector: 'app-job-financial-step',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    SelectModule,
    InputNumberModule,
    InputTextModule,
    DatePickerModule,
    ButtonModule
  ],
  templateUrl: './job-financial-step.component.html',
  styles: []
})
export class JobFinancialStepComponent implements OnInit {
  @Input() data!: CreateEmployeeDto;
  @Output() dataChange = new EventEmitter<Partial<CreateEmployeeDto>>();
  @Output() prev = new EventEmitter<void>();
  @Output() next = new EventEmitter<void>();

  private departmentService = inject(DepartmentService);
  private setupService = inject(SetupService);
  private messageService = inject(MessageService);
  private cdr = inject(ChangeDetectorRef);

  departments: any[] = [];
  jobs: any[] = [];
  jobGrades: any[] = [];
  banks: any[] = [];

  ngOnInit() {
    this.loadLookups();
  }

  loadLookups() {
    // Departments
    this.departmentService.getAll().subscribe({
        next: (res) => {
            this.departments = res.map(d => ({ label: d.deptNameAr, value: d.deptId }));
            this.cdr.markForCheck();
        }
    });

    // Jobs
    this.setupService.getAll('Jobs').subscribe({
        next: (res: any) => {
             const items: Job[] = res.data?.items || res.items || res.data || res || [];
             this.jobs = items.map(j => ({ label: j.jobTitleAr, value: j.jobId }));
             this.cdr.markForCheck();
        }
    });

    // Job Grades
    this.setupService.getAll('JobGrades').subscribe({
        next: (res: any) => {
            console.log('JobGrades Response:', res);
            const items: any[] = res.data?.items || res.items || res.data || res || [];
            this.jobGrades = items.map(g => ({ 
                label: g.gradeNameAr, 
                value: g.jobGradeId,
                minSalary: g.minSalary,
                maxSalary: g.maxSalary
            }));
            if (this.jobGrades.length === 0) {
                console.warn('No Job Grades found!');
            }
            this.cdr.markForCheck();
        },
        error: (err) => {
            console.error('Failed to load JobGrades', err);
            this.messageService.add({severity: 'error', summary: 'خطأ', detail: 'فشل تحميل الدرجات الوظيفية'});
        }
    });

    // Banks
    this.setupService.getAll('Banks').subscribe({
        next: (res: any) => {
            const items: any[] = res.data?.items || res.items || res.data || res || [];
            this.banks = items.map(b => ({ label: b.bankNameAr, value: b.bankId }));
            this.cdr.markForCheck();
        }
    });
  }

  // Helper for Salary Validation
  selectedGradeMinSalary: number | null = null;
  selectedGradeMaxSalary: number | null = null;

  onGradeChange() {
      const grade = this.jobGrades.find(g => g.value === this.data.jobGradeId);
      if (grade) {
          this.selectedGradeMinSalary = grade.minSalary;
          this.selectedGradeMaxSalary = grade.maxSalary;
          
          // Auto-set basic salary if 0? Optional. 
          // For now just hint.
      } else {
          this.selectedGradeMinSalary = null;
          this.selectedGradeMaxSalary = null;
      }
      this.onChange();
  }

  onChange() {
    this.dataChange.emit(this.data);
  }

  get totalSalary(): number {
    return (this.data.basicSalary || 0) + 
           (this.data.housingAllowance || 0) + 
           (this.data.transportAllowance || 0) + 
           (this.data.medicalAllowance || 0);
  }

  get isSalaryInvalid(): boolean {
    if (this.data.basicSalary === undefined || this.data.basicSalary === null) return false;
    if (this.selectedGradeMinSalary !== null && this.data.basicSalary < this.selectedGradeMinSalary) return true;
    if (this.selectedGradeMaxSalary !== null && this.data.basicSalary > this.selectedGradeMaxSalary) return true;
    return false;
  }

  onPrev() {
    this.prev.emit();
  }

  onNext() {
    if (!this.data.departmentId || !this.data.jobId || !this.data.hireDate || !this.data.basicSalary) {
       this.messageService.add({ severity: 'warn', summary: 'تنبيه', detail: 'يرجى تعبئة الحقول الإلزامية (القسم، الوظيفة، تاريخ التعيين، الراتب الأساسي)' });
       return; 
    }
    this.next.emit();
  }
}
