import { Component, Input, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { InputTextModule } from 'primeng/inputtext';
import { ToastModule } from 'primeng/toast';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { CheckboxModule } from 'primeng/checkbox';
import { EmployeeService } from '../../services/employee.service';
import { MessageService, ConfirmationService } from 'primeng/api';

@Component({
  selector: 'app-profile-bank-accounts',
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
    CheckboxModule
  ],
  templateUrl: './profile-bank-accounts.component.html',
  providers: [MessageService, ConfirmationService]
})
export class ProfileBankAccountsComponent implements OnInit {
  @Input() employeeId!: number;
  
  accounts = signal<any[]>([]);
  loading = signal<boolean>(false);
  
  displayDialog = false;
  submitted = false;
  isEditMode = false;
  bankForm: FormGroup;
  selectedAccountId: number | null = null;

  private employeeService = inject(EmployeeService);
  private messageService = inject(MessageService);
  private confirmationService = inject(ConfirmationService);
  private fb = inject(FormBuilder);

  constructor() {
    this.bankForm = this.fb.group({
      bankName: ['', Validators.required],
      accountHolderName: ['', Validators.required],
      accountNumber: ['', Validators.required],
      ibanNumber: ['', [Validators.required, Validators.pattern('^[A-Z]{2}[0-9]{2}[a-zA-Z0-9]{1,30}$')]],
      isPrimary: [false]
    });
  }

  ngOnInit() {
    if (this.employeeId) {
      this.loadData();
    }
  }

  loadData() {
    this.loading.set(true);
    this.employeeService.getBankAccounts(this.employeeId).subscribe({
      next: (res) => {
        this.accounts.set(res.data || []);
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }

  showAddDialog() {
    this.isEditMode = false;
    this.selectedAccountId = null;
    this.bankForm.reset({ isPrimary: false });
    this.submitted = false;
    this.displayDialog = true;
  }

  showEditDialog(acc: any) {
    this.isEditMode = true;
    this.selectedAccountId = acc.bankAccountId;
    this.bankForm.patchValue({
        bankName: acc.bankName,
        accountHolderName: acc.accountHolderName,
        accountNumber: acc.accountNumber,
        ibanNumber: acc.ibanNumber,
        isPrimary: acc.isPrimary
    });
    this.displayDialog = true;
  }

  save() {
    this.submitted = true;
    if (this.bankForm.invalid) return;

    this.loading.set(true);
    const formVal = this.bankForm.value;
    const data = {
        EmployeeId: this.employeeId,
        BankAccountId: this.selectedAccountId, // For update
        BankName: formVal.bankName,
        AccountHolderName: formVal.accountHolderName,
        AccountNumber: formVal.accountNumber,
        IbanNumber: formVal.ibanNumber,
        IsPrimary: formVal.isPrimary
    };

    if (this.isEditMode && this.selectedAccountId) {
        this.employeeService.updateBankAccount(this.employeeId, this.selectedAccountId, data).subscribe({
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
        this.employeeService.addBankAccount(this.employeeId, data).subscribe({
            next: () => {
              this.messageService.add({ severity: 'success', summary: 'نجاح', detail: 'تم إضافة الحساب البنكي بنجاح' });
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
      message: 'هل أنت متأكد من حذف هذا الحساب البنكي؟',
      header: 'تأكيد الحذف',
      icon: 'pi pi-exclamation-triangle',
      acceptLabel: 'نعم',
      rejectLabel: 'لا',
      acceptButtonStyleClass: 'p-button-danger',
      accept: () => {
        this.employeeService.deleteBankAccount(this.employeeId, id).subscribe({
          next: () => {
             this.messageService.add({ severity: 'success', summary: 'تم الحذف', detail: 'تم الحذف بنجاح' });
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
