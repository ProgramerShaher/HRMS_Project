import { Component, OnInit, signal, inject, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MessageService } from 'primeng/api';
import { ToastModule } from 'primeng/toast';
import { TableModule } from 'primeng/table';
import { TagModule } from 'primeng/tag';
import { ButtonModule } from 'primeng/button';
import { DatePickerModule } from 'primeng/datepicker';
import { SelectModule } from 'primeng/select';
import { IconFieldModule } from 'primeng/iconfield';
import { InputIconModule } from 'primeng/inputicon';
import { InputTextModule } from 'primeng/inputtext';
import { LeaveTransactionService } from '../../services/leave-transaction.service';
import { LeaveTransaction } from '../../models/leave.models';
import { AuthService } from '../../../../core/auth/services/auth.service';

@Component({
  selector: 'app-transaction-history',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ToastModule,
    TableModule,
    TagModule,
    ButtonModule,
    DatePickerModule,
    SelectModule,
    IconFieldModule,
    InputIconModule,
    InputTextModule
  ],
  providers: [MessageService],
  template: `
    <div class="bg-white dark:bg-slate-900 rounded-xl p-4 border border-slate-200 dark:border-slate-800 shadow-sm flex flex-col h-full gap-4 animate-in fade-in slide-in-from-bottom-5 duration-700" dir="rtl">
      <p-toast></p-toast>
      
      <!-- Header -->
      <div class="flex flex-col md:flex-row justify-between items-center pb-3 border-b border-slate-100 dark:border-slate-800 gap-4 md:gap-0">
        <div class="flex items-center gap-3">
          <div class="w-7 h-7 rounded-md bg-emerald-50 dark:bg-emerald-900/20 text-emerald-600 dark:text-emerald-400 flex items-center justify-center">
            <i class="pi pi-history text-[11px]"></i>
          </div>
          <div>
            <h3 class="text-xs font-bold text-slate-800 dark:text-slate-100 leading-tight">سجل حركات الإجازات</h3>
            <p class="text-[9px] text-slate-500 dark:text-slate-400 font-medium mt-0.5">مراقبة وتدقيق جميع التغييرات على أرصدة الموظفين</p>
          </div>
        </div>
        
        <div class="flex items-center gap-2">
            <div class="px-2 py-1 bg-emerald-50 dark:bg-emerald-900/20 border border-emerald-100 dark:border-emerald-800/50 rounded-md flex items-center gap-1">
                <i class="pi pi-chart-line text-emerald-600 dark:text-emerald-400 text-[10px]"></i>
                <span class="text-emerald-700 dark:text-emerald-400 text-[10px] font-bold">{{ transactions().length }} حركة مكتشفة</span>
            </div>
            <button pButton icon="pi pi-refresh" title="تحديث"
                class="p-button-outlined p-button-secondary !h-8 !w-8 !p-0 flex justify-center items-center !rounded-md hover:!bg-slate-50 dark:hover:!bg-slate-800 !border-slate-200 dark:!border-slate-700 !text-slate-700 dark:!text-slate-300 transition-all"
                (click)="loadTransactions()">
            </button>
        </div>
      </div>

      <!-- Filters Bar -->
      <div class="flex flex-wrap gap-2 items-end bg-slate-50 dark:bg-slate-800/30 p-2.5 rounded-lg border border-slate-100 dark:border-slate-800">
        
        <div class="flex flex-col gap-1 w-full md:w-32" *ngIf="isAdmin()">
            <label class="text-[9px] font-bold text-slate-500">رقم الموظف</label>
            <div class="relative">
                <i class="pi pi-user absolute left-2 top-1/2 -translate-y-1/2 text-slate-400 text-[10px]"></i>
                <input type="text" pInputText [(ngModel)]="filters.employeeId" placeholder="بحث بالرقم..." 
                    class="w-full !h-7 !pl-6 !pr-2 !rounded-md !bg-white dark:!bg-slate-900 !border-slate-200 dark:!border-slate-700 !text-[10px] focus:!border-emerald-500 transition-all" />
            </div>
        </div>

        <div class="flex flex-col gap-1 w-full md:w-36">
            <label class="text-[9px] font-bold text-slate-500">نوع الحركة</label>
            <p-select [options]="typeOptions" [(ngModel)]="filters.transactionType" 
                      placeholder="الكل" [showClear]="true" 
                      styleClass="w-full !h-7 !rounded-md !bg-white dark:!bg-slate-900 !border-slate-200 dark:!border-slate-700 !text-[10px] focus:!border-emerald-500 transition-all flex items-center">
            </p-select>
        </div>

        <div class="flex flex-col gap-1 w-full md:w-32">
            <label class="text-[9px] font-bold text-slate-500">الفترة من</label>
            <p-datepicker [(ngModel)]="filters.fromDate" placeholder="من تاريخ" dateFormat="yy-mm-dd"
                styleClass="w-full" inputStyleClass="w-full !h-7 !px-2 !rounded-md !bg-white dark:!bg-slate-900 !border-slate-200 dark:!border-slate-700 !text-[10px] focus:!border-emerald-500 transition-all text-center">
            </p-datepicker>
        </div>

        <div class="flex flex-col gap-1 w-full md:w-32">
            <label class="text-[9px] font-bold text-slate-500">إلى تاريخ</label>
            <p-datepicker [(ngModel)]="filters.toDate" placeholder="إلى تاريخ" dateFormat="yy-mm-dd"
                styleClass="w-full" inputStyleClass="w-full !h-7 !px-2 !rounded-md !bg-white dark:!bg-slate-900 !border-slate-200 dark:!border-slate-700 !text-[10px] focus:!border-emerald-500 transition-all text-center">
            </p-datepicker>
        </div>

        <button pButton icon="pi pi-filter" label="تطبيق" 
            class="p-button-primary !h-7 !bg-emerald-600 hover:!bg-emerald-700 !border-none !text-[10px] !font-bold !rounded-md !px-3 shadow-sm transition-colors whitespace-nowrap"
            (click)="loadTransactions()">
        </button>
        <button pButton icon="pi pi-filter-slash" 
            class="p-button-outlined p-button-secondary !h-7 !w-7 !p-0 flex justify-center items-center !rounded-md hover:!bg-white dark:hover:!bg-slate-800 !border-slate-200 dark:!border-slate-700 !text-slate-700 dark:!text-slate-300 transition-all"
            title="إعادة تعيين"
            (click)="resetFilters()">
        </button>
      </div>

      <!-- Main Table -->
      <div class="flex-grow bg-white dark:bg-slate-900 rounded-lg border border-slate-100 dark:border-slate-800 overflow-hidden relative">
        <p-table 
          [value]="transactions()" 
          [paginator]="true" 
          [rows]="10"
          [loading]="loading()"
          [rowsPerPageOptions]="[10, 20, 50]"
          styleClass="p-datatable-sm clean-table"
          [rowHover]="true">
          
          <ng-template pTemplate="header">
            <tr class="bg-slate-50 dark:bg-slate-800/50 border-b border-slate-100 dark:border-slate-800">
              <th class="!px-4 !py-2.5 !text-[9px] !font-bold !text-slate-500 dark:!text-slate-400 !uppercase tracking-wider !bg-transparent !border-none">الموظف</th>
              <th class="!px-4 !py-2.5 !text-[9px] !font-bold !text-slate-500 dark:!text-slate-400 !uppercase tracking-wider !bg-transparent !border-none">نوع الإجازة</th>
              <th class="!px-4 !py-2.5 !text-[9px] !font-bold !text-slate-500 dark:!text-slate-400 !uppercase tracking-wider !bg-transparent !border-none">نوع الحركة</th>
              <th class="!px-4 !py-2.5 !text-[9px] !font-bold !text-slate-500 dark:!text-slate-400 !uppercase tracking-wider !bg-transparent !border-none text-center">الأيام</th>
              <th class="!px-4 !py-2.5 !text-[9px] !font-bold !text-slate-500 dark:!text-slate-400 !uppercase tracking-wider !bg-transparent !border-none">التاريخ</th>
              <th class="!px-4 !py-2.5 !text-[9px] !font-bold !text-slate-500 dark:!text-slate-400 !uppercase tracking-wider !bg-transparent !border-none">الملاحظات</th>
              <th class="!px-4 !py-2.5 !text-[9px] !font-bold !text-slate-500 dark:!text-slate-400 !uppercase tracking-wider !bg-transparent !border-none">المرجع</th>
            </tr>
          </ng-template>

          <ng-template pTemplate="body" let-tx>
            <tr class="hover:bg-slate-50/50 dark:hover:bg-slate-800/50 transition-colors duration-200 border-b border-slate-50 dark:border-slate-800/50">
              <td class="!px-4 !py-2 !border-none">
                <div class="flex items-center gap-2">
                   <div class="w-7 h-7 rounded-lg bg-emerald-50 dark:bg-emerald-900/20 flex items-center justify-center text-[10px] font-black text-emerald-600">
                      {{ tx.employeeName?.charAt(0) }}
                   </div>
                   <span class="font-bold text-slate-800 dark:text-slate-200 text-[10px]">{{ tx.employeeName }}</span>
                </div>
              </td>
              <td class="!px-4 !py-2 !text-[9px] !font-bold !text-slate-700 dark:!text-slate-300 !border-none">{{ tx.leaveTypeName }}</td>
              <td class="!px-4 !py-2 !border-none">
                <p-tag 
                  [value]="getTypeLabel(tx.transactionType)" 
                  [severity]="getTypeSeverity(tx.transactionType)"
                  styleClass="!text-[9px] !font-bold !px-2 !py-0.5 rounded">
                </p-tag>
              </td>
              <td class="!px-4 !py-2 !border-none text-center">
                <span [class]="getDaysClass(tx.transactionType)" class="font-black text-[11px] font-mono">
                  {{ getSignedDays(tx) }}
                </span>
              </td>
              <td class="!px-4 !py-2 !border-none">
                 <div class="flex flex-col">
                    <span class="text-[10px] font-bold text-slate-700 dark:text-slate-300 font-mono">{{ tx.transactionDate | date: 'dd/MM/yyyy' }}</span>
                    <span class="text-[8px] text-slate-400 font-mono">{{ tx.transactionDate | date: 'HH:mm' }}</span>
                 </div>
              </td>
              <td class="!px-4 !py-2 !border-none">
                <span class="text-[9px] text-slate-500 dark:text-slate-400 block truncate max-w-[150px]" [title]="tx.notes">
                  {{ tx.notes || '—' }}
                </span>
              </td>
              <td class="!px-4 !py-2 !border-none">
                <span class="px-1.5 py-0.5 bg-slate-100 dark:bg-slate-800/80 rounded border border-slate-200 dark:border-slate-700 text-[9px] font-mono text-slate-500 font-bold">
                   #{{ tx.referenceId || '-' }}
                </span>
              </td>
            </tr>
          </ng-template>

          <ng-template pTemplate="emptymessage">
            <tr>
              <td colspan="7" class="text-center py-12 text-slate-400">
                <div class="flex flex-col items-center justify-center">
                    <i class="pi pi-search text-3xl mb-3 opacity-50"></i>
                    <span class="text-[11px] font-bold text-slate-900 dark:text-white mb-1">لا توجد حركات مطابقة للبحث</span>
                </div>
              </td>
            </tr>
          </ng-template>
        </p-table>
      </div>
    </div>
  `,
  styles: [`
    :host { display: block; height: 100%; }
  `]
})
export class TransactionHistoryComponent implements OnInit {
  transactions = signal<LeaveTransaction[]>([]);
  loading = signal(false);

