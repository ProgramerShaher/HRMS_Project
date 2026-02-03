import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { AvatarModule } from 'primeng/avatar';
import { TagModule } from 'primeng/tag';
import { MenuModule } from 'primeng/menu';
import { ToastModule } from 'primeng/toast';
import { TooltipModule } from 'primeng/tooltip';
import { MenuItem, MessageService } from 'primeng/api';
import { EmployeeService } from '../../services/employee.service';
import { Employee } from '../../models/employee.model';
import { catchError, finalize } from 'rxjs/operators';
import { of } from 'rxjs';

@Component({
  selector: 'app-employees-list',
  standalone: true,
  imports: [
    CommonModule, 
    RouterModule, 
    TableModule, 
    ButtonModule, 
    InputTextModule, 
    AvatarModule, 
    TagModule, 
    MenuModule, 
    ToastModule,
    TooltipModule
  ],
  providers: [MessageService],
  templateUrl: './employees-list.component.html',
  styleUrls: ['./employees-list.component.scss']
})
export class EmployeesListComponent implements OnInit {
  // Services
  private employeeService = inject(EmployeeService);
  private router = inject(Router);
  private messageService = inject(MessageService);

  // State
  employees = signal<Employee[]>([]);
  totalRecords = signal<number>(0);
  loading = signal<boolean>(true);
  
  // Pagination & Search
  first = 0;
  rows = 10;
  searchQuery = '';

  ngOnInit() {
    this.loadEmployees();
  }

  loadEmployees(event?: any) {
    this.loading.set(true);
    const page = event ? (event.first / event.rows) + 1 : 1;
    this.rows = event ? event.rows : 10;

    this.employeeService.getAll(page, this.rows, this.searchQuery)
      .pipe(
        catchError(error => {
          this.messageService.add({ severity: 'error', summary: 'خطأ', detail: 'فشل في تحميل بيانات الموظفين' });
          return of({ items: [], totalCount: 0 });
        }),
        finalize(() => this.loading.set(false))
      )
      .subscribe(response => {
        this.employees.set(response.items || []);
        this.totalRecords.set(response.totalCount || 0);
      });
  }

  onSearch(value: string) {
    this.searchQuery = value;
    // Debounce structure could be added here, for now direct call reset to page 1
    this.loadEmployees({ first: 0, rows: this.rows }); 
  }

  // Navigation
  goToWizard() {
    this.router.navigate(['/employees/new']);
  }

  viewProfile(id: number) {
    this.router.navigate(['/employees', id]);
  }

  editEmployee(id: number) {
    this.router.navigate(['/employees/edit', id]);
  }

  // Row Actions (Optional Context Menu)
  getMenuItems(employee: Employee): MenuItem[] {
    return [
      { label: 'عرض الملف', icon: 'pi pi-user', command: () => this.viewProfile(employee.employeeId) },
      { label: 'تعديل', icon: 'pi pi-pencil', command: () => this.editEmployee(employee.employeeId) },
    ];
  }

  // Helpers
  getInitials(name: string): string {
    if (!name) return 'EMP';
    const parts = name.split(' ');
    if (parts.length === 1) return parts[0].substring(0, 2).toUpperCase();
    return (parts[0][0] + parts[1][0]).toUpperCase();
  }
}
