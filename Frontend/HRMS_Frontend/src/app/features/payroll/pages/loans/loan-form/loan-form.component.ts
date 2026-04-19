import { Component, OnInit, signal, inject, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { InputNumberModule } from 'primeng/inputnumber';
import { DatePickerModule } from 'primeng/datepicker';
import { MessageModule } from 'primeng/message';
import { LoanService } from '../../../services/loan.service';
import { SalaryStructureService } from '../../../services/salary-structure.service';
import { CreateLoanRequest } from '../../../models/loan.models';
import { AuthService } from '../../../../../core/auth/services/auth.service';
import { MessageService } from 'primeng/api';
import { ToastModule } from 'primeng/toast';

@Component({
  selector: 'app-loan-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, ButtonModule, InputNumberModule, DatePickerModule, MessageModule, ToastModule],
  templateUrl: './loan-form.component.html',
  styleUrls: ['./loan-form.component.scss'],
  providers: [MessageService]
})
export class LoanFormComponent implements OnInit {
  private fb = inject(FormBuilder);
  private loanService = inject(LoanService);
  private structureService = inject(SalaryStructureService);
  private authService = inject(AuthService);
  private router = inject(Router);
  private messageService = inject(MessageService);

  loanForm!: FormGroup;
  loading = signal(false);
  basicSalary = signal(0);
  
  monthlyInstallment = computed(() => {
    const amount = this.loanForm?.get('loanAmount')?.value || 0;
    const count = this.loanForm?.get('installmentCount')?.value || 1;
    return amount / count;
  });

  installmentPercentage = computed(() => {
    const installment = this.monthlyInstallment();
    const basic = this.basicSalary();
    if (basic === 0) return 0;
    return (installment / basic) * 100;
  });

  isValid30Percent = computed(() => {
    return this.installmentPercentage() <= 30;
  });

  ngOnInit() {
    this.initForm();
    this.loadBasicSalary();
  }

  initForm() {
    const employeeId = this.authService.currentUser()?.employeeId;
    this.loanForm = this.fb.group({
      employeeId: [employeeId, Validators.required],
      loanAmount: [null, [Validators.required, Validators.min(1000)]],
      installmentCount: [null, [Validators.required, Validators.min(1), Validators.max(60)]],
      startDate: [new Date(), Validators.required]
    });
  }

  loadBasicSalary() {
    const employeeId = this.authService.currentUser()?.employeeId;
    if (!employeeId) return;

    this.structureService.getSalaryBreakdown(employeeId).subscribe({
      next: (breakdown) => {
        this.basicSalary.set(breakdown.basicSalary);
      },
      error: (err) => console.error('Error loading salary:', err)
    });
  }

  formatCurrency(value: number): string {
    return new Intl.NumberFormat('ar-YE', {
      style: 'decimal',
      minimumFractionDigits: 0,
      maximumFractionDigits: 0
    }).format(value);
  }

  onSubmit() {
    if (this.loanForm.invalid) {
      this.messageService.add({
        severity: 'error',
        summary: 'خطأ',
        detail: 'يرجى ملء جميع الحقول المطلوبة'
      });
      return;
    }

    if (!this.isValid30Percent()) {
      this.messageService.add({
        severity: 'error',
        summary: 'خطأ',
        detail: 'القسط الشهري يتجاوز 30% من الراتب الأساسي'
      });
      return;
    }

    this.loading.set(true);
    const request: CreateLoanRequest = this.loanForm.value;

    this.loanService.createLoan(request).subscribe({
      next: (response) => {
        if (response.succeeded) {
          this.messageService.add({
            severity: 'success',
            summary: 'نجح',
            detail: 'تم إنشاء السلفة بنجاح'
          });
          setTimeout(() => {
            this.router.navigate(['/payroll/loans/my-loans']);
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
    this.router.navigate(['/payroll/loans/my-loans']);
  }
}
