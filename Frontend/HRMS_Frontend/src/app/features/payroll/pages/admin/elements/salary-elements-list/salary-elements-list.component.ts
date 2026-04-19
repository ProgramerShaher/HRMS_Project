import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { SelectModule } from 'primeng/select';
import { TagModule } from 'primeng/tag';
import { TooltipModule } from 'primeng/tooltip';
import { ActionButtonsComponent } from '../../../../../../shared/components/action-buttons/action-buttons.component';
import { SalaryElementService } from '../../../../services/salary-element.service';

interface SalaryElement {
  elementId: number;
  elementNameAr: string;
  elementNameEn: string;
  elementType: 'ALLOWANCE' | 'DEDUCTION';
  calculationType: 'FIXED' | 'PERCENTAGE';
  defaultValue: number;
  isActive: boolean;
  isTaxable: boolean;
  displayOrder: number;
}

/**
 * قائمة عناصر الراتب - واجهة إدارية
 * Salary Elements List - Admin Interface
 */
@Component({
  selector: 'app-salary-elements-list',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    TableModule,
    ButtonModule,
    InputTextModule,
    SelectModule,
    TagModule,
    TooltipModule,
    ActionButtonsComponent
  ],
  templateUrl: 'salary-elements-list.component.html'
})
export class SalaryElementsListComponent implements OnInit {
  elements = signal<SalaryElement[]>([]);
  filteredElements = signal<SalaryElement[]>([]);
  loading = signal(false);

  // Filters
  searchTerm = signal('');
  selectedType = signal<any>(null);
  selectedStatus = signal<any>(null);

  typeOptions = [
    { label: 'الكل', value: null },
    { label: 'بدل', value: 'ALLOWANCE' },
    { label: 'خصم', value: 'DEDUCTION' }
  ];

  statusOptions = [
    { label: 'الكل', value: null },
    { label: 'نشط', value: true },
    { label: 'غير نشط', value: false }
  ];

  constructor(
    private elementService: SalaryElementService,
    private router: Router
  ) {}

  ngOnInit() {
    this.loadElements();
  }

  loadElements() {
    this.loading.set(true);
    
    this.elementService.getElements().subscribe({
      next: (response: any) => {
        if (response.succeeded && response.data) {
          this.elements.set(response.data);
          this.applyFilters();
        }
        this.loading.set(false);
      },
      error: (err: any) => {
        console.error('Error loading elements:', err);
        this.loading.set(false);
      }
    });
  }

  applyFilters() {
    let filtered = [...this.elements()];

    // Search filter
    if (this.searchTerm()) {
      const term = this.searchTerm().toLowerCase();
      filtered = filtered.filter(el => 
        el.elementNameAr.toLowerCase().includes(term) ||
        el.elementNameEn.toLowerCase().includes(term)
      );
    }

    // Type filter
    if (this.selectedType()?.value) {
      filtered = filtered.filter(el => el.elementType === this.selectedType().value);
    }

    // Status filter
    if (this.selectedStatus()?.value !== null && this.selectedStatus()?.value !== undefined) {
      filtered = filtered.filter(el => el.isActive === this.selectedStatus().value);
    }

    this.filteredElements.set(filtered);
  }

  clearFilters() {
    this.searchTerm.set('');
    this.selectedType.set(null);
    this.selectedStatus.set(null);
    this.applyFilters();
  }

  createNew() {
    this.router.navigate(['/payroll/elements/form']);
  }

  editElement(element: SalaryElement) {
    this.router.navigate(['/payroll/elements/form', element.elementId]);
  }

  toggleStatus(element: SalaryElement) {
    // TODO: Implement toggle status
    console.log('Toggle status for:', element);
  }

  getTypeSeverity(type: string): 'success' | 'warn' {
    return type === 'ALLOWANCE' ? 'success' : 'warn';
  }

  getTypeLabel(type: string): string {
    return type === 'ALLOWANCE' ? 'بدل' : 'خصم';
  }

  getTypeIcon(type: string): string {
    return type === 'ALLOWANCE' ? 'pi-plus-circle' : 'pi-minus-circle';
  }

  getCalculationTypeLabel(type: string): string {
    return type === 'FIXED' ? 'ثابت' : 'نسبة مئوية';
  }

  formatValue(value: number, type: string): string {
    if (type === 'PERCENTAGE') {
      return `${value}%`;
    }
    return new Intl.NumberFormat('ar-YE', { style: 'decimal' }).format(value);
  }

  getAllowancesCount(): number {
    return this.filteredElements().filter(e => e.elementType === 'ALLOWANCE').length;
  }

  getDeductionsCount(): number {
    return this.filteredElements().filter(e => e.elementType === 'DEDUCTION').length;
  }
}
