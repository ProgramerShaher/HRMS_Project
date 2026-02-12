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
    <div class="p-6 space-y-6">
      <p-toast></p-toast>
      
      <!-- Premium Header -->
      <div class="bg-gradient-to-br from-slate-900 to-slate-800 rounded-3xl p-8 shadow-2xl relative overflow-hidden border border-slate-700/50">
        <div class="absolute top-0 right-0 w-64 h-64 bg-emerald-500/10 rounded-full blur-3xl -translate-y-1/2 translate-x-1/2"></div>
        
        <div class="flex flex-col md:flex-row md:items-center justify-between gap-6 relative z-10">
          <div>
            <h1 class="text-4xl font-black text-white tracking-tight flex items-center gap-4">
              <div class="w-12 h-12 bg-emerald-500 rounded-2xl flex items-center justify-center shadow-lg shadow-emerald-500/30">
                <i class="pi pi-history text-2xl"></i>
              </div>
              سجل حركات الإجازات
            </h1>
            <p class="text-slate-400 mt-2 text-lg font-medium">مراقبة وتدقيق جميع التغييرات على أرصدة الموظفين</p>
          </div>
          
          <div class="flex items-center gap-3">
            <button pButton icon="pi pi-refresh" (click)="loadTransactions()" 
                    class="p-button-secondary p-button-text p-button-rounded text-white hover:bg-slate-700"></button>
            <div class="px-4 py-2 bg-emerald-500/10 border border-emerald-500/20 rounded-xl">
              <span class="text-emerald-400 font-bold">{{ transactions().length }} حركة مكتشفة</span>
            </div>
          </div>
        </div>
      </div>

      <!-- Filters Bar -->
      <div class="bg-white dark:bg-zinc-900 rounded-2xl p-6 shadow-sm border border-slate-100 dark:border-zinc-800 flex flex-wrap gap-4 items-end">
        
        <div class="flex flex-col gap-2" *ngIf="isAdmin()">
            <label class="text-xs font-bold text-slate-500 uppercase tracking-wider">رقم الموظف</label>
            <p-iconField iconPosition="left">
              <p-inputIcon styleClass="pi pi-user"></p-inputIcon>
              <input type="text" pInputText [(ngModel)]="filters.employeeId" placeholder="بحث بالرقم..." class="w-32" />
            </p-iconField>
        </div>

        <div class="flex flex-col gap-2">
            <label class="text-xs font-bold text-slate-500 uppercase tracking-wider">نوع الحركة</label>
            <p-select [options]="typeOptions" [(ngModel)]="filters.transactionType" 
                      placeholder="الكل" [showClear]="true" class="w-48"></p-select>
        </div>

        <div class="flex flex-col gap-2">
            <label class="text-xs font-bold text-slate-500 uppercase tracking-wider">الفترة من</label>
            <p-datepicker [(ngModel)]="filters.fromDate" [showIcon]="true" placeholder="من تاريخ" dateFormat="yy-mm-dd"></p-datepicker>
        </div>

        <div class="flex flex-col gap-2">
            <label class="text-xs font-bold text-slate-500 uppercase tracking-wider">إلى تاريخ</label>
            <p-datepicker [(ngModel)]="filters.toDate" [showIcon]="true" placeholder="إلى تاريخ" dateFormat="yy-mm-dd"></p-datepicker>
        </div>

        <p-button label="تطبيق الفلتر" icon="pi pi-filter" styleClass="p-button-raised p-button-primary" (onClick)="loadTransactions()"></p-button>
        <p-button label="إعادة تعيين" icon="pi pi-filter-slash" [outlined]="true" (onClick)="resetFilters()"></p-button>
      </div>

      <!-- Main Table -->
      <div class="bg-white dark:bg-zinc-900 rounded-2xl shadow-xl overflow-hidden border border-slate-100 dark:border-zinc-800">
        <p-table 
          [value]="transactions()" 
          [paginator]="true" 
          [rows]="10"
          [loading]="loading()"
          [rowsPerPageOptions]="[10, 20, 50]"
          styleClass="p-datatable-lg p-datatable-striped">
          
          <ng-template pTemplate="header">
            <tr class="bg-slate-50 dark:bg-zinc-800/50">
              <th class="p-4 text-slate-700 dark:text-zinc-300">الموظف</th>
              <th class="p-4 text-slate-700 dark:text-zinc-300">نوع الإجازة</th>
              <th class="p-4 text-slate-700 dark:text-zinc-300">نوع الحركة</th>
              <th class="p-4 text-slate-700 dark:text-zinc-300 text-center">الأيام</th>
              <th class="p-4 text-slate-700 dark:text-zinc-300">التاريخ</th>
              <th class="p-4 text-slate-700 dark:text-zinc-300">الملاحظات</th>
              <th class="p-4 text-slate-700 dark:text-zinc-300">المرجع</th>
            </tr>
          </ng-template>

          <ng-template pTemplate="body" let-tx>
            <tr class="hover:bg-slate-50/50 dark:hover:bg-zinc-800/30 transition-colors">
              <td class="p-4">
                <div class="flex items-center gap-3">
                   <div class="w-8 h-8 rounded-full bg-slate-100 dark:bg-zinc-800 flex items-center justify-center text-xs font-bold text-slate-600 dark:text-zinc-400">
                      {{ tx.employeeName?.charAt(0) }}
                   </div>
                   <span class="font-medium text-slate-900 dark:text-white">{{ tx.employeeName }}</span>
                </div>
              </td>
              <td class="p-4 text-slate-600 dark:text-zinc-400">{{ tx.leaveTypeName }}</td>
              <td class="p-4">
                <p-tag 
                  [value]="getTypeLabel(tx.transactionType)" 
                  [severity]="getTypeSeverity(tx.transactionType)"
                  class="rounded-lg px-2">
                </p-tag>
              </td>
              <td class="p-4 text-center">
                <span [class]="getDaysClass(tx.transactionType)" class="font-black text-lg">
                  {{ getSignedDays(tx) }}
                </span>
              </td>
              <td class="p-4">
                 <div class="flex flex-col">
                    <span class="text-sm font-semibold text-slate-900 dark:text-white">{{ tx.transactionDate | date: 'dd/MM/yyyy' }}</span>
                    <span class="text-[10px] text-slate-400 uppercase tracking-tighter">{{ tx.transactionDate | date: 'HH:mm' }}</span>
                 </div>
              </td>
              <td class="p-4">
                <span class="text-sm text-slate-500 dark:text-zinc-500 italic max-w-xs truncate block" [title]="tx.notes">
                  {{ tx.notes || 'لايوجد ملاحظات' }}
                </span>
              </td>
              <td class="p-4">
                <span class="px-2 py-1 bg-slate-100 dark:bg-zinc-800 rounded text-xs font-mono text-slate-500">
                   #{{ tx.referenceId || '-' }}
                </span>
              </td>
            </tr>
          </ng-template>

          <ng-template pTemplate="emptymessage">
            <tr>
              <td colspan="7" class="text-center py-20 text-slate-400">
                <i class="pi pi-search text-6xl mb-4 block text-slate-200"></i>
                <span class="text-lg">لا توجد حركات مطابقة للبحث</span>
              </td>
            </tr>
          </ng-template>
        </p-table>
      </div>
    </div>
  `,
  styles: [`
    :host { display: block; background: #f8fafc; min-height: 100vh; }
    .dark :host { background: #09090b; }
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
  ) {}

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
