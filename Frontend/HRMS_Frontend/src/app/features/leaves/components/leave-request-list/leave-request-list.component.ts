import { Component, Input, Output, EventEmitter, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { TagModule } from 'primeng/tag';
import { SelectModule } from 'primeng/select';
import { LeaveRequest, LeaveRequestStatus } from '../../models/leave.models';

@Component({
  selector: 'app-leave-request-list',
  standalone: true,
  imports: [CommonModule, FormsModule, TableModule, ButtonModule, TagModule, SelectModule],
  template: `
    <div class="bg-white rounded-lg shadow-sm">
      <!-- Header with filter -->
      <div class="p-3 border-b border-gray-200 flex items-center justify-between">
        <h3 class="text-base font-semibold text-gray-800">طلبات الإجازات</h3>
        <p-select
          [(ngModel)]="selectedStatus"
          [options]="statusOptions"
          optionLabel="label"
          optionValue="value"
          placeholder="جميع الحالات"
          styleClass="w-48"
          (onChange)="onStatusChange()">
        </p-select>
      </div>

      <!-- Table -->
      <p-table 
        [value]="filteredRequests()" 
        [paginator]="true" 
        [rows]="10"
        [tableStyle]="{ 'min-width': '60rem' }"
        styleClass="p-datatable-sm">
        
        <ng-template pTemplate="header">
          <tr>
            <th style="width: 5%">#</th>
            <th style="width: 15%">نوع الإجازة</th>
            <th style="width: 12%">من تاريخ</th>
            <th style="width: 12%">إلى تاريخ</th>
            <th style="width: 8%">الأيام</th>
            <th style="width: 15%">السبب</th>
            <th style="width: 12%">الحالة</th>
            <th style="width: 13%">التاريخ</th>
            <th style="width: 8%">إجراءات</th>
          </tr>
        </ng-template>

        <ng-template pTemplate="body" let-request let-i="rowIndex">
          <tr>
            <td>{{ i + 1 }}</td>
            <td>{{ request.leaveTypeName }}</td>
            <td>{{ request.startDate | date: 'yyyy-MM-dd' }}</td>
            <td>{{ request.endDate | date: 'yyyy-MM-dd' }}</td>
            <td><span class="font-semibold">{{ request.daysCount }}</span></td>
            <td>
              <span class="text-xs text-gray-600">{{ request.reason | slice:0:30 }}...</span>
            </td>
            <td>
              <p-tag 
                [value]="getStatusLabel(request.status)" 
                [severity]="getStatusSeverity(request.status)">
              </p-tag>
            </td>
            <td class="text-xs text-gray-500">{{ request.createdAt | date: 'yyyy-MM-dd HH:mm' }}</td>
            <td>
              @if (request.status === 'PENDING') {
                <p-button 
                  icon="pi pi-times" 
                  severity="danger" 
                  size="small"
                  [outlined]="true"
                  [rounded]="true"
                  (onClick)="onCancel(request.requestId!)">
                </p-button>
              }
            </td>
          </tr>
        </ng-template>

        <ng-template pTemplate="emptymessage">
          <tr>
            <td colspan="9" class="text-center py-8 text-gray-500">
              <i class="pi pi-inbox text-4xl mb-2 block"></i>
              لا توجد طلبات
            </td>
          </tr>
        </ng-template>
      </p-table>
    </div>
  `,
  styles: [`
    :host { display: block; }
  `]
})
export class LeaveRequestListComponent {
  @Input() set requests(data: LeaveRequest[]) {
    this.allRequests.set(data);
    this.filterRequests();
  }
  @Output() cancelRequest = new EventEmitter<number>();

  allRequests = signal<LeaveRequest[]>([]);
  filteredRequests = signal<LeaveRequest[]>([]);
  selectedStatus: string | null = null;

  statusOptions = [
    { label: 'جميع الحالات', value: null },
    { label: 'معلق', value: 'PENDING' },
    { label: 'مقبول', value: 'APPROVED' },
    { label: 'مرفوض', value: 'REJECTED' },
    { label: 'ملغي', value: 'CANCELLED' }
  ];

  onStatusChange() {
    this.filterRequests();
  }

  filterRequests() {
    const requests = this.allRequests();
    if (this.selectedStatus) {
      this.filteredRequests.set(requests.filter(r => r.status === this.selectedStatus));
    } else {
      this.filteredRequests.set(requests);
    }
  }

  getStatusLabel(status: string): string {
    const labels: Record<string, string> = {
      'PENDING': 'معلق',
      'APPROVED': 'مقبول',
      'REJECTED': 'مرفوض',
      'CANCELLED': 'ملغي'
    };
    return labels[status] || status;
  }

  getStatusSeverity(status: string): 'success' | 'danger' | 'warn' | 'info' {
    const severities: Record<string, any> = {
      'APPROVED': 'success',
      'REJECTED': 'danger',
      'PENDING': 'warn',
      'CANCELLED': 'info'
    };
    return severities[status] || 'info';
  }

  onCancel(requestId: number) {
    this.cancelRequest.emit(requestId);
  }
}
