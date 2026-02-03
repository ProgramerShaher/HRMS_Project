import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { InputNumberModule } from 'primeng/inputnumber';
import { DynamicDialogRef, DynamicDialogConfig } from 'primeng/dynamicdialog';
import { SetupService } from '../../services/setup.service';
import { MessageService } from 'primeng/api';
import { forkJoin } from 'rxjs';
import { SelectModule } from 'primeng/select';

@Component({
  selector: 'app-attendance-policy-form',
  standalone: true,
  imports: [
    CommonModule, 
    ReactiveFormsModule, 
    ButtonModule, 
    InputTextModule, 
    InputNumberModule,
    SelectModule
  ],
  templateUrl: './attendance-policy-form.component.html',
  styleUrls: ['./attendance-policy-form.component.scss']
})
export class AttendancePolicyFormComponent implements OnInit {
  fb = inject(FormBuilder);
  ref = inject(DynamicDialogRef);
  config = inject(DynamicDialogConfig);
  setupService = inject(SetupService);
  messageService = inject(MessageService);

  form!: FormGroup;
  isEditMode = false;
  loading = false;
  
  departments = signal<any[]>([]);
  jobs = signal<any[]>([]);

  ngOnInit() {
    this.initForm();
    this.loadDependencies();
    
    if (this.config.data && this.config.data.policyId) {
      this.isEditMode = true;
      this.form.patchValue(this.config.data);
    }
  }

  initForm() {
    this.form = this.fb.group({
      policyId: [null],
      policyNameAr: ['', [Validators.required]],
      deptId: [null], // Optional
      jobId: [null],  // Optional
      lateGraceMins: [0, [Validators.required, Validators.min(0)]],
      overtimeMultiplier: [1.5, [Validators.required, Validators.min(1)]],
      weekendOtMultiplier: [2, [Validators.required, Validators.min(1)]]
    });
  }

  loadDependencies() {
    // Assuming endpoints are 'departments' and 'jobs'
    // We adjust extraction based on response format
    const depts$ = this.setupService.getAll<any>('departments');
    const jobs$ = this.setupService.getAll<any>('jobs');

    forkJoin([depts$, jobs$]).subscribe({
      next: ([deptsRes, jobsRes]: [any, any]) => {
        // Departments
        const depts = Array.isArray(deptsRes) ? deptsRes : (Array.isArray(deptsRes?.data) ? deptsRes.data : []);
        this.departments.set(depts);

        // Jobs
        const jobsList = Array.isArray(jobsRes) ? jobsRes : (Array.isArray(jobsRes?.data) ? jobsRes.data : []);
        this.jobs.set(jobsList);
      }
    });
  }

  onSubmit() {
    if (this.form.invalid) return;

    this.loading = true;
    const formValue = this.form.value;

    if (this.isEditMode) {
      this.setupService.update('AttendancePolicy', formValue.policyId, formValue).subscribe({
        next: () => {
          this.messageService.add({ severity: 'success', summary: 'نجاح', detail: 'تم التعديل بنجاح' });
          this.ref.close(true);
          this.loading = false;
        },
        error: () => this.loading = false
      });
    } else {
      this.setupService.create('AttendancePolicy', formValue).subscribe({
        next: () => {
          this.messageService.add({ severity: 'success', summary: 'نجاح', detail: 'تم الإضافة بنجاح' });
          this.ref.close(true);
          this.loading = false;
        },
        error: () => this.loading = false
      });
    }
  }

  onCancel() {
    this.ref.close(false);
  }
}
