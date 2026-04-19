import { Component, OnInit, signal, inject, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { TagModule } from 'primeng/tag';
import { SelectModule } from 'primeng/select';
import { InputTextModule } from 'primeng/inputtext';
import { ToastModule } from 'primeng/toast';
import { DialogModule } from 'primeng/dialog';
import { TextareaModule } from 'primeng/textarea';
import { MessageService } from 'primeng/api';
import { LeaveRequestService } from '../../services/leave-request.service';
import { LeaveRequest, LeaveRequestStatus } from '../../models/leave.models';
import { LeaveConfigurationService } from '../../services/leave-configuration.service';
import { LeaveRequestFormComponent } from '../../components/leave-request-form/leave-request-form.component';

@Component({
  selector: 'app-all-requests',
  standalone: true,
  imports: [
    CommonModule, FormsModule, TableModule, ButtonModule, TagModule,
    SelectModule, InputTextModule, ToastModule, DialogModule, TextareaModule,
    LeaveRequestFormComponent
  ],
  providers: [MessageService],
  templateUrl: './all-requests.component.html',
  styles: [`:host { display: block; }`]
})
export class AllRequestsComponent implements OnInit {
  private service  = inject(LeaveRequestService);
  private msgSvc   = inject(MessageService);
  private cfgSvc   = inject(LeaveConfigurationService);

  allRequests  = signal<LeaveRequest[]>([]);
  filtered     = signal<LeaveRequest[]>([]);
  loading      = signal(false);
  cancelling   = signal(false);

  // Filters
  searchText   = '';
  statusFilter: string | undefined = undefined;

  // Cancel dialog
  showCancelDialog   = false;
  selectedRequest: LeaveRequest | null = null;
  
  // Create Request dialog
  showRequestDialog  = false;

  statusOptions = [
    { label: 'جميع الحالات', value: undefined },
    { label: 'معلق',    value: 'PENDING' },
    { label: 'معتمد',   value: 'APPROVED' },
    { label: 'مرفوض',  value: 'REJECTED' },
    { label: 'ملغي',    value: 'CANCELLED' }
  ];

  ngOnInit() { this.load(); }

  load() {
    this.loading.set(true);
    this.service.getAllRequests().subscribe({
      next: (res) => {
        const data = res.succeeded ? (res.data ?? []) : [];
        // We display all requests as the base; additional filtering possible
        this.allRequests.set(data);
        this.applyFilter();
        this.loading.set(false);
      },
      error: () => {
        this.msgSvc.add({ severity: 'error', summary: 'خطأ', detail: 'فشل تحميل الطلبات' });
        this.loading.set(false);
      }
    });
  }

  applyFilter() {
    let data = this.allRequests();
    if (this.statusFilter) {
      data = data.filter(r => r.status === this.statusFilter);
    }
    if (this.searchText.trim()) {
      const term = this.searchText.toLowerCase();
      data = data.filter(r =>
        r.employeeName?.toLowerCase().includes(term) ||
        r.leaveTypeName?.toLowerCase().includes(term) ||
        r.departmentName?.toLowerCase().includes(term)
      );
    }
    this.filtered.set(data);
  }

  resetFilters() {
    this.searchText   = '';
    this.statusFilter = undefined;
    this.applyFilter();
  }

  openCancelDialog(req: LeaveRequest) {
    this.selectedRequest = req;
    this.showCancelDialog = true;
  }

  confirmCancel() {
    if (!this.selectedRequest?.requestId) return;
    this.cancelling.set(true);
    this.service.cancelRequest(this.selectedRequest.requestId).subscribe({
      next: (res) => {
        this.cancelling.set(false);
        if (res.succeeded) {
          this.msgSvc.add({ severity: 'success', summary: 'تم', detail: 'تم إلغاء الطلب بنجاح' });
          this.showCancelDialog = false;
          this.load();
        } else {
          this.msgSvc.add({ severity: 'error', summary: 'خطأ', detail: res.message });
        }
      },
      error: () => {
        this.cancelling.set(false);
        this.msgSvc.add({ severity: 'error', summary: 'خطأ', detail: 'فشل إلغاء الطلب' });
      }
    });
  }

  onFormSubmitted() {
    this.showRequestDialog = false;
    this.msgSvc.add({ severity: 'success', summary: 'نجح', detail: 'تم تقديم طلب الإجازة بنجاح' });
    this.load();
  }

  getStatusSeverity(status: string): 'success' | 'warn' | 'danger' | 'info' {
    const map: Record<string, any> = {
      PENDING: 'warn', MANAGER_APPROVED: 'info', HR_APPROVED: 'info',
      APPROVED: 'success', REJECTED: 'danger', CANCELLED: 'danger'
    };
    return map[status] ?? 'info';
  }

  getStatusLabel(status: string): string {
    const labels: Record<string, string> = {
      PENDING: 'معلق', MANAGER_APPROVED: 'معتمد المدير',
      HR_APPROVED: 'معتمد HR', APPROVED: 'معتمد',
      REJECTED: 'مرفوض', CANCELLED: 'ملغي'
    };
    return labels[status] ?? status;
  }

  canCancel(status: string): boolean {
    return ['PENDING', 'MANAGER_APPROVED'].includes(status);
  }
}
