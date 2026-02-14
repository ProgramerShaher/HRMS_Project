import { Component, Input, Output, EventEmitter, OnInit, signal, inject, forwardRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, NG_VALUE_ACCESSOR, ControlValueAccessor } from '@angular/forms';
import { AutoCompleteModule } from 'primeng/autocomplete';
import { EmployeeService } from '../../services/employee.service';
import { Employee } from '../../models/employee.models';

@Component({
  selector: 'app-employee-selector',
  standalone: true,
  imports: [CommonModule, FormsModule, AutoCompleteModule],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => EmployeeSelectorComponent),
      multi: true
    }
  ],
  template: `
    <div class="employee-selector">
      <p-autoComplete
        [(ngModel)]="selectedEmployee"
        [suggestions]="filteredEmployees()"
        (completeMethod)="searchEmployees($event)"
        (onSelect)="onEmployeeSelect($event)"
        (onClear)="onClear()"
        field="fullNameAr"
        [dropdown]="true"
        [forceSelection]="true"
        [placeholder]="placeholder"
        [disabled]="disabled"
        styleClass="w-full">
        
        <ng-template let-employee pTemplate="item">
          <div class="flex items-center gap-3 p-2">
            <div class="w-10 h-10 rounded-full bg-gradient-to-br from-blue-500 to-indigo-600 flex items-center justify-center text-white font-bold">
              {{ getInitials(employee.fullNameAr) }}
            </div>
            <div class="flex-1">
              <p class="font-semibold text-slate-900 dark:text-white">{{ employee.fullNameAr }}</p>
              <div class="flex items-center gap-2 text-xs text-slate-500 dark:text-zinc-400">
                <span class="px-2 py-0.5 bg-blue-50 dark:bg-blue-900/30 text-blue-600 dark:text-blue-400 rounded">
                  {{ employee.employeeCode }}
                </span>
                @if (employee.jobTitle) {
                  <span>{{ employee.jobTitle }}</span>
                }
                @if (employee.departmentName) {
                  <span>• {{ employee.departmentName }}</span>
                }
              </div>
            </div>
          </div>
        </ng-template>

        <ng-template pTemplate="empty">
          <div class="text-center p-4 text-slate-500">
            <i class="pi pi-search text-2xl mb-2"></i>
            <p>لا توجد نتائج</p>
          </div>
        </ng-template>
      </p-autoComplete>

      @if (selectedEmployee && showDetails) {
        <div class="mt-3 p-3 bg-blue-50 dark:bg-blue-900/20 rounded-xl border border-blue-200 dark:border-blue-800">
          <div class="flex items-center gap-3">
            <div class="w-12 h-12 rounded-full bg-gradient-to-br from-blue-500 to-indigo-600 flex items-center justify-center text-white font-bold text-lg">
              {{ getInitials(selectedEmployee.fullNameAr) }}
            </div>
            <div class="flex-1">
              <p class="font-bold text-slate-900 dark:text-white">{{ selectedEmployee.fullNameAr }}</p>
              <p class="text-sm text-slate-600 dark:text-zinc-400">{{ selectedEmployee.employeeCode }}</p>
              @if (selectedEmployee.jobTitle) {
                <p class="text-xs text-slate-500 dark:text-zinc-500">{{ selectedEmployee.jobTitle }}</p>
              }
            </div>
          </div>
        </div>
      }
    </div>
  `,
  styles: [`
    :host {
      display: block;
    }
  `]
})
export class EmployeeSelectorComponent implements OnInit, ControlValueAccessor {
  private employeeService = inject(EmployeeService);

  @Input() placeholder = 'ابحث عن موظف...';
  @Input() disabled = false;
  @Input() showDetails = true;
  @Output() employeeSelected = new EventEmitter<Employee>();

  selectedEmployee: Employee | null = null;
  filteredEmployees = signal<Employee[]>([]);
  allEmployees = signal<Employee[]>([]);

  // ControlValueAccessor
  private onChange: any = () => {};
  private onTouched: any = () => {};

  ngOnInit() {
    this.loadEmployees();
  }

  loadEmployees() {
    this.employeeService.getActiveEmployees().subscribe({
      next: (response: any) => {
        // Handle different API response structures:
        // 1. Result<PagedResult<T>> -> response.data.items
        // 2. Result<List<T>> -> response.data
        // 3. Direct array -> response
        let data = [];
        if (Array.isArray(response)) {
          data = response;
        } else if (response?.data) {
          data = Array.isArray(response.data) ? response.data : (response.data.items || []);
        }

        if (Array.isArray(data)) {
          this.allEmployees.set(data);
        }
      },
      error: (err) => console.error('Error loading employees:', err)
    });
  }

  searchEmployees(event: any) {
    const query = event.query.toLowerCase();
    const filtered = this.allEmployees().filter(emp => 
      emp.fullNameAr.toLowerCase().includes(query) ||
      emp.employeeCode.toLowerCase().includes(query) ||
      emp.jobTitle?.toLowerCase().includes(query) ||
      emp.departmentName?.toLowerCase().includes(query)
    );
    this.filteredEmployees.set(filtered);
  }

  onEmployeeSelect(event: any) {
    this.selectedEmployee = event;
    this.onChange(event?.employeeId);
    this.onTouched();
    this.employeeSelected.emit(event);
  }

  onClear() {
    this.selectedEmployee = null;
    this.onChange(null);
    this.onTouched();
    this.employeeSelected.emit(null as any);
  }

  getInitials(name: string): string {
    if (!name) return '؟';
    const words = name.trim().split(' ');
    if (words.length >= 2) {
      return words[0].charAt(0) + words[1].charAt(0);
    }
    return name.charAt(0);
  }

  // ControlValueAccessor implementation
  writeValue(value: number | null): void {
    if (value) {
      const employee = this.allEmployees().find(e => e.employeeId === value);
      if (employee) {
        this.selectedEmployee = employee;
      } else {
        // Load from API if not in cache
        this.employeeService.getEmployeeById(value).subscribe({
          next: (response) => {
            if (response.succeeded && response.data) {
              this.selectedEmployee = response.data;
            }
          }
        });
      }
    } else {
      this.selectedEmployee = null;
    }
  }

  registerOnChange(fn: any): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: any): void {
    this.onTouched = fn;
  }

  setDisabledState(isDisabled: boolean): void {
    this.disabled = isDisabled;
  }
}
