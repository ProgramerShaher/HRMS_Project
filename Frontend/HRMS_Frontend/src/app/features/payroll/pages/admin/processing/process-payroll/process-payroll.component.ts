import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { DatePickerModule } from 'primeng/datepicker';
import { TableModule } from 'primeng/table';
import { ProgressBarModule } from 'primeng/progressbar';
import { CardModule } from 'primeng/card';
import { MessageService } from 'primeng/api';
import { ToastModule } from 'primeng/toast';
import { TagModule } from 'primeng/tag';
import { ActionButtonsComponent } from '../../../../../../shared/components/action-buttons/action-buttons.component';
import { PayrollProcessingService } from '../../../../services/payroll-processing.service';

interface ProcessingSummary {
  totalEmployees: number;
  processedEmployees: number;
  totalGrossSalary: number;
  totalDeductions: number;
  totalNetSalary: number;
  status: 'PENDING' | 'PROCESSING' | 'COMPLETED' | 'FAILED';
  errors: string[];
}

/**
 * معالجة الرواتب - واجهة إدارية
 * Process Payroll - Admin Interface
 */
@Component({
  selector: 'app-process-payroll',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ButtonModule,
    DatePickerModule,
    TableModule,
    ProgressBarModule,
    CardModule,
    ToastModule,
    TagModule,
    ActionButtonsComponent
  ],
  providers: [MessageService],
  templateUrl: 'process-payroll.component.html',
  styleUrl: 'process-payroll.component.scss'
})
export class ProcessPayrollComponent implements OnInit {
  selectedDate = signal<Date>(new Date());
  loading = signal(false);
  processing = signal(false);
  progress = signal(0);
  
  summary = signal<ProcessingSummary>({
    totalEmployees: 0,
    processedEmployees: 0,
    totalGrossSalary: 0,
    totalDeductions: 0,
    totalNetSalary: 0,
    status: 'PENDING',
    errors: []
  });

  employees = signal<any[]>([]);
  processedEmployees = signal<any[]>([]);

  constructor(
    private payrollService: PayrollProcessingService,
    private messageService: MessageService,
    private router: Router
  ) {}

  ngOnInit() {
    this.loadEmployeesSummary();
  }

  loadEmployeesSummary() {
    this.loading.set(true);
    
    // TODO: Load employees for selected month
    setTimeout(() => {
      // Mock data for now
      this.employees.set([]);
      this.loading.set(false);
    }, 1000);
  }

  onDateChange(date: Date) {
    this.selectedDate.set(date);
    this.loadEmployeesSummary();
  }

  async processPayroll() {
    if (!this.selectedDate()) {
      this.messageService.add({
        severity: 'warn',
        summary: 'تحذير',
        detail: 'يرجى اختيار الشهر'
      });
      return;
    }

    this.processing.set(true);
    this.progress.set(0);
    
    const currentSummary = this.summary();
    currentSummary.status = 'PROCESSING';
    this.summary.set({ ...currentSummary });

    // Simulate processing
    const totalSteps = 100;
    for (let i = 0; i <= totalSteps; i++) {
      await new Promise(resolve => setTimeout(resolve, 50));
      this.progress.set(i);
    }

    // TODO: Call actual API
    setTimeout(() => {
      const finalSummary = this.summary();
      finalSummary.status = 'COMPLETED';
      this.summary.set({ ...finalSummary });
      
      this.processing.set(false);
      this.messageService.add({
        severity: 'success',
        summary: 'نجح',
        detail: 'تمت معالجة الرواتب بنجاح'
      });
    }, 1000);
  }

  viewResults() {
    this.router.navigate(['/payroll/processing/runs']);
  }

  formatCurrency(value: number): string {
    return new Intl.NumberFormat('ar-YE', {
      style: 'decimal',
      minimumFractionDigits: 0,
      maximumFractionDigits: 0
    }).format(value);
  }

  formatDate(date: Date): string {
    return new Date(date).toLocaleDateString('ar-YE', {
      year: 'numeric',
      month: 'long'
    });
  }

  getStatusSeverity(status: string): 'success' | 'info' | 'warn' | 'danger' {
    switch (status) {
      case 'COMPLETED': return 'success';
      case 'PROCESSING': return 'info';
      case 'PENDING': return 'warn';
      case 'FAILED': return 'danger';
      default: return 'info';
    }
  }

  getStatusLabel(status: string): string {
    switch (status) {
      case 'COMPLETED': return 'مكتمل';
      case 'PROCESSING': return 'جاري المعالجة';
      case 'PENDING': return 'معلق';
      case 'FAILED': return 'فشل';
      default: return status;
    }
  }
}
