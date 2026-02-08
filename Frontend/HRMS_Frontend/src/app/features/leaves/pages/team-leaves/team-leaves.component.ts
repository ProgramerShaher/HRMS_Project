import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MessageService } from 'primeng/api';
import { ToastModule } from 'primeng/toast';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { TagModule } from 'primeng/tag';
import { InputTextModule } from 'primeng/inputtext';
import { LeaveRequestService } from '../../services/leave-request.service';
import { LeaveRequest } from '../../models/leave.models';

@Component({
  selector: 'app-team-leaves',
  standalone: true,
  imports: [CommonModule, FormsModule, ToastModule, TableModule, ButtonModule, TagModule, InputTextModule],
  providers: [MessageService],
  template: `
    <div class="p-4">
      <p-toast></p-toast>
      
      <div class="bg-gradient-to-r from-purple-600 to-purple-700 rounded-lg shadow-md p-4 text-white mb-4">
        <h1 class="text-2xl font-bold flex items-center gap-2">
          <i class="pi pi-users"></i>
          طلبات إجازات الفريق
        </h1>
        <p class="text-sm text-purple-100 mt-1">الموافقة والرفض على طلبات الإجازات</p>
      </div>

      <div class="bg-white rounded-lg shadow-sm">
        <p-table 
          [value]="pendingRequests()" 
          [paginator]="true" 
          [rows]="10"
          [loading]="loading()"
          styleClass="p-datatable-sm">
          
          <ng-template pTemplate="header">
            <tr>
              <th>الموظف</th>
              <th>نوع الإجازة</th>
              <th>من تاريخ</th>
              <th>إلى تاريخ</th>
              <th>الأيام</th>
              <th>السبب</th>
              <th>الحالة</th>
              <th>إجراءات</th>
            </tr>
          </ng-template>

          <ng-template pTemplate="body" let-request>
            <tr>
              <td>{{ request.employeeName }}</td>
              <td>{{ request.leaveTypeName }}</td>
              <td>{{ request.startDate | date: 'yyyy-MM-dd' }}</td>
              <td>{{ request.endDate | date: 'yyyy-MM-dd' }}</td>
              <td><span class="font-semibold">{{ request.daysCount }}</span></td>
              <td class="text-xs">{{ request.reason | slice:0:50 }}...</td>
              <td>
                <p-tag value="معلق" severity="warn"></p-tag>
              </td>
              <td>
                <div class="flex gap-1">
                  <p-button 
                    icon="pi pi-check" 
                    severity="success" 
                    size="small"
                    [outlined]="true"
                    pTooltip="موافقة"
                    (onClick)="approveRequest(request.requestId!)">
                  </p-button>
                  <p-button 
                    icon="pi pi-times" 
                    severity="danger" 
                    size="small"
                    [outlined]="true"
                    pTooltip="رفض"
                    (onClick)="rejectRequest(request.requestId!)">
                  </p-button>
                </div>
              </td>
            </tr>
          </ng-template>

          <ng-template pTemplate="emptymessage">
            <tr>
              <td colspan="8" class="text-center py-8 text-gray-500">
                <i class="pi pi-inbox text-4xl mb-2 block"></i>
                لا توجد طلبات معلقة
              </td>
            </tr>
          </ng-template>
        </p-table>
      </div>
    </div>
  `,
  styles: [`:host { display: block; }`]
})
export class TeamLeavesComponent implements OnInit {
  pendingRequests = signal<LeaveRequest[]>([]);
  loading = signal(false);

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
      error: () => {
        this.loading.set(false);
        this.messageService.add({ severity: 'error', summary: 'خطأ', detail: 'فشل تحميل الطلبات' });
      }
    });
  }

  approveRequest(requestId: number) {
    if (confirm('هل أنت متأكد من الموافقة على هذا الطلب؟')) {
      this.leaveRequestService.approveRequest(requestId).subscribe({
        next: (res) => {
          if (res.succeeded) {
            this.messageService.add({ severity: 'success', summary: 'نجح', detail: 'تمت الموافقة بنجاح' });
            this.loadPendingRequests();
          }
        },
        error: () => {
          this.messageService.add({ severity: 'error', summary: 'خطأ', detail: 'فشلت الموافقة' });
        }
      });
    }
  }

  rejectRequest(requestId: number) {
    const reason = prompt('أدخل سبب الرفض:');
    if (reason) {
      this.leaveRequestService.rejectRequest(requestId, reason).subscribe({
        next: (res) => {
          if (res.succeeded) {
            this.messageService.add({ severity: 'success', summary: 'نجح', detail: 'تم رفض الطلب' });
            this.loadPendingRequests();
          }
        },
        error: () => {
          this.messageService.add({ severity: 'error', summary: 'خطأ', detail: 'فشل رفض الطلب' });
        }
      });
    }
  }
}
