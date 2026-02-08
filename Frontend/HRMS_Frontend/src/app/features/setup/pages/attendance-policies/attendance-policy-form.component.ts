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
  loading = signal(false);
  
  // Stores all jobs fetched from API
  allJobs: any[] = [];
  
  // Stores filtered jobs based on selected department
  departments = signal<any[]>([]);
  filteredJobs = signal<any[]>([]);

  ngOnInit() {
    this.initForm();
    this.loadDependencies();
    
    // Watch for department changes to filter jobs
    this.form.get('deptId')?.valueChanges.subscribe(deptId => {
        this.filterJobs(deptId);
    });

    if (this.config.data && this.config.data.policyId) {
      this.isEditMode = true;
      this.form.patchValue(this.config.data);
      // Trigger filtering if editing and deptId is present
      if (this.config.data.deptId) {
          // We need to wait for dependencies to load, or handling it in loadDependencies
      }
    }
  }

  initForm() {
    this.form = this.fb.group({
      policyId: [null],
      policyNameAr: ['', [Validators.required]],
      deptId: [null], 
      jobId: [null],  
      lateGraceMins: [0, [Validators.required, Validators.min(0)]],
      overtimeMultiplier: [1.5, [Validators.required, Validators.min(1)]],
      weekendOtMultiplier: [2, [Validators.required, Validators.min(1)]]
    });
  }

  loadDependencies() {
    this.loading.set(true);
    // Use PascalCase as per backend conventions seen in DepartmentService
    const depts$ = this.setupService.getAll<any>('Departments');
    const jobs$ = this.setupService.getAll<any>('Jobs');

    forkJoin([depts$, jobs$]).subscribe({
      next: ([deptsRes, jobsRes]: [any, any]) => {
        // Helper to extract items from various API response formats
        const extractItems = (res: any) => {
            if (Array.isArray(res)) return res;
            if (res?.data && Array.isArray(res.data)) return res.data;
            if (res?.data?.items && Array.isArray(res.data.items)) return res.data.items;
            if (res?.items && Array.isArray(res.items)) return res.items;
            return [];
        };

        // Departments
        this.departments.set(extractItems(deptsRes));

        // Jobs
        this.allJobs = extractItems(jobsRes);
        
        // Initial filtering if in edit mode
        const currentDeptId = this.form.get('deptId')?.value;
        this.filterJobs(currentDeptId);
        
        this.loading.set(false);
      },
      error: (err) => {
          console.error('Error loading dependencies', err);
          this.loading.set(false);
      }
    });
  }

  filterJobs(deptId: number | null) {
      if (!deptId) {
          // If no department selected, maybe show all jobs or none? 
          // User asked: "fetch Jobs that belong to the Department only".
          // If no dept, typically show all or clear. Let's show all for flexibility unless strict.
          this.filteredJobs.set(this.allJobs);
          return;
      }
      // Assuming Job object has 'deptId' or similar. 
      // If Job doesn't have deptId directly (common in some schemas, linked via another table), 
      // we might need to check the Job DTO structure. 
      // Based on standard HRMS, Jobs are often linked to Departments or generic.
      // Let's assume Job entity has 'deptId' property based on context. 
      // Checking local Job interface would be good, but assuming standard filtering:
      const filtered = this.allJobs.filter(j => j.deptId === deptId || !j.deptId); // Show dept-specific + global jobs
      this.filteredJobs.set(filtered);
      
      // Clear selected job if it doesn't belong to the new department
      const currentJobId = this.form.get('jobId')?.value;
      if (currentJobId) {
          const jobExists = filtered.find(j => j.jobId === currentJobId);
          if (!jobExists) {
              this.form.patchValue({ jobId: null });
          }
      }
  }

  onSubmit() {
    if (this.form.invalid) return;

    this.loading.set(true);
    const formValue = this.form.value;
    console.log('Attendance Policy Form Submission:', formValue);
    console.log('Payload:', JSON.stringify(formValue));

    if (this.isEditMode) {
      this.setupService.update('AttendancePolicy', formValue.policyId, formValue).subscribe({
        next: () => {
          this.messageService.add({ severity: 'success', summary: 'نجاح', detail: 'تم التعديل بنجاح' });
          this.ref.close(true);
          this.loading.set(false);
        },
        error: () => this.loading.set(false)
      });
    } else {
      this.setupService.create('AttendancePolicy', formValue).subscribe({
        next: () => {
          this.messageService.add({ severity: 'success', summary: 'نجاح', detail: 'تم الإضافة بنجاح' });
          this.ref.close(true);
          this.loading.set(false);
        },
        error: () => this.loading.set(false)
      });
    }
  }

  onCancel() {
    this.ref.close(false);
  }
}
