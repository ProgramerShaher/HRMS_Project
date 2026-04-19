import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { TableModule } from 'primeng/table';
import { SelectModule } from 'primeng/select';
import { FormsModule } from '@angular/forms';
import { PayrollProcessingService } from '../../../services/payroll-processing.service';
import { Payslip } from '../../../models/payroll-processing.models';
import { AuthService } from '../../../../../core/auth/services/auth.service';

@Component({
  selector: 'app-my-payslips',
  standalone: true,
  imports: [CommonModule, ButtonModule, TableModule, SelectModule, FormsModule],
  templateUrl: './my-payslips.component.html',
  styleUrls: ['./my-payslips.component.scss']
})
export class MyPayslipsComponent implements OnInit {
  private payrollService = inject(PayrollProcessingService);
  private authService = inject(AuthService);
  private router = inject(Router);

  payslips = signal<Payslip[]>([]);
  loading = signal(false);

  ngOnInit() {
    this.loadPayslips();
  }

  loadPayslips() {
    const employeeId = this.authService.currentUser()?.employeeId;
    if (!employeeId) return;

    this.loading.set(true);
    this.payrollService.getEmployeePayslips(employeeId).subscribe({
      next: (res) => {
        if (res.succeeded && res.data) {
          this.payslips.set(res.data);
        }
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }

  formatCurrency(value: number): string {
    return new Intl.NumberFormat('ar-YE', { style: 'decimal' }).format(value);
  }

  getMonthName(month: number | undefined): string {
    if (!month) return '-';
    const months = ['يناير', 'فبراير', 'مارس', 'أبريل', 'مايو', 'يونيو', 'يوليو', 'أغسطس', 'سبتمبر', 'أكتوبر', 'نوفمبر', 'ديسمبر'];
    return months[month - 1] || '-';
  }
}
