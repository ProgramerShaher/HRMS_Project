import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AttendanceSettingsService } from '../../services/attendance-settings.service';
import { ActionShiftSwapCommand, ActionOvertimeCommand } from '../../models/attendance.models';
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
  // Mock Data for now as endpoints usually filter by status in backend
  requests: any[] = [
    { id: 101, type: 'Swap', employeeName: 'أحمد علي', date: '2026-02-12', details: 'تبديل مع خالد محمد', status: 'Pending' },
    { id: 102, type: 'Overtime', employeeName: 'سارة حسن', date: '2026-02-13', details: '3 ساعات - ضغط عمل', status: 'Pending' }
  ];

  private settingsService = inject(AttendanceSettingsService);
  private messageService = inject(MessageService);

  dialogVisible = false;
  currentRequest: any = null;
  actionType: 'Approve' | 'Reject' = 'Approve';
  comment = '';

  ngOnInit() {
      // In real app, call service.getPendingRequests() 
  }

  openAction(request: any, action: 'Approve' | 'Reject') {
      this.currentRequest = request;
      this.actionType = action;
      this.comment = '';
      this.dialogVisible = true;
  }

  submitAction() {
      if (!this.currentRequest) return;

      if (this.currentRequest.type === 'Swap') {
          const cmd: ActionShiftSwapCommand = {
              requestId: this.currentRequest.id,
              managerId: 1, // Mock current user
              action: this.actionType,
              comment: this.comment
          };
          this.settingsService.actionSwapRequest(cmd).subscribe({
              next: () => this.successAndClose(),
              error: () => this.error()
          });
      } else if (this.currentRequest.type === 'Overtime') {
          const cmd: ActionOvertimeCommand = {
              requestId: this.currentRequest.id,
              managerId: 1,
              action: this.actionType,
              approvedHours: 0, // Should be input if approval
              comment: this.comment
          };
           this.settingsService.actionOvertime(cmd).subscribe({
              next: () => this.successAndClose(),
              error: () => this.error()
          });
      }
  }

  successAndClose() {
      this.messageService.add({severity:'success', summary: 'تم', detail: 'تم تنفيذ الإجراء بنجاح'});
      this.dialogVisible = false;
      // remove from list locally
      this.requests = this.requests.filter(r => r.id !== this.currentRequest.id);
      this.currentRequest = null;
  }
  
  error() {
       this.messageService.add({severity:'error', summary: 'خطأ', detail: 'فشل تنفيذ الإجراء'});
  }
}
