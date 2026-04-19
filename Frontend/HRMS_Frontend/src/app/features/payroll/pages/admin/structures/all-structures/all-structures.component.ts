import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { TagModule } from 'primeng/tag';
import { ActionButtonsComponent } from '../../../../../../shared/components/action-buttons/action-buttons.component';
import { SalaryStructureService } from '../../../../services/salary-structure.service';

import { SalaryStructureSummary } from '../../../../models/salary-structure.models';

/**
 * جميع هياكل الرواتب - واجهة إدارية
 * All Salary Structures - Admin Interface
 */
@Component({
  selector: 'app-all-structures',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    TableModule,
    ButtonModule,
    InputTextModule,
    TagModule,
    ActionButtonsComponent
  ],
  templateUrl: './all-structures.component.html',
  styleUrl: './all-structures.component.scss'
})
export class AllStructuresComponent implements OnInit {
  structures = signal<SalaryStructureSummary[]>([]);
  filteredStructures = signal<SalaryStructureSummary[]>([]);
  loading = signal(false);

  searchTerm = signal('');

  constructor(
    private structureService: SalaryStructureService,
    private router: Router
  ) {}

  ngOnInit() {
    this.loadStructures();
  }

  loadStructures() {
    this.loading.set(true);
    this.structureService.getAllStructures().subscribe({
      next: (response) => {
        if (response.succeeded) {
          this.structures.set(response.data || []);
          this.applyFilters();
        }
        this.loading.set(false);
      },
      error: (err) => {
        console.error('Error loading structures:', err);
        this.loading.set(false);
      }
    });
  }

  applyFilters() {
    let filtered = [...this.structures()];

    if (this.searchTerm()) {
      const term = this.searchTerm().toLowerCase();
      filtered = filtered.filter(s => 
        s.employeeNameAr.toLowerCase().includes(term) ||
        s.employeeCode.toLowerCase().includes(term) ||
        s.departmentName?.toLowerCase().includes(term)
      );
    }

    this.filteredStructures.set(filtered);
  }

  clearFilters() {
    this.searchTerm.set('');
    this.applyFilters();
  }

  viewStructure(employee: SalaryStructureSummary) {
    this.router.navigate(['/payroll/processing/runs', employee.employeeId]); // Need to check correct route
  }

  editStructure(employee: SalaryStructureSummary) {
    this.router.navigate(['/payroll/structures/employee', employee.employeeId]);
  }

  formatCurrency(value: number): string {
    return new Intl.NumberFormat('ar-YE', {
      style: 'currency',
      currency: 'YER',
      maximumFractionDigits: 0
    }).format(value);
  }

  formatDate(date: string | null): string {
    if (!date) return '-';
    return new Date(date).toLocaleDateString('ar-YE');
  }

  getPendingCount() {
    return this.filteredStructures().filter(s => !s.hasStructure).length;
  }
}
