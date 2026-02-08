import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MessageService } from 'primeng/api';
import { ToastModule } from 'primeng/toast';
import { TableModule } from 'primeng/table';
import { TagModule } from 'primeng/tag';
import { ButtonModule } from 'primeng/button';
import { LeaveTransactionService } from '../../services/leave-transaction.service';
import { LeaveTransaction } from '../../models/leave.models';

@Component({
  selector: 'app-transaction-history',
  standalone: true,
  imports: [CommonModule, ToastModule, TableModule, TagModule, ButtonModule],
  providers: [MessageService],
  template: `
    <div class="p-4">
      <p-toast></p-toast>
      
      <div class="bg-gradient-to-r from-green-600 to-green-700 rounded-lg shadow-md p-4 text-white mb-4">
        <h1 class="text-2xl font-bold flex items-center gap-2">
          <i class="pi pi-history"></i>
          سجل حركات الإجازات
        </h1>
        <p class="text-sm text-green-100 mt-1">عرض جميع الحركات والتعديلات على أرصدة الإجازات</p>
      </div>

      <div class="bg-white rounded-lg shadow-sm">
        <p-table 
          [value]="transactions()" 
          [paginator]="true" 
          [rows]="15"
          [loading]="loading()"
          styleClass="p-datatable-sm">
          
          <ng-template pTemplate="header">
            <tr>
              <th style="width: 5%">#</th>
              <th style="width: 15%">الموظف</th>
              <th style="width: 15%">نوع الإجازة</th>
              <th style="width: 12%">نوع الحركة</th>
              <th style="width: 8%">الأيام</th>
              <th style="width: 25%">الملاحظات</th>
              <th style="width: 12%">التاريخ</th>
              <th style="width: 8%">المرجع</th>
            </tr>
          </ng-template>

          <ng-template pTemplate="body" let-tx let-i="rowIndex">
            <tr>
              <td>{{ i + 1 }}</td>
              <td>{{ tx.employeeName }}</td>
              <td>{{ tx.leaveTypeName }}</td>
              <td>
                <p-tag 
                  [value]="getTypeLabel(tx.transactionType)" 
                  [severity]="getTypeSeverity(tx.transactionType)">
                </p-tag>
              </td>
              <td>
                <span [class]="getDaysClass(tx.transactionType)" class="font-semibold">
                  {{ getSignedDays(tx) }}
                </span>
              </td>
              <td class="text-xs text-gray-600">{{ tx.notes || '-' }}</td>
              <td class="text-xs text-gray-500">{{ tx.transactionDate | date: 'yyyy-MM-dd HH:mm' }}</td>
              <td class="text-xs">{{ tx.referenceId || '-' }}</td>
            </tr>
          </ng-template>

          <ng-template pTemplate="emptymessage">
            <tr>
              <td colspan="8" class="text-center py-8 text-gray-500">
                <i class="pi pi-inbox text-4xl mb-2 block"></i>
                لا توجد حركات
              </td>
            </tr>
          </ng-template>
        </p-table>
      </div>
    </div>
  `,
  styles: [`:host { display: block; }`]
})
export class TransactionHistoryComponent implements OnInit {
  transactions = signal<LeaveTransaction[]>([]);
  loading = signal(false);
  employeeId = 1; // TODO: Get from auth

  constructor(
    private transactionService: LeaveTransactionService,
    private messageService: MessageService
  ) {}

  ngOnInit() {
    this.loadTransactions();
  }

  loadTransactions() {
    this.loading.set(true);
    this.transactionService.getTransactionHistory(this.employeeId).subscribe({
      next: (res) => {
        if (res.succeeded) {
          this.transactions.set(res.data);
        }
        this.loading.set(false);
      },
      error: () => {
        this.loading.set(false);
        this.messageService.add({ severity: 'error', summary: 'خطأ', detail: 'فشل تحميل السجل' });
      }
    });
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
    return type === 'DEDUCTION' ? 'text-red-600' : 'text-green-600';
  }

  getSignedDays(tx: LeaveTransaction): string {
    const sign = tx.transactionType === 'DEDUCTION' ? '-' : '+';
    return `${sign}${tx.days}`;
  }
}
