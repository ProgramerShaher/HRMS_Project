import { Component, inject, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AttendanceSettingsService } from '../../services/attendance-settings.service';
import { ActionShiftSwapCommand, ActionOvertimeCommand, PendingApprovalDto } from '../../models/attendance.models';
import { MessageService } from 'primeng/api';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { ToastModule } from 'primeng/toast';
import { DialogModule } from 'primeng/dialog';
import { TextareaModule } from 'primeng/textarea';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-approvals',
  standalone: true,
  imports: [CommonModule, TableModule, ButtonModule, ToastModule, DialogModule, TextareaModule, FormsModule],
  providers: [MessageService],
  templateUrl: './approvals.component.html'
})
export class ApprovalsComponent implements OnInit {
  requests: PendingApprovalDto[] = [];
  loading = false;

  private settingsService = inject(AttendanceSettingsService);
  private messageService = inject(MessageService);
  private cdr = inject(ChangeDetectorRef);

  dialogVisible = false;
  currentRequest: any = null;
  actionType: 'APPROVE' | 'REJECT' = 'APPROVE';
  comment = '';

  ngOnInit() {
      this.loadPendingRequests();
  }

  loadPendingRequests() {
      this.loading = true;
      this.settingsService.getPendingApprovals().subscribe({
          next: (res) => {
              this.requests = res.data;
              this.loading = false;
              this.cdr.detectChanges();
          },
          error: () => {
              this.loading = false;
              this.messageService.add({severity:'error', summary: 'خطأ', detail: 'فشل تحميل الطلبات'});
              this.cdr.detectChanges();
          }
      });
  }

  openAction(request: any, action: 'APPROVE' | 'REJECT') {
      this.currentRequest = request;
      this.actionType = action;
      this.comment = '';
      this.dialogVisible = true;
  }

  submitAction() {
      if (!this.currentRequest) return;

      if (this.currentRequest.requestType === 'Swap') {
          const cmd: ActionShiftSwapCommand = {
              requestId: this.currentRequest.id,
              managerId: 1, 
              action: this.actionType,
              comment: this.comment
          };
          this.settingsService.actionSwapRequest(cmd).subscribe({
              next: () => this.successAndClose(),
              error: () => this.error()
          });
      } else if (this.currentRequest.requestType === 'Overtime') {
          const cmd: ActionOvertimeCommand = {
              requestId: this.currentRequest.id,
              managerId: 1,
              action: this.actionType,
              approvedHours: 0, 
              comment: this.comment
          };
           this.settingsService.actionOvertime(cmd).subscribe({
              next: () => this.successAndClose(),
              error: () => this.error()
          });
      } else if (this.currentRequest.requestType === 'Permission') {
          const cmd = {
              permissionRequestId: this.currentRequest.id,
              action: this.actionType === 'APPROVE' ? 'Approve' : 'Reject',
              rejectionReason: this.comment,
              approverId: 1
          };
          // Assuming we need a method for permission action in setting service or use attendance service
          // Let's use attendanceService for permissions if needed, but for now let's hope it works via a unified endpoint or separate calls
          this.settingsService.actionPermission(cmd as any).subscribe({
              next: () => this.successAndClose(),
              error: () => this.error()
          });
      }
  }

  successAndClose() {
      this.messageService.add({severity:'success', summary: 'تم', detail: 'تم تنفيذ الإجراء بنجاح'});
      this.dialogVisible = false;
      this.loadPendingRequests();
      this.currentRequest = null;
  }
  
  error() {
       this.messageService.add({severity:'error', summary: 'خطأ', detail: 'فشل تنفيذ الإجراء'});
  }
}
