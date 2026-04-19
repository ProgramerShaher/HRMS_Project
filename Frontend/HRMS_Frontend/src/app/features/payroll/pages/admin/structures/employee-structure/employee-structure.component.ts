import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { FormsModule } from '@angular/forms';
import { CardModule } from 'primeng/card';
import { ButtonModule } from 'primeng/button';
import { TableModule } from 'primeng/table';
import { InputNumberModule } from 'primeng/inputnumber';
import { SelectModule } from 'primeng/select';
import { MessageService } from 'primeng/api';
import { ToastModule } from 'primeng/toast';
import { TagModule } from 'primeng/tag';
import { TooltipModule } from 'primeng/tooltip';
import { ActionButtonsComponent } from '../../../../../../shared/components/action-buttons/action-buttons.component';
import { SalaryStructureService } from '../../../../services/salary-structure.service';
import { SalaryElementService } from '../../../../services/salary-element.service';

import { EmployeeSalaryStructure, EmployeeStructureItem } from '../../../../models/salary-structure.models';

/**
 * هيكل راتب موظف - واجهة إدارية
 * Employee Salary Structure - Admin Interface
 */
@Component({
  selector: 'app-employee-structure',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    CardModule,
    ButtonModule,
    TableModule,
    InputNumberModule,
    SelectModule,
    ToastModule,
    TagModule,
    TooltipModule,
    ActionButtonsComponent
  ],
  providers: [MessageService],
  templateUrl: './employee-structure.component.html',
  styleUrl: './employee-structure.component.scss'
})
export class EmployeeStructureComponent implements OnInit {
  structure = signal<EmployeeSalaryStructure | null>(null);
  loading = signal(false);
  saving = signal(false);
  employeeId = signal<number>(0);
  
  editMode = signal(false);

  // Available elements for adding
  availableElements = signal<any[]>([]);
  selectedElement = signal<any>(null);

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private fb: FormBuilder,
    private structureService: SalaryStructureService,
    private elementService: SalaryElementService,
    private messageService: MessageService
  ) {}

  ngOnInit() {
    this.route.params.subscribe(params => {
      const id = +params['id'];
      if (id > 0) {
        this.employeeId.set(id);
        this.loadStructure(id);
        this.loadAvailableElements();
      }
    });
  }

  loadStructure(employeeId: number) {
    this.loading.set(true);
    this.structureService.getEmployeeStructure(employeeId).subscribe({
      next: (response) => {
        if (response.succeeded && response.data) {
          this.structure.set(response.data);
        }
        this.loading.set(false);
      },
      error: (err) => {
        console.error('Error loading structure:', err);
        this.loading.set(false);
      }
    });
  }

  loadAvailableElements() {
    this.elementService.getElements().subscribe({
      next: (response) => {
        if (response.succeeded && response.data) {
          this.availableElements.set(response.data.filter((e: any) => e.isActive));
        }
      }
    });
  }

  getEarnings(): EmployeeStructureItem[] {
    return this.structure()?.elements.filter(e => e.elementType === 'ALLOWANCE') || [];
  }

  getDeductions(): EmployeeStructureItem[] {
    return this.structure()?.elements.filter(e => e.elementType === 'DEDUCTION') || [];
  }

  formatCurrency(value: number | undefined): string {
    if (value === undefined) return '-';
    return new Intl.NumberFormat('ar-YE', {
      style: 'currency',
      currency: 'YER',
      maximumFractionDigits: 0
    }).format(value);
  }

  goBack() {
    this.router.navigate(['/payroll/structures']);
  }

  // Placeholder functions for future implementation of editing
  toggleEditMode() { this.editMode.set(!this.editMode()); }
  removeElement(element: EmployeeStructureItem) { /* Implementation */ }
  addElement() { /* Implementation */ }
}
