import { Component, Input, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { InputTextModule } from 'primeng/inputtext';
import { TextareaModule } from 'primeng/textarea';
import { ToastModule } from 'primeng/toast';
import { DatePickerModule } from 'primeng/datepicker';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { CheckboxModule } from 'primeng/checkbox';
import { EmployeeService } from '../../services/employee.service';
import { MessageService, ConfirmationService } from 'primeng/api';
import { Tag } from "primeng/tag";

@Component({
  selector: 'app-profile-experience',
  standalone: true,
  imports: [
    CommonModule,
    TableModule,
    ButtonModule,
    DialogModule,
    ReactiveFormsModule,
    InputTextModule,
    TextareaModule,
    ToastModule,
    DatePickerModule,
    ConfirmDialogModule,
    CheckboxModule,
    Tag
],
  templateUrl: './profile-experience.component.html',
  providers: [MessageService, ConfirmationService]
})
export class ProfileExperienceComponent implements OnInit {
  @Input() employeeId!: number;
  
  experiences = signal<any[]>([]);
  loading = signal<boolean>(false);
  
  displayDialog = false;
  submitted = false;
  isEditMode = false;
  selectedExperienceId: number | null = null;
  experienceForm: FormGroup;

  private employeeService = inject(EmployeeService);
  private messageService = inject(MessageService);
  private confirmationService = inject(ConfirmationService);
  private fb = inject(FormBuilder);

  constructor() {
    this.experienceForm = this.fb.group({
      companyNameAr: ['', Validators.required],
      jobTitleAr: ['', Validators.required],
      startDate: [null, Validators.required],
      endDate: [null],
      isCurrent: [false],
      responsibilities: [''],
      reasonForLeaving: ['']
    });
  }

  ngOnInit() {
    if (this.employeeId) {
      this.loadData();
    }
  }

  loadData() {
    this.loading.set(true);
    this.employeeService.getExperiences(this.employeeId).subscribe({
      next: (res) => {
        this.experiences.set(res.data || []);
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }

  showDialog() {
    this.isEditMode = false;
    this.selectedExperienceId = null;
    this.experienceForm.reset({ isCurrent: false });
    this.submitted = false;
    this.displayDialog = true;
  }

  showEditDialog(exp: any) {
    this.isEditMode = true;
    this.selectedExperienceId = exp.experienceId;
    this.experienceForm.patchValue({
      companyNameAr: exp.companyNameAr,
      jobTitleAr: exp.jobTitleAr,
      startDate: exp.startDate ? new Date(exp.startDate) : null,
      endDate: exp.endDate ? new Date(exp.endDate) : null,
      isCurrent: exp.isCurrent,
      responsibilities: exp.responsibilities,
      reasonForLeaving: exp.reasonForLeaving
    });
    this.onCurrentChange();
    this.submitted = false;
    this.displayDialog = true;
  }

  onCurrentChange() {
    if (this.experienceForm.get('isCurrent')?.value) {
      this.experienceForm.get('endDate')?.setValue(null);
      this.experienceForm.get('endDate')?.disable();
    } else {
      this.experienceForm.get('endDate')?.enable();
    }
  }

  save() {
    this.submitted = true;
    if (this.experienceForm.invalid) return;

    this.loading.set(true);
    const formVal = this.experienceForm.value;
    const data = { 
        ...formVal, 
        employeeId: this.employeeId,
        experienceId: this.selectedExperienceId,
        IsCurrent: formVal.isCurrent ? 1 : 0, // Convert boolean to byte
        StartDate: formVal.startDate ? new Date(formVal.startDate).toISOString() : null,
        EndDate: formVal.isCurrent ? null : (formVal.endDate ? new Date(formVal.endDate).toISOString() : null),
    };

    if (this.isEditMode && this.selectedExperienceId) {
        this.employeeService.updateExperience(this.employeeId, this.selectedExperienceId, data).subscribe({
            next: () => {
              this.messageService.add({ severity: 'success', summary: 'نجاح', detail: 'تم تحديث الخبرة بنجاح' });
              this.displayDialog = false;
              this.loadData();
            },
            error: (err) => {
              console.error('Save Error:', err);
              const errorMessage = err.error?.message || err.error?.detail || 'حدث خطأ أثناء الحفظ';
              this.messageService.add({ severity: 'error', summary: 'خطأ', detail: errorMessage });
              this.loading.set(false);
            }
        });
    } else {
        this.employeeService.addExperience(this.employeeId, data).subscribe({
            next: () => {
              this.messageService.add({ severity: 'success', summary: 'نجاح', detail: 'تم إضافة الخبرة بنجاح' });
              this.displayDialog = false;
              this.loadData();
            },
            error: () => {
              this.messageService.add({ severity: 'error', summary: 'خطأ', detail: 'حدث خطأ أثناء الحفظ' });
              this.loading.set(false);
            }
        });
    }
  }

  delete(id: number) {
    this.confirmationService.confirm({
      message: 'هل أنت متأكد من حذف هذا السجل؟',
      header: 'تأكيد الحذف',
      icon: 'pi pi-exclamation-triangle',
      acceptLabel: 'نعم',
      rejectLabel: 'لا',
      acceptButtonStyleClass: 'p-button-danger',
      accept: () => {
        this.employeeService.deleteExperience(this.employeeId, id).subscribe({
          next: () => {
            this.messageService.add({ severity: 'success', summary: 'تم الحذف', detail: 'تم حذف الخبرة بنجاح' });
            this.loadData();
          },
          error: () => {
             this.messageService.add({ severity: 'error', summary: 'خطأ', detail: 'لا يمكن حذف العنصر' });
          }
        });
      }
    });
  }
}
