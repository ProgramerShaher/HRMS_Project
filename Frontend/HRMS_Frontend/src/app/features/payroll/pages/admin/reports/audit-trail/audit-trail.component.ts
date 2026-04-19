import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { SelectModule } from 'primeng/select';
import { DatePickerModule } from 'primeng/datepicker';
import { TagModule } from 'primeng/tag';
import { ActionButtonsComponent } from '../../../../../../shared/components/action-buttons/action-buttons.component';

import { PayrollProcessingService } from '../../../../services/payroll-processing.service';

interface AuditLog {
  auditId: number;
  entityName: string;
  entityId: number;
  action: string;
  performedBy: string;
  performedAt: string;
  oldValues: string | null;
  newValues: string | null;
  notes: string | null;
}

@Component({
  selector: 'app-audit-trail',
  standalone: true,
  imports: [CommonModule, FormsModule, TableModule, ButtonModule, InputTextModule, SelectModule, DatePickerModule, TagModule, ActionButtonsComponent],
  templateUrl: './audit-trail.component.html',
  styleUrl: './audit-trail.component.scss'
})
export class AuditTrailComponent implements OnInit {
  logs = signal<AuditLog[]>([]);
  filteredLogs = signal<AuditLog[]>([]);
  loading = signal(false);
  searchTerm = signal('');

  constructor(private payrollService: PayrollProcessingService) {}

  ngOnInit() {
    this.loadLogs();
  }

  loadLogs() {
    this.loading.set(true);
    this.payrollService.getAuditTrail().subscribe({
      next: (response) => {
        if (response.succeeded) {
          this.logs.set(response.data || []);
          this.applyFilters();
        }
        this.loading.set(false);
      },
      error: (err) => {
        console.error('Error loading audit logs:', err);
        this.loading.set(false);
      }
    });
  }

  applyFilters() {
    let filtered = [...this.logs()];
    if (this.searchTerm()) {
      const term = this.searchTerm().toLowerCase();
      filtered = filtered.filter(l => 
        l.entityName.toLowerCase().includes(term) ||
        l.performedBy.toLowerCase().includes(term) ||
        l.action.toLowerCase().includes(term) ||
        this.parseValues(l.newValues).toLowerCase().includes(term)
      );
    }
    this.filteredLogs.set(filtered);
  }

  parseValues(jsonString: string | null): string {
    if (!jsonString) return '-';
    try {
      const data = JSON.parse(jsonString);
      // Condensed view: take first 3 relevant keys or common ones
      const relevantKeys = ['ElementNameAr', 'Amount', 'EmployeeNameAr', 'Status', 'ElementType'];
      const parts = Object.keys(data)
        .filter(key => relevantKeys.includes(key) || (!key.includes('Id') && !key.includes('At') && !key.includes('By')))
        .slice(0, 3)
        .map(key => `${key}: ${data[key]}`);
      
      return parts.join(' | ') || 'بيانات تقنية';
    } catch {
      return 'خطأ في قراءة البيانات';
    }
  }

  formatDate(date: string): string {
    return new Date(date).toLocaleString('ar-YE');
  }

  getEntityLabel(entity: string): string {
    const labels: { [key: string]: string } = {
      'SALARY_ELEMENTS': 'عناصر الراتب',
      'EMPLOYEE_STRUCTURE': 'هيكل الراتب',
      'LOANS': 'السلف',
      'PAYROLL_RUNS': 'مسيرات الرواتب'
    };
    return labels[entity] || entity;
  }

  getActionSeverity(action: string): "success" | "secondary" | "info" | "warn" | "danger" | "contrast" | undefined {
    switch (action.toLowerCase()) {
      case 'added': return 'success';
      case 'updated': return 'info';
      case 'deleted': return 'danger';
      default: return 'secondary';
    }
  }

  getActionLabel(action: string): string {
    const labels: { [key: string]: string } = {
      'Added': 'إضافة',
      'Updated': 'تعديل',
      'Deleted': 'حذف'
    };
    return labels[action] || action;
  }
}
