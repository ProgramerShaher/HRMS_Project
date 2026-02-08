import { Component, Input, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { InputTextModule } from 'primeng/inputtext';
import { SelectModule } from 'primeng/select';
import { FileUploadModule } from 'primeng/fileupload';
import { ToastModule } from 'primeng/toast';
import { InputNumberModule } from 'primeng/inputnumber';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { EmployeeService } from '../../services/employee.service';
import { MessageService, ConfirmationService } from 'primeng/api';

@Component({
  selector: 'app-profile-qualifications',
  standalone: true,
  imports: [
    CommonModule,
    TableModule,
    ButtonModule,
    DialogModule,
    ReactiveFormsModule,
    InputTextModule,
    SelectModule,
    FileUploadModule,
    ToastModule,
    InputNumberModule,
    ConfirmDialogModule
  ],
  templateUrl: './profile-qualifications.component.html',
  providers: [MessageService, ConfirmationService]
})
export class ProfileQualificationsComponent implements OnInit {
  @Input() employeeId!: number;
  
  // Data Signals
  qualifications = signal<any[]>([]);
  loading = signal<boolean>(false);
  
  // Dialog State
  displayDialog = false;
  submitted = false;
  qualificationForm: FormGroup;
  selectedFile: File | null = null;

  // Services
  private employeeService = inject(EmployeeService);
  private messageService = inject(MessageService);
  private confirmationService = inject(ConfirmationService);
  private fb = inject(FormBuilder);

  // Constants
  currentYear = new Date().getFullYear();
  years: number[] = [];

  constructor() {
    this.qualificationForm = this.fb.group({
      degreeType: ['', Validators.required],
      majorAr: ['', [
          Validators.required,
          Validators.pattern(/^[\u0600-\u06FF\s]+$/) // Arabic only
      ]],
      universityAr: ['', Validators.required],
      countryId: [null], 
      graduationYear: [this.currentYear, [Validators.required, Validators.min(1950), Validators.max(this.currentYear + 5)]],
      grade: ['']
    });

    // Generate years for dropdown
    for (let i = this.currentYear + 1; i >= 1970; i--) {
      this.years.push(i);
    }
  }

  ngOnInit() {
    if (this.employeeId) {
      this.loadData();
    }
  }

  loadData() {
    this.loading.set(true);
    this.employeeService.getQualifications(this.employeeId).subscribe({
      next: (res) => {
        this.qualifications.set(res.data || []);
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }

  showDialog() {
    this.qualificationForm.reset({ graduationYear: this.currentYear });
    this.selectedFile = null;
    this.submitted = false;
    this.displayDialog = true;
  }

  onFileSelect(event: any) {
    if (event.files && event.files.length > 0) {
      this.selectedFile = event.files[0];
    }
  }

  save() {
    this.submitted = true;
    if (this.qualificationForm.invalid) return;

    this.loading.set(true);
    const data = { ...this.qualificationForm.value, employeeId: this.employeeId };

    // Assuming ADD only for now as per endpoints, if EDIT is needed logic changes
    this.employeeService.addQualification(this.employeeId, data, this.selectedFile || undefined).subscribe({
      next: () => {
        this.messageService.add({ severity: 'success', summary: 'نجاح', detail: 'تم إضافة المؤهل بنجاح' });
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
      message: 'هل أنت متأكد من حذف هذا المؤهل؟',
      header: 'تأكيد الحذف',
      icon: 'pi pi-exclamation-triangle',
      acceptLabel: 'نعم',
      rejectLabel: 'لا',
      acceptButtonStyleClass: 'p-button-danger',
      accept: () => {
        this.employeeService.deleteQualification(this.employeeId, id).subscribe({
          next: () => {
            this.messageService.add({ severity: 'success', summary: 'تم الحذف', detail: 'تم حذف المؤهل بنجاح' });
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
