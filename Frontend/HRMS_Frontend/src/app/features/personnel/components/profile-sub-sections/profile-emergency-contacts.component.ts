import { Component, Input, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { InputTextModule } from 'primeng/inputtext';
import { ToastModule } from 'primeng/toast';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { EmployeeService } from '../../services/employee.service';
import { MessageService, ConfirmationService } from 'primeng/api';
import { Tag } from "primeng/tag";

@Component({
  selector: 'app-profile-emergency-contacts',
  standalone: true,
  imports: [
    CommonModule,
    TableModule,
    ButtonModule,
    DialogModule,
    ReactiveFormsModule,
    InputTextModule,
    ToastModule,
    ConfirmDialogModule,
    Tag
],
  templateUrl: './profile-emergency-contacts.component.html',
  providers: [MessageService, ConfirmationService]
})
export class ProfileEmergencyContactsComponent implements OnInit {
  @Input() employeeId!: number;
  
  contacts = signal<any[]>([]);
  loading = signal<boolean>(false);
  
  displayDialog = false;
  submitted = false;
  isEditMode = false;
  contactForm: FormGroup;
  selectedContactId: number | null = null;

  private employeeService = inject(EmployeeService);
  private messageService = inject(MessageService);
  private confirmationService = inject(ConfirmationService);
  private fb = inject(FormBuilder);

  constructor() {
    this.contactForm = this.fb.group({
      fullNameAr: ['', [
          Validators.required, 
          Validators.pattern(/^[\u0600-\u06FF\s]+$/)
      ]],
      relationship: ['', Validators.required],
      mobile: ['', [
          Validators.required, 
          Validators.pattern(/^\d+$/),
          Validators.maxLength(20)
      ]]
    });
  }

  ngOnInit() {
    if (this.employeeId) {
      this.loadData();
    }
  }

  loadData() {
    this.loading.set(true);
    this.employeeService.getEmergencyContacts(this.employeeId).subscribe({
      next: (res) => {
        this.contacts.set(res.data || []);
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }

  showAddDialog() {
    this.isEditMode = false;
    this.selectedContactId = null;
    this.contactForm.reset();
    this.submitted = false;
    this.displayDialog = true;
  }

  showEditDialog(contact: any) {
    this.isEditMode = true;
    this.selectedContactId = contact.emergencyContactId;
    this.contactForm.patchValue({
        contactNameAr: contact.contactNameAr,
        relationship: contact.relationship,
        phonePrimary: contact.phonePrimary,
        phoneSecondary: contact.phoneSecondary,
        address: contact.address
    });
    this.displayDialog = true;
  }

  save() {
    this.submitted = true;
    if (this.contactForm.invalid) return;

    this.loading.set(true);
    const formVal = this.contactForm.value;
    const data = {
        EmployeeId: this.employeeId,
        EmergencyContactId: this.selectedContactId,
        ContactNameAr: formVal.fullNameAr,
        Relationship: formVal.relationship,
        PhonePrimary: formVal.mobile,
        PhoneSecondary: ''
    };

    if (this.isEditMode && this.selectedContactId) {
        this.employeeService.updateEmergencyContact(this.employeeId, this.selectedContactId, data).subscribe({
            next: () => {
              this.messageService.add({ severity: 'success', summary: 'نجاح', detail: 'تم تحديث البيانات بنجاح' });
              this.displayDialog = false;
              this.loadData();
            },
            error: () => {
              this.messageService.add({ severity: 'error', summary: 'خطأ', detail: 'حدث خطأ أثناء التحديث' });
              this.loading.set(false);
            }
        });
    } else {
        this.employeeService.addEmergencyContact(this.employeeId, data).subscribe({
            next: () => {
              this.messageService.add({ severity: 'success', summary: 'نجاح', detail: 'تم إضافة جهة الاتصال بنجاح' });
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
    }
  }

  delete(id: number) {
    this.confirmationService.confirm({
      message: 'هل أنت متأكد من حذف جهة الاتصال هذه؟',
      header: 'تأكيد الحذف',
      icon: 'pi pi-exclamation-triangle',
      acceptLabel: 'نعم',
      rejectLabel: 'لا',
      acceptButtonStyleClass: 'p-button-danger',
      accept: () => {
        this.employeeService.deleteEmergencyContact(this.employeeId, id).subscribe({
          next: () => {
            this.messageService.add({ severity: 'success', summary: 'تم الحذف', detail: 'تم حذف جهة الاتصال بنجاح' });
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
