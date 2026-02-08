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
    CheckboxModule
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
  expForm: FormGroup;

  private employeeService = inject(EmployeeService);
  private messageService = inject(MessageService);
  private confirmationService = inject(ConfirmationService);
  private fb = inject(FormBuilder);

  constructor() {
    this.expForm = this.fb.group({
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
    this.expForm.reset({ isCurrent: false });
    this.submitted = false;
    this.displayDialog = true;
  }

  onCurrentChange() {
    if (this.expForm.get('isCurrent')?.value) {
      this.expForm.get('endDate')?.setValue(null);
      this.expForm.get('endDate')?.disable();
    } else {
      this.expForm.get('endDate')?.enable();
    }
  }

  save() {
    this.submitted = true;
    if (this.expForm.invalid) return;

    this.loading.set(true);
    const formVal = this.expForm.value;
    const data = {
        EmployeeId: this.employeeId,
        CompanyNameAr: formVal.companyNameAr,
        JobTitleAr: formVal.jobTitleAr,
        StartDate: formVal.startDate ? new Date(formVal.startDate).toISOString() : null,
        EndDate: formVal.isCurrent ? null : (formVal.endDate ? new Date(formVal.endDate).toISOString() : null),
        IsCurrent: formVal.isCurrent ? 1 : 0,
        Responsibilities: formVal.responsibilities,
        ReasonForLeaving: formVal.reasonForLeaving
    };

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
