import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { TagModule } from 'primeng/tag';
import { ActionButtonsComponent } from '../../../../../../shared/components/action-buttons/action-buttons.component';
import { PayrollProcessingService } from '../../../../services/payroll-processing.service';
import { SelectModule } from 'primeng/select';

interface EmployeeSalaryDetail {
  employeeId: number;
  employeeCode: string;
  employeeNameAr: string;
  departmentName?: string;
  jobTitle?: string;
  basicSalary: number;
  allowancesCount: number;
  deductionsCount: number;
  totalAllowances: number;
  totalDeductions: number;
  grossSalary: number;
  netSalary: number;
  overtimeAmount: number;
  latenessDeduction: number;
  absenceDeduction: number;
  monthlyLoanDeduction: number;
  activeLoansCount: number;
  isActive: boolean;
  lastPayrollDate?: Date;
  hasSalaryStructure: boolean;
}

/**
 * جميع رواتب الموظفين - واجهة إدارية
 * All Employees Salaries - Admin Interface
 */
@Component({
  selector: 'app-all-employees-salaries',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    TableModule,
    ButtonModule,
    InputTextModule,
    TagModule,
    SelectModule,
    ActionButtonsComponent
  ],
  templateUrl: './all-employees-salaries.component.html',
  styleUrl: './all-employees-salaries.component.scss'
})
export class AllEmployeesSalariesComponent implements OnInit {
  salaries = signal<EmployeeSalaryDetail[]>([]);
  filteredSalaries = signal<EmployeeSalaryDetail[]>([]);
  loading = signal(false);

  // Filters
  searchTerm = signal('');
  selectedDepartment = signal<any>(null);
  selectedStatus = signal<any>(null);

  departments = signal<any[]>([]);
  statusOptions = [
    { label: 'الكل', value: null },
    { label: 'نشط', value: true },
    { label: 'غير نشط', value: false }
  ];

  constructor(
    private payrollService: PayrollProcessingService,
    private router: Router
  ) {}

  ngOnInit() {
    this.loadSalaries();
  }

  loadSalaries() {
    this.loading.set(true);
    
    this.payrollService.getAllEmployeesSalaries(
      this.selectedDepartment()?.value,
      this.selectedStatus()?.value,
      this.searchTerm()
    ).subscribe({
      next: (response) => {
        if (response.succeeded && response.data) {
          this.salaries.set(response.data);
          this.filteredSalaries.set(response.data);
          
          // Extract unique departments
          const depts = [...new Set(response.data.map(s => s.departmentName).filter(d => d))];
          this.departments.set([
            { label: 'الكل', value: null },
            ...depts.map(d => ({ label: d, value: d }))
          ]);
        }
        this.loading.set(false);
      },
      error: (err) => {
        console.error('Error loading salaries:', err);
        this.loading.set(false);
      }
    });
  }

  applyFilters() {
    this.loadSalaries();
  }

  clearFilters() {
    this.searchTerm.set('');
    this.selectedDepartment.set(null);
    this.selectedStatus.set(null);
    this.loadSalaries();
  }

  viewDetails(employee: EmployeeSalaryDetail) {
    this.router.navigate(['/payroll/salaries/breakdown', employee.employeeId]);
  }

  formatCurrency(value: number): string {
    return new Intl.NumberFormat('ar-YE', {
      style: 'decimal',
      minimumFractionDigits: 0,
      maximumFractionDigits: 0
    }).format(value);
  }

  getStatusSeverity(isActive: boolean): 'success' | 'danger' {
    return isActive ? 'success' : 'danger';
  }

  getStatusLabel(isActive: boolean): string {
    return isActive ? 'نشط' : 'غير نشط';
  }

  exportToExcel() {
    // TODO: Implement export functionality
    console.log('Export to Excel');
  }
}
