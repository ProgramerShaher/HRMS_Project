import { Component, Input, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { InputTextModule } from 'primeng/inputtext';
import { FileUploadModule } from 'primeng/fileupload';
import { ToastModule } from 'primeng/toast';
import { DatePickerModule } from 'primeng/datepicker'; // Use DatePickerModule from PrimeNG v18+
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { CheckboxModule } from 'primeng/checkbox';
import { EmployeeService } from '../../services/employee.service';
import { MessageService, ConfirmationService } from 'primeng/api';

@Component({
  selector: 'app-profile-certifications',
  standalone: true,
  imports: [
    CommonModule,
    TableModule,
    ButtonModule,
    DialogModule,
    ReactiveFormsModule,
    InputTextModule,
    FileUploadModule,
    ToastModule,
    DatePickerModule,
    ConfirmDialogModule,
    CheckboxModule
  ],
  templateUrl: './profile-certifications.component.html',
  providers: [MessageService, ConfirmationService]
})
export class ProfileCertificationsComponent implements OnInit {
  @Input() employeeId!: number;
  
  certifications = signal<any[]>([]);
  loading = signal<boolean>(false);
  
  displayDialog = false;
  submitted = false;
  certForm: FormGroup;
  selectedFile: File | null = null;

  private employeeService = inject(EmployeeService);
  private messageService = inject(MessageService);
  private confirmationService = inject(ConfirmationService);
  private fb = inject(FormBuilder);

  constructor() {
    this.certForm = this.fb.group({
      certificationName: ['', Validators.required],
      issuingOrganization: ['', Validators.required], // Updated to match API field? API says 'IssuingAuthority' or 'issuingOrganization'? Check Model.
                                                    // Checked API log earlier: "IssuingAuthority" in POST/PUT. "issuingOrganization" in GET display?
                                                    // Let's check employee.service.ts or previous logs. 
                                                    // Log says: POST: CertNameAr, IssuingAuthority, IssueDate... 
                                                    // GET response: certificationName, issuingOrganization...
                                                    // I will map accordingly.
      issueDate: [null, Validators.required],
      expiryDate: [null],
      certNumber: [''],
      isMandatory: [false]
    });
  }

  ngOnInit() {
    if (this.employeeId) {
      this.loadData();
    }
  }

  loadData() {
    this.loading.set(true);
    this.employeeService.getCertifications(this.employeeId).subscribe({
      next: (res) => {
        this.certifications.set(res.data || []);
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }

  showDialog() {
    this.certForm.reset({ isMandatory: false });
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
    if (this.certForm.invalid) return;

    this.loading.set(true);
    
    // Map form values to API DTO keys
    const formVal = this.certForm.value;
    const data = {
        EmployeeId: this.employeeId,
        CertNameAr: formVal.certificationName,
        IssuingAuthority: formVal.issuingOrganization,
        IssueDate: formVal.issueDate ? new Date(formVal.issueDate).toISOString() : null,
        ExpiryDate: formVal.expiryDate ? new Date(formVal.expiryDate).toISOString() : null,
        CertNumber: formVal.certNumber,
        IsMandatory: formVal.isMandatory ? 1 : 0
    };

    this.employeeService.addCertification(this.employeeId, data, this.selectedFile || undefined).subscribe({
      next: () => {
        this.messageService.add({ severity: 'success', summary: 'نجاح', detail: 'تم إضافة الشهادة بنجاح' });
        this.displayDialog = false;
        this.loadData();
      },
      error: () => {
        this.messageService.add({ severity: 'error', summary: 'خطأ', detail: 'حدث خطأ أثناء الحفظ' });
        this.loading.set(false);
      }
    });
  }

  isExpired(dateStr: string): boolean {
    if (!dateStr) return false;
    const expiryDate = new Date(dateStr);
    const today = new Date();
    // Reset time part for accurate date comparison
    today.setHours(0, 0, 0, 0);
    expiryDate.setHours(0, 0, 0, 0);
    return expiryDate < today;
  }

  delete(id: number) {
    this.confirmationService.confirm({
      message: 'هل أنت متأكد من حذف هذه الشهادة؟',
      header: 'تأكيد الحذف',
      icon: 'pi pi-exclamation-triangle',
      acceptLabel: 'نعم',
      rejectLabel: 'لا',
      acceptButtonStyleClass: 'p-button-danger',
      accept: () => {
        this.employeeService.deleteCertification(this.employeeId, id).subscribe({
          next: () => {
            this.messageService.add({ severity: 'success', summary: 'تم الحذف', detail: 'تم حذف الشهادة بنجاح' });
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
