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
    <div class="bg-white dark:bg-slate-900 rounded-xl border border-slate-200 dark:border-slate-800 shadow-sm overflow-hidden flex flex-col">
      <!-- Header with filter -->
      <div class="p-3 border-b border-slate-100 dark:border-slate-800 flex items-center justify-between bg-slate-50/50 dark:bg-slate-900/50">
        <h3 class="text-[11px] font-bold text-slate-800 dark:text-slate-100 flex items-center gap-2">
            <i class="pi pi-list text-emerald-500"></i>
            طلبات الإجازات
        </h3>
        <p-select
          [(ngModel)]="selectedStatus"
          [options]="statusOptions"
          optionLabel="label"
          optionValue="value"
          placeholder="جميع الحالات"
          styleClass="w-48 !h-8 !text-[10px] !rounded-md !bg-white dark:!bg-slate-800 !border-slate-200 dark:!border-slate-700 focus:!border-emerald-500 transition-all flex items-center"
          (onChange)="onStatusChange()">
        </p-select>
      </div>

      <!-- Table -->
      <p-table
        [value]="filteredRequests()"
        [paginator]="true"
        [rows]="10"
        styleClass="p-datatable-sm clean-table"
        [rowHover]="true"
        responsiveLayout="scroll">

        <ng-template pTemplate="header">
          <tr class="bg-slate-50 dark:bg-slate-800/50 border-b border-slate-100 dark:border-slate-800">
            <th class="!px-4 !py-2.5 !text-[9px] !font-bold !text-slate-500 dark:!text-slate-400 !uppercase tracking-wider !bg-transparent !border-none w-10 text-center">#</th>
            <th class="!px-4 !py-2.5 !text-[9px] !font-bold !text-slate-500 dark:!text-slate-400 !uppercase tracking-wider !bg-transparent !border-none">نوع الإجازة</th>
            <th class="!px-4 !py-2.5 !text-[9px] !font-bold !text-slate-500 dark:!text-slate-400 !uppercase tracking-wider !bg-transparent !border-none text-center">من تاريخ</th>
            <th class="!px-4 !py-2.5 !text-[9px] !font-bold !text-slate-500 dark:!text-slate-400 !uppercase tracking-wider !bg-transparent !border-none text-center">إلى تاريخ</th>
            <th class="!px-4 !py-2.5 !text-[9px] !font-bold !text-slate-500 dark:!text-slate-400 !uppercase tracking-wider !bg-transparent !border-none text-center">الأيام</th>
            <th class="!px-4 !py-2.5 !text-[9px] !font-bold !text-slate-500 dark:!text-slate-400 !uppercase tracking-wider !bg-transparent !border-none">السبب</th>
            <th class="!px-4 !py-2.5 !text-[9px] !font-bold !text-slate-500 dark:!text-slate-400 !uppercase tracking-wider !bg-transparent !border-none text-center">الحالة</th>
            <th class="!px-4 !py-2.5 !text-[9px] !font-bold !text-slate-500 dark:!text-slate-400 !uppercase tracking-wider !bg-transparent !border-none text-center">التاريخ</th>
            <th class="!px-4 !py-2.5 !text-[9px] !font-bold !text-slate-500 dark:!text-slate-400 !uppercase tracking-wider !bg-transparent !border-none text-center w-24">إجراءات</th>
          </tr>
        </ng-template>

        <ng-template pTemplate="body" let-request let-i="rowIndex">
          <tr class="hover:bg-slate-50/50 dark:hover:bg-slate-800/50 transition-colors duration-200 border-b border-slate-50 dark:border-slate-800/50">
            <td class="!px-4 !py-3 !text-[10px] !text-slate-500 !font-bold !border-none text-center">{{ i + 1 }}</td>
            <td class="!px-4 !py-3 !text-[10px] !text-slate-700 dark:!text-slate-300 !font-bold !border-none">{{ request.leaveTypeName }}</td>
            <td class="!px-4 !py-3 !border-none text-center">
                <span class="inline-flex items-center gap-1.5 px-2 py-0.5 rounded bg-slate-100 dark:bg-slate-800/50 text-slate-700 dark:text-slate-300 border border-slate-200 dark:border-slate-700 mx-auto w-fit">
                    <i class="pi pi-calendar text-[9px] text-slate-400"></i>
                    <span class="font-mono font-bold text-[9px]">{{ request.startDate | date: 'yyyy-MM-dd' }}</span>
                </span>
            </td>
            <td class="!px-4 !py-3 !border-none text-center">
                <span class="inline-flex items-center gap-1.5 px-2 py-0.5 rounded bg-slate-100 dark:bg-slate-800/50 text-slate-700 dark:text-slate-300 border border-slate-200 dark:border-slate-700 mx-auto w-fit">
                    <i class="pi pi-calendar text-[9px] text-slate-400"></i>
                    <span class="font-mono font-bold text-[9px]">{{ request.endDate | date: 'yyyy-MM-dd' }}</span>
                </span>
            </td>
            <td class="!px-4 !py-3 !border-none text-center">
                <span class="px-2 py-0.5 rounded bg-sky-50 dark:bg-sky-900/20 text-sky-700 dark:text-sky-400 border border-sky-100 dark:border-sky-800/50 text-[10px] font-bold inline-block mx-auto min-w-[30px]">
                    {{ request.daysCount }}
                </span>
            </td>
            <td class="!px-4 !py-3 !text-[9px] !text-slate-500 dark:!text-slate-400 !border-none">
              <span [title]="request.reason" class="truncate max-w-[150px] inline-block">{{ request.reason | slice:0:30 }}{{ request.reason?.length > 30 ? '...' : '' }}</span>
            </td>
            <td class="!px-4 !py-3 !border-none text-center">
              <span [class]="'px-2 py-0.5 rounded text-[9px] font-bold block w-fit mx-auto ' + getSeverityClass(request.status)">
                 {{ getStatusLabel(request.status) }}
              </span>
            </td>
            <td class="!px-4 !py-3 !border-none text-center text-[9px] text-slate-500 font-mono">{{ request.createdAt | date: 'yyyy-MM-dd' }}</td>
            <td class="!px-4 !py-3 !border-none text-center">
              @if (request.status === 'PENDING' || request.status === 'MANAGER_APPROVED') {
                <button pButton icon="pi pi-times" 
                    class="p-button-text p-button-rounded !w-7 !h-7 !p-0 !text-[11px] !text-red-500 hover:!bg-red-100 dark:hover:!bg-red-900/40 transition-colors mx-auto block" 
                    title="إلغاء الطلب"
                    (click)="onCancel(request.requestId!)"></button>
              } @else {
                  <span class="text-slate-300 dark:text-slate-600">-</span>
              }
            </td>
          </tr>
        </ng-template>

        <ng-template pTemplate="emptymessage">
          <tr>
            <td colspan="9" class="text-center py-12">
                <div class="flex flex-col items-center justify-center text-slate-400 dark:text-slate-500">
                    <i class="pi pi-inbox text-3xl mb-3 opacity-50"></i>
                    <p class="text-[11px] font-bold text-slate-900 dark:text-white mb-1">لا توجد طلبات</p>
                    <p class="text-[9px]">لم يتم العثور على أي طلبات إجازة.</p>
                </div>
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
    { label: 'موافقة مدير', value: 'MANAGER_APPROVED' },
    { label: 'موافقة HR', value: 'HR_APPROVED' },
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
      'MANAGER_APPROVED': 'موافقة مدير',
      'HR_APPROVED': 'موافقة HR',
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
      'MANAGER_APPROVED': 'info',
      'HR_APPROVED': 'info',
      'CANCELLED': 'info'
    };
    return severities[status] || 'info';
  }

  getSeverityClass(status: string): string {
    const severities: Record<string, string> = {
      'APPROVED': 'bg-emerald-50 dark:bg-emerald-900/30 text-emerald-600 dark:text-emerald-400 border border-emerald-200 dark:border-emerald-800',
      'REJECTED': 'bg-red-50 dark:bg-red-900/30 text-red-600 dark:text-red-400 border border-red-200 dark:border-red-800',
      'PENDING': 'bg-amber-50 dark:bg-amber-900/30 text-amber-600 dark:text-amber-400 border border-amber-200 dark:border-amber-800',
      'MANAGER_APPROVED': 'bg-sky-50 dark:bg-sky-900/30 text-sky-600 dark:text-sky-400 border border-sky-200 dark:border-sky-800',
      'HR_APPROVED': 'bg-blue-50 dark:bg-blue-900/30 text-blue-600 dark:text-blue-400 border border-blue-200 dark:border-blue-800',
      'CANCELLED': 'bg-slate-100 dark:bg-slate-800 text-slate-500 border border-slate-200 dark:border-slate-700'
    };
    return severities[status] || 'bg-slate-100 text-slate-500';
  }

  onCancel(requestId: number) {
    this.cancelRequest.emit(requestId);
  }
}
