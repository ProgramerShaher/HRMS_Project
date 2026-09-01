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
    <div class="bg-white dark:bg-slate-900 rounded-xl p-4 border border-slate-200 dark:border-slate-800 shadow-sm flex flex-col h-full gap-4 animate-in fade-in slide-in-from-bottom-5 duration-700" dir="rtl">
      <p-toast></p-toast>
      
      <!-- Header -->
      <div class="flex flex-col md:flex-row justify-between items-center pb-3 border-b border-slate-100 dark:border-slate-800 gap-4 md:gap-0">
        <div class="flex items-center gap-3">
          <div class="w-7 h-7 rounded-md bg-purple-50 dark:bg-purple-900/20 text-purple-600 dark:text-purple-400 flex items-center justify-center">
            <i class="pi pi-users text-[11px]"></i>
          </div>
          <div>
            <h3 class="text-xs font-bold text-slate-800 dark:text-slate-100 leading-tight">طلبات إجازات الفريق</h3>
            <p class="text-[9px] text-slate-500 dark:text-slate-400 font-medium mt-0.5">الموافقة والرفض على طلبات الإجازات</p>
          </div>
        </div>
        <button pButton icon="pi pi-refresh" title="تحديث"
            class="p-button-outlined p-button-secondary !h-8 !w-8 !p-0 flex justify-center items-center !rounded-md hover:!bg-slate-50 dark:hover:!bg-slate-800 !border-slate-200 dark:!border-slate-700 !text-slate-700 dark:!text-slate-300 transition-all"
            (click)="loadPendingRequests()">
        </button>
      </div>

      <!-- Table -->
      <div class="flex-grow bg-white dark:bg-slate-900 rounded-lg border border-slate-100 dark:border-slate-800 overflow-hidden relative">
        <p-table 
          [value]="pendingRequests()" 
          [paginator]="true" 
          [rows]="10"
          [loading]="loading()"
          styleClass="p-datatable-sm clean-table"
          [rowHover]="true">
          
          <ng-template pTemplate="header">
            <tr class="bg-slate-50 dark:bg-slate-800/50 border-b border-slate-100 dark:border-slate-800">
              <th class="!px-4 !py-2.5 !text-[9px] !font-bold !text-slate-500 dark:!text-slate-400 !uppercase tracking-wider !bg-transparent !border-none">الموظف</th>
              <th class="!px-4 !py-2.5 !text-[9px] !font-bold !text-slate-500 dark:!text-slate-400 !uppercase tracking-wider !bg-transparent !border-none">نوع الإجازة</th>
              <th class="!px-4 !py-2.5 !text-[9px] !font-bold !text-slate-500 dark:!text-slate-400 !uppercase tracking-wider !bg-transparent !border-none text-center">التاريخ (من-إلى)</th>
              <th class="!px-4 !py-2.5 !text-[9px] !font-bold !text-slate-500 dark:!text-slate-400 !uppercase tracking-wider !bg-transparent !border-none text-center">الأيام</th>
              <th class="!px-4 !py-2.5 !text-[9px] !font-bold !text-slate-500 dark:!text-slate-400 !uppercase tracking-wider !bg-transparent !border-none">السبب</th>
              <th class="!px-4 !py-2.5 !text-[9px] !font-bold !text-slate-500 dark:!text-slate-400 !uppercase tracking-wider !bg-transparent !border-none text-center">الحالة</th>
              <th class="!px-4 !py-2.5 !text-[9px] !font-bold !text-slate-500 dark:!text-slate-400 !uppercase tracking-wider !bg-transparent !border-none text-center w-28">إجراءات</th>
            </tr>
          </ng-template>

          <ng-template pTemplate="body" let-request>
            <tr class="hover:bg-slate-50/50 dark:hover:bg-slate-800/50 transition-colors duration-200 border-b border-slate-50 dark:border-slate-800/50">
              <td class="!px-4 !py-2 !border-none">
                <div class="flex items-center gap-2">
                  <div class="w-7 h-7 rounded-lg bg-purple-50 dark:bg-purple-900/20 flex items-center justify-center text-[10px] font-black text-purple-600">
                    {{ request.employeeName?.charAt(0) ?? '?' }}
                  </div>
                  <div>
                    <p class="font-bold text-slate-800 dark:text-slate-200 text-[10px]">{{ request.employeeName ?? '—' }}</p>
                    <p class="text-[9px] text-slate-400 font-mono">#{{ request.employeeNumber ?? request.employeeId }}</p>
                  </div>
                </div>
              </td>
              <td class="!px-4 !py-2 !text-[9px] !text-slate-700 dark:!text-slate-300 !font-bold !border-none">{{ request.leaveTypeName }}</td>
              <td class="!px-4 !py-2 !border-none text-center">
                <div class="flex items-center justify-center gap-1">
                    <span class="inline-flex items-center gap-1 px-1.5 py-0.5 rounded bg-slate-100 dark:bg-slate-800/50 text-slate-700 dark:text-slate-300 border border-slate-200 dark:border-slate-700 w-fit">
                        <span class="font-mono font-bold text-[8px]">{{ request.startDate | date:'dd/MM/yyyy' }}</span>
                    </span>
                    <span class="text-[8px] text-slate-400">-</span>
                    <span class="inline-flex items-center gap-1 px-1.5 py-0.5 rounded bg-slate-100 dark:bg-slate-800/50 text-slate-700 dark:text-slate-300 border border-slate-200 dark:border-slate-700 w-fit">
                        <span class="font-mono font-bold text-[8px]">{{ request.endDate | date:'dd/MM/yyyy' }}</span>
                    </span>
                </div>
              </td>
              <td class="!px-4 !py-2 !border-none text-center">
                <span class="px-2 py-0.5 rounded bg-sky-50 dark:bg-sky-900/20 text-sky-700 dark:text-sky-400 border border-sky-100 dark:border-sky-800/50 text-[10px] font-bold inline-block mx-auto min-w-[30px]">
                  {{ request.daysCount }}
                </span>
              </td>
              <td class="!px-4 !py-2 !text-[9px] !text-slate-500 dark:!text-slate-400 !border-none">
                <span [title]="request.reason" class="truncate max-w-[150px] inline-block">{{ request.reason | slice:0:50 }}{{ request.reason?.length > 50 ? '...' : '' }}</span>
              </td>
              <td class="!px-4 !py-2 !border-none text-center">
                <span class="px-2 py-0.5 rounded text-[9px] font-bold block w-fit mx-auto bg-amber-50 dark:bg-amber-900/30 text-amber-600 dark:text-amber-400 border border-amber-200 dark:border-amber-800">
                    معلق
                </span>
              </td>
              <td class="!px-4 !py-2 !border-none text-center">
                <div class="flex items-center justify-center gap-1">
                  <button pButton icon="pi pi-check" 
                      class="p-button-text p-button-rounded !w-7 !h-7 !p-0 !text-[11px] !text-emerald-500 hover:!bg-emerald-100 dark:hover:!bg-emerald-900/40 transition-colors" 
                      title="موافقة"
                      (click)="approveRequest(request.requestId!)"></button>
                  <button pButton icon="pi pi-times" 
                      class="p-button-text p-button-rounded !w-7 !h-7 !p-0 !text-[11px] !text-red-500 hover:!bg-red-100 dark:hover:!bg-red-900/40 transition-colors" 
                      title="رفض"
                      (click)="rejectRequest(request.requestId!)"></button>
                </div>
              </td>
            </tr>
          </ng-template>

          <ng-template pTemplate="emptymessage">
            <tr>
              <td colspan="7" class="text-center py-12 text-slate-400">
                <div class="flex flex-col items-center justify-center">
                    <i class="pi pi-inbox text-3xl mb-3 opacity-50"></i>
                    <p class="text-[11px] font-bold text-slate-900 dark:text-white mb-1">لا توجد طلبات معلقة</p>
                </div>
              </td>
            </tr>
          </ng-template>
        </p-table>
      </div>
    </div>
  `,
  styles: [`:host { display: block; height: 100%; }`]
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
    console.log('🔄 Loading pending leave requests...');
    
    this.leaveRequestService.getPendingRequests().subscribe({
      next: (res) => {
        console.log('✅ Pending Requests API Response:', res);
        if (res.succeeded) {
          this.pendingRequests.set(res.data);
          console.log('📋 Pending requests loaded:', res.data);
        } else {
          console.error('❌ Pending Requests API failed:', res.message);
          this.messageService.add({ 
            severity: 'warn', 
            summary: 'تحذير', 
            detail: res.message || 'لا توجد طلبات معلقة' 
          });
        }
        this.loading.set(false);
      },
      error: (err) => {
        console.error('❌ Pending Requests API Error:', err);
        this.loading.set(false);
        this.messageService.add({ 
          severity: 'error', 
          summary: 'خطأ', 
          detail: err.error?.message || 'فشل تحميل الطلبات' 
        });
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
