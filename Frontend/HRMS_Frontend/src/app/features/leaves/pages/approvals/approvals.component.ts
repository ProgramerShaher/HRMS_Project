import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { TextareaModule } from 'primeng/textarea';
import { MessageService } from 'primeng/api';
import { ToastModule } from 'primeng/toast';
import { LeaveRequestService } from '../../services/leave-request.service';
import { LeaveRequest } from '../../models/leave.models';

@Component({
  selector: 'app-approvals',
  standalone: true,
  imports: [
    CommonModule, 
    FormsModule, 
    TableModule, 
    ButtonModule, 
    DialogModule, 
    TextareaModule,
    ToastModule
  ],
  providers: [MessageService],
  templateUrl: './approvals.component.html',
  styles: [`
    :host { display: block; }
    .custom-table ::ng-deep .p-datatable-thead > tr > th {
      @apply bg-slate-50 dark:bg-zinc-800/50 text-slate-700 dark:text-zinc-300 font-bold border-none;
    }
    .custom-table ::ng-deep .p-datatable-tbody > tr {
      @apply border-b border-slate-50 dark:border-zinc-800/50;
    }
  `]
})
export class ApprovalsComponent implements OnInit {
  pendingRequests = signal<LeaveRequest[]>([]);
  loading = signal(false);
  
  showRejectDialog = false;
  rejectionReason = '';
  selectedRequest: LeaveRequest | null = null;

  constructor(
    private leaveRequestService: LeaveRequestService,
    private messageService: MessageService
  ) {}

  ngOnInit() {
    this.loadPendingRequests();
  }

  loadPendingRequests() {
    this.loading.set(true);
    this.leaveRequestService.getPendingRequests().subscribe({
      next: (res) => {
        if (res.succeeded) {
          this.pendingRequests.set(res.data);
        }
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }

  approve(request: LeaveRequest) {
    if (confirm(`هل أنت متأكد من الموافقة على إجازة الموظف: ${request.employeeName}؟`)) {
      this.leaveRequestService.approveRequest(request.requestId!).subscribe({
        next: (res) => {
          if (res.succeeded) {
            this.messageService.add({ severity: 'success', summary: 'نجح', detail: 'تمت الموافقة بنجاح' });
            this.loadPendingRequests();
          }
        }
      });
    }
  }

  reject(request: LeaveRequest) {
    this.selectedRequest = request;
    this.rejectionReason = '';
    this.showRejectDialog = true;
  }

  confirmReject() {
    if (!this.selectedRequest || !this.rejectionReason) return;

    this.leaveRequestService.rejectRequest(this.selectedRequest.requestId!, this.rejectionReason).subscribe({
      next: (res) => {
        if (res.succeeded) {
          this.messageService.add({ severity: 'success', summary: 'نجح', detail: 'تم رفض الطلب' });
          this.showRejectDialog = false;
          this.loadPendingRequests();
        }
      }
    });
  }
}