  filters = {
    employeeId: undefined as any,
    transactionType: undefined as string | undefined,
    fromDate: undefined as Date | undefined,
    toDate: undefined as Date | undefined,
    leaveTypeId: undefined as number | undefined
  };

  typeOptions = [
    { label: 'استحقاق رصيد', value: 'ACCRUAL' },
    { label: 'خصم إجازة', value: 'DEDUCTION' },
    { label: 'تعديل يدوي', value: 'ADJUSTMENT' },
    { label: 'إلغاء عملية', value: 'CANCELLATION' },
    { label: 'ترحيل أرصدة', value: 'CARRY_FORWARD' }
  ];

  isAdmin = computed(() => {
    const roles = this.authService.currentUser()?.roles || [];
    return roles.includes('System_Admin') || roles.includes('HR_Manager');
  });

  constructor(
    private transactionService: LeaveTransactionService,
    private messageService: MessageService,
    private authService: AuthService
  ) { }

  ngOnInit() {
    this.checkAccess();
    this.loadTransactions();
  }

  checkAccess() {
    if (!this.isAdmin()) {
      this.filters.employeeId = this.authService.currentUser()?.employeeId ?? undefined;
    }
  }

  loadTransactions() {
    this.loading.set(true);

    // Convert dates to ISO string if they exist
    const searchFilters = {
      ...this.filters,
      fromDate: this.filters.fromDate?.toISOString(),
      toDate: this.filters.toDate?.toISOString()
    };

    this.transactionService.getTransactionHistory(searchFilters).subscribe({
      next: (res) => {
        if (res.succeeded) {
          this.transactions.set(res.data);
        }
        this.loading.set(false);
      },
      error: () => {
        this.loading.set(false);
        this.messageService.add({ severity: 'error', summary: 'خطأ', detail: 'فشل تحميل سجل الحركات' });
      }
    });
  }

