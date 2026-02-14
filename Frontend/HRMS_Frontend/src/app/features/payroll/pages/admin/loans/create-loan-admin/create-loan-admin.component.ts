import { Component, OnInit, signal, inject, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { InputNumberModule } from 'primeng/inputnumber';
import { DatePickerModule } from 'primeng/datepicker';
import { MessageModule } from 'primeng/message';
import { MessageService } from 'primeng/api';
import { ToastModule } from 'primeng/toast';
import { LoanService } from '../../../../services/loan.service';
import { EmployeeSelectorComponent } from '../../../../../../shared/components/employee-selector/employee-selector.component';
import { Employee } from '../../../../../../shared/models/employee.models';

@Component({
  selector: 'app-create-loan-admin',
  standalone: true,
  imports: [
    CommonModule, ReactiveFormsModule, ButtonModule, InputNumberModule,
    DatePickerModule, MessageModule, ToastModule, EmployeeSelectorComponent
  ],
  providers: [MessageService],
  templateUrl: 'create-loan-admin.component.html',
  styleUrls: ['create-loan-admin.component.scss']
})
export class CreateLoanAdminComponent implements OnInit {
  private fb = inject(FormBuilder);
  private loanService = inject(LoanService);
  private router = inject(Router);
  private messageService = inject(MessageService);

  loanForm!: FormGroup;
  loading = signal(false);
  selectedEmployee = signal<Employee | null>(null);

  // Computed values
  monthlyInstallment = computed(() => {
    const amount = this.loanForm?.get('loanAmount')?.value || 0;
    const count = this.loanForm?.get('installmentCount')?.value || 1;
    return count > 0 ? amount / count : 0;
  });

  maxAllowedLoan = computed(() => {
    // TODO: Get from employee salary structure
    // For now, assume a basic salary
    const basicSalary = 10000; // This should come from API
    return basicSalary * 0.3;
  });

  exceedsLimit = computed(() => {
    const amount = this.loanForm?.get('loanAmount')?.value || 0;
    return amount > this.maxAllowedLoan();
  });

  ngOnInit() {
    this.initForm();
  }

  initForm() {
    this.loanForm = this.fb.group({
      employeeId: [null, Validators.required],
      loanAmount: [null, [Validators.required, Validators.min(100)]],
      installmentCount: [12, [Validators.required, Validators.min(1), Validators.max(60)]],
      startDate: [new Date(), Validators.required]
    });
  }

  onEmployeeSelected(employee: Employee) {
    this.selectedEmployee.set(employee);
    if (employee) {
      this.loanForm.patchValue({ employeeId: employee.employeeId });
      // TODO: Load employee salary structure to calculate max loan
    }
  }

  onSubmit() {
    if (this.loanForm.invalid) {
      this.markFormGroupTouched(this.loanForm);
      this.messageService.add({
        severity: 'error',
        summary: 'خطأ',
        detail: 'يرجى تعبئة جميع الحقول المطلوبة'
      });
      return;
    }

    if (this.exceedsLimit()) {
      this.messageService.add({
        severity: 'warn',
        summary: 'تحذير',
        detail: 'المبلغ يتجاوز الحد المسموح (30% من الراتب الأساسي)'
      });
      return;
    }

    this.loading.set(true);
    const formValue = this.loanForm.value;

    this.loanService.createLoan({
      employeeId: formValue.employeeId,
      loanAmount: formValue.loanAmount,
      installmentCount: formValue.installmentCount,
      startDate: formValue.startDate
    }).subscribe({
      next: (response) => {
        if (response.succeeded) {
          this.messageService.add({
            severity: 'success',
            summary: 'نجح',
            detail: 'تم إنشاء السلفة بنجاح'
          });
          setTimeout(() => {
            this.router.navigate(['/payroll/loans/admin/all']);
          }, 1500);
        } else {
          this.messageService.add({
            severity: 'error',
            summary: 'خطأ',
            detail: response.message || 'فشل إنشاء السلفة'
          });
        }
        this.loading.set(false);
      },
      error: (err) => {
        this.messageService.add({
          severity: 'error',
          summary: 'خطأ',
          detail: 'حدث خطأ أثناء إنشاء السلفة'
        });
        this.loading.set(false);
      }
    });
  }

  cancel() {
    this.router.navigate(['/payroll/loans/admin/all']);
  }

  private markFormGroupTouched(formGroup: FormGroup) {
    Object.keys(formGroup.controls).forEach(key => {
      const control = formGroup.get(key);
      control?.markAsTouched();
    });
  }

  formatCurrency(value: number): string {
    return new Intl.NumberFormat('ar-YE', { style: 'decimal' }).format(value);
  }
}
