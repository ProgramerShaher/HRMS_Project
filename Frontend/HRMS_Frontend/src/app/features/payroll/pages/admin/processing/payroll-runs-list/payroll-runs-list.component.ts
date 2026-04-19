import { CommonModule } from '@angular/common';
import { PayrollRun, PayrollRunStatus } from '../../../../models/payroll-processing.models';
import { FormsModule } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { TableModule } from 'primeng/table';
import { InputTextModule } from 'primeng/inputtext';
import { SelectModule } from 'primeng/select';
import { TagModule } from 'primeng/tag';
import { ActionButtonsComponent } from '../../../../../../shared/components/action-buttons/action-buttons.component';
import { Component, OnInit, signal } from '@angular/core';
import { PayrollProcessingService } from '../../../../services/payroll-processing.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-payroll-runs-list',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    TableModule,
    ButtonModule,
    InputTextModule,
    SelectModule,
    TagModule,
    ActionButtonsComponent
  ],
  templateUrl: './payroll-runs-list.component.html',
  styleUrl: './payroll-runs-list.component.scss'
})
export class PayrollRunsListComponent implements OnInit {
  runs = signal<PayrollRun[]>([]);
  filteredRuns = signal<PayrollRun[]>([]);
  loading = signal(false);

  searchTerm = signal('');
  selectedYear = signal<number>(new Date().getFullYear());
  selectedStatus = signal<string | null>(null);

  yearOptions = signal<any[]>([]);
  statusOptions = [
    { label: 'الكل', value: null },
    { label: 'مكتمل', value: 'COMPLETED' },
    { label: 'جاري المعالجة', value: 'PROCESSING' },
    { label: 'معلق', value: 'PENDING' },
    { label: 'تم الترحيل', value: 'POSTED' }
  ];

  constructor(
    private payrollService: PayrollProcessingService,
    private router: Router
  ) {}

  ngOnInit() {
    this.initializeYearOptions();
    this.loadRuns();
  }

  initializeYearOptions() {
    const currentYear = new Date().getFullYear();
    const years = [];
    for (let i = currentYear; i >= currentYear - 5; i--) {
      years.push({ label: i.toString(), value: i });
    }
    this.yearOptions.set(years);
  }

  loadRuns() {
    this.loading.set(true);
    this.payrollService.getPayrollRuns(this.selectedYear(), this.selectedStatus() || undefined).subscribe({
      next: (response) => {
        if (response.succeeded) {
          this.runs.set(response.data || []);
          this.applyFilters();
        }
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }

  applyFilters() {
    let filtered = [...this.runs()];

    if (this.searchTerm()) {
      const term = this.searchTerm().toLowerCase();
      filtered = filtered.filter(r => 
        r.runId.toString().includes(term) ||
        r.processedBy?.toLowerCase().includes(term)
      );
    }

    this.filteredRuns.set(filtered);
  }

  clearFilters() {
    this.searchTerm.set('');
    this.selectedYear.set(new Date().getFullYear());
    this.selectedStatus.set(null);
    this.loadRuns();
  }

  viewDetails(run: PayrollRun) {
    this.router.navigate(['/payroll/processing/runs', run.runId]);
  }

  formatCurrency(value: number): string {
    return new Intl.NumberFormat('ar-YE', { 
      style: 'currency', 
      currency: 'YER',
      maximumFractionDigits: 0 
    }).format(value);
  }

  getMonthName(month: number): string {
    const months = ['يناير', 'فبراير', 'مارس', 'أبريل', 'مايو', 'يونيو',
                    'يوليو', 'أغسطس', 'سبتمبر', 'أكتوبر', 'نوفمبر', 'ديسمبر'];
    return months[month - 1] || '';
  }

  getStatusSeverity(status: string): 'success' | 'info' | 'warn' | 'danger' | 'secondary' {
    switch (status) {
      case 'COMPLETED': return 'success';
      case 'POSTED': return 'info';
      case 'PROCESSING': return 'info';
      case 'PENDING': return 'warn';
      case 'FAILED': return 'danger';
      default: return 'secondary';
    }
  }

  getStatusLabel(status: string): string {
    switch (status) {
      case 'COMPLETED': return 'مكتمل';
      case 'POSTED': return 'تم الترحيل';
      case 'PROCESSING': return 'جاري المعالجة';
      case 'PENDING': return 'معلق';
      case 'FAILED': return 'فشل';
      default: return status;
    }
  }
}