  resetFilters() {
    this.filters.transactionType = undefined;
    this.filters.fromDate = undefined;
    this.filters.toDate = undefined;
    this.checkAccess(); // Reset employeeId correctly
    this.loadTransactions();
  }

  getTypeLabel(type: string): string {
    const labels: Record<string, string> = {
      'ACCRUAL': 'استحقاق',
      'DEDUCTION': 'خصم',
      'ADJUSTMENT': 'تعديل',
      'CANCELLATION': 'إلغاء',
      'CARRY_FORWARD': 'ترحيل'
    };
    return labels[type] || type;
  }

  getTypeSeverity(type: string): 'success' | 'danger' | 'warn' | 'info' {
    const severities: Record<string, any> = {
      'ACCRUAL': 'success',
      'DEDUCTION': 'danger',
      'ADJUSTMENT': 'warn',
      'CANCELLATION': 'info',
      'CARRY_FORWARD': 'info'
    };
    return severities[type] || 'info';
  }

  getDaysClass(type: string): string {
    return type === 'DEDUCTION' || (type === 'ADJUSTMENT' && false) ? 'text-rose-600' : 'text-emerald-600';
  }

  getSignedDays(tx: LeaveTransaction): string {
    if (tx.days > 0) return `+${tx.days}`;
    return `${tx.days}`; // ALready has minus from backend if deduction
  }
}
