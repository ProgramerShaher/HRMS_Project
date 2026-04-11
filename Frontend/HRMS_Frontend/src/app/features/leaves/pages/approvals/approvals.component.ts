import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { TextareaModule } from 'primeng/textarea';
import { TagModule } from 'primeng/tag';
import { ToastModule } from 'primeng/toast';
import { MessageService } from 'primeng/api';
import { LeaveRequestService } from '../../services/leave-request.service';
import { LeaveRequest } from '../../models/leave.models';

@Component({
  selector: 'app-approvals',
  standalone: true,
  imports: [
    CommonModule, FormsModule, TableModule, ButtonModule,
    DialogModule, TextareaModule, TagModule, ToastModule
  ],
  providers: [MessageService],
  templateUrl: './approvals.component.html',
  styles: [`:host { display: block; }`]
})
export class ApprovalsComponent implements OnInit {
  private service   = inject(LeaveRequestService);
  private msgSvc    = inject(MessageService);

  pendingRequests   = signal<LeaveRequest[]>([]);
  loading           = signal(false);
  approving         = signal(false);
  rejecting         = signal(false);

  // Approve dialog
  showApproveDialog = false;
  approveComments   = '';
  selectedRequest: LeaveRequest | null = null;

  // Reject dialog
  showRejectDialog  = false;
  rejectionReason   = '';

  ngOnInit() { this.load(); }

  load() {
    this.loading.set(true);
    this.service.getPendingRequests().subscribe({
      next: (res) => {
        if (res.succeeded) this.pendingRequests.set(res.data ?? []);
        else this.toast('warn', 'تنبيه', res.message);
        this.loading.set(false);
      },
      error: () => {
        this.toast('error', 'خطأ', 'فشل تحميل الطلبات المعلقة');
        this.loading.set(false);
      }
    });
  }

  openApprove(req: LeaveRequest) {
    this.selectedRequest = req;
    this.approveComments = '';
    this.showApproveDialog = true;
  }

  confirmApprove() {
    if (!this.selectedRequest) return;
    this.approving.set(true);
    this.service.approveRequest(this.selectedRequest.requestId!, this.approveComments).subscribe({
      next: (res) => {
        this.approving.set(false);
        if (res.succeeded) {
          this.toast('success', 'تمت الموافقة', `تمت الموافقة على إجازة ${this.selectedRequest?.employeeName}`);
          this.showApproveDialog = false;
          this.load();
        } else {
          this.toast('error', 'خطأ', res.message);
        }
      },
      error: () => {
        this.approving.set(false);
        this.toast('error', 'خطأ', 'فشلت عملية الاعتماد');
      }
    });
  }

  openReject(req: LeaveRequest) {
    this.selectedRequest = req;
    this.rejectionReason = '';
    this.showRejectDialog = true;
  }

  confirmReject() {
    if (!this.selectedRequest || !this.rejectionReason.trim()) {
      this.toast('warn', 'تنبيه', 'يجب كتابة سبب الرفض');
      return;
    }
    this.rejecting.set(true);
    this.service.rejectRequest(this.selectedRequest.requestId!, this.rejectionReason.trim()).subscribe({
      next: (res) => {
        this.rejecting.set(false);
        if (res.succeeded) {
          this.toast('success', 'تم الرفض', `تم رفض طلب إجازة ${this.selectedRequest?.employeeName}`);
          this.showRejectDialog = false;
          this.load();
        } else {
          this.toast('error', 'خطأ', res.message);
        }
      },
      error: () => {
        this.rejecting.set(false);
        this.toast('error', 'خطأ', 'فشلت عملية الرفض');
      }
    });
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
      PENDING: 'معلق', MANAGER_APPROVED: 'معتمد من المدير',
      HR_APPROVED: 'معتمد من HR', APPROVED: 'معتمد',
      REJECTED: 'مرفوض', CANCELLED: 'ملغي'
    };
    return labels[status] ?? status;
  }

  private toast(severity: string, summary: string, detail: string) {
    this.msgSvc.add({ severity, summary, detail, life: 4000 });
  }
}
