import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MessageService } from 'primeng/api';
import { ToastModule } from 'primeng/toast';
import { TableModule } from 'primeng/table';
import { TagModule } from 'primeng/tag';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { InputNumberModule } from 'primeng/inputnumber';
import { LeaveBalanceService } from '../../services/leave-balance.service';
import { EmployeeLeaveTypeBalance } from '../../models/leave.models';

type EmployeeTotals = {
  entitlementDays: number;
  consumedDays: number;
  remainingDays: number;
};

@Component({
  selector: 'app-employee-balances',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ToastModule,
    TableModule,
    TagModule,
    ButtonModule,
    InputTextModule,
    InputNumberModule
  ],
  providers: [MessageService],
  template: `
    <div class="p-6 space-y-6 animate-in fade-in duration-500">
      <p-toast></p-toast>

      <div class="glass-panel p-6 rounded-2xl flex flex-col md:flex-row md:items-center justify-between gap-4 border-none shadow-sm">
        <div>
          <h1 class="text-3xl font-extrabold text-slate-900 dark:text-white tracking-tight flex items-center gap-3">
            <i class="pi pi-users text-indigo-600"></i>
            أرصدة الإجازات للموظفين
          </h1>
          <p class="text-slate-500 dark:text-zinc-400 mt-1">
            عرض إجمالي الاستحقاق والمستهلك والمتبقي لكل موظف ولكل نوع إجازة.
          </p>
        </div>

        <div class="flex items-end gap-3 flex-wrap">
          <div class="flex flex-col gap-1.5">
            <label class="text-xs font-bold text-slate-500 uppercase tracking-wider">السنة</label>
            <p-inputNumber [(ngModel)]="year" [min]="2000" [max]="2100" [useGrouping]="false" styleClass="w-32 rounded-xl"></p-inputNumber>
          </div>

          <div class="flex flex-col gap-1.5">
            <label class="text-xs font-bold text-slate-500 uppercase tracking-wider">بحث موظف</label>
            <input pInputText [(ngModel)]="search" class="w-64 rounded-xl" placeholder="رقم الموظف أو الاسم..." />
          </div>

          <p-button
            label="تحديث"
            icon="pi pi-refresh"
            styleClass="p-button-raised p-button-primary rounded-xl px-6"
            (onClick)="load()"
            [loading]="loading()">
          </p-button>
        </div>
      </div>

      <div class="glass-panel rounded-2xl border-none shadow-sm overflow-hidden bg-white dark:bg-zinc-900">
        <p-table
          [value]="rows()"
          [loading]="loading()"
          [rowGroupMode]="'subheader'"
          groupRowsBy="employeeId"
          sortField="employeeId"
          [sortOrder]="1"
          responsiveLayout="scroll"
          styleClass="p-datatable-lg custom-table">

          <ng-template pTemplate="header">
            <tr>
              <th class="bg-slate-50 dark:bg-zinc-800/50 text-slate-700 dark:text-zinc-300 font-bold py-4">نوع الإجازة</th>
              <th class="bg-slate-50 dark:bg-zinc-800/50 text-slate-700 dark:text-zinc-300 font-bold py-4 text-center">الإجمالي</th>
              <th class="bg-slate-50 dark:bg-zinc-800/50 text-slate-700 dark:text-zinc-300 font-bold py-4 text-center">المستهلك</th>
              <th class="bg-slate-50 dark:bg-zinc-800/50 text-slate-700 dark:text-zinc-300 font-bold py-4 text-center">المتبقي</th>
              <th class="bg-slate-50 dark:bg-zinc-800/50 text-slate-700 dark:text-zinc-300 font-bold py-4 text-center">السنة</th>
            </tr>
          </ng-template>

          <ng-template pTemplate="groupheader" let-rowData>
            <tr class="bg-slate-50/60 dark:bg-zinc-800/30">
              <td colspan="5" class="py-4">
                <div class="flex flex-col md:flex-row md:items-center justify-between gap-3">
                  <div class="flex items-center gap-3">
                    <div class="w-10 h-10 rounded-2xl bg-indigo-50 dark:bg-indigo-900/20 flex items-center justify-center text-indigo-700 dark:text-indigo-300 font-black">
                      {{ rowData.employeeNameAr?.charAt(0) }}
                    </div>
                    <div>
                      <div class="font-black text-slate-900 dark:text-white">
                        {{ rowData.employeeNameAr }}
                        <span class="text-slate-400 font-semibold mr-2">#{{ rowData.employeeNumber }}</span>
                      </div>
                      <div class="text-xs text-slate-500 dark:text-zinc-400">{{ rowData.departmentNameAr }}</div>
                    </div>
                  </div>

                  <div class="flex items-center gap-2 flex-wrap">
                    <span class="px-3 py-1 rounded-full text-xs font-bold bg-slate-100 dark:bg-zinc-800 text-slate-700 dark:text-zinc-200">
                      إجمالي: {{ totalsFor(rowData.employeeId).entitlementDays }}
                    </span>
                    <span class="px-3 py-1 rounded-full text-xs font-bold bg-amber-50 dark:bg-amber-900/20 text-amber-700 dark:text-amber-300">
                      مستهلك: {{ totalsFor(rowData.employeeId).consumedDays }}
                    </span>
                    <span class="px-3 py-1 rounded-full text-xs font-bold bg-emerald-50 dark:bg-emerald-900/20 text-emerald-700 dark:text-emerald-300">
                      متبقي: {{ totalsFor(rowData.employeeId).remainingDays }}
                    </span>
                  </div>
                </div>
              </td>
            </tr>
          </ng-template>

          <ng-template pTemplate="body" let-r>
            <tr class="hover:bg-slate-50/50 dark:hover:bg-zinc-800/30 transition-colors">
              <td class="py-4">
                <span class="font-bold text-slate-900 dark:text-white">{{ r.leaveTypeNameAr }}</span>
              </td>
              <td class="py-4 text-center font-extrabold text-slate-900 dark:text-white">{{ r.entitlementDays }}</td>
              <td class="py-4 text-center font-extrabold text-amber-700 dark:text-amber-300">{{ r.consumedDays }}</td>
              <td class="py-4 text-center font-extrabold text-emerald-700 dark:text-emerald-300">{{ r.remainingDays }}</td>
              <td class="py-4 text-center text-slate-500 dark:text-zinc-400 font-mono">{{ r.year }}</td>
            </tr>
          </ng-template>

          <ng-template pTemplate="emptymessage">
            <tr>
              <td colspan="5" class="text-center py-20 text-slate-400">
                <i class="pi pi-inbox text-6xl opacity-20 mb-4 block"></i>
                <div class="text-xl font-bold">لا توجد بيانات</div>
                <p class="text-sm mt-1">تأكد من تهيئة الأرصدة للسنة المختارة أو عدّل الفلاتر.</p>
              </td>
            </tr>
          </ng-template>
        </p-table>
      </div>
    </div>
  `,
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
export class EmployeeBalancesComponent implements OnInit {
  rows = signal<EmployeeLeaveTypeBalance[]>([]);
  loading = signal(false);

  year = new Date().getFullYear();
  search = '';

  private totals = signal<Map<number, EmployeeTotals>>(new Map());

  constructor(
    private leaveBalanceService: LeaveBalanceService,
    private messageService: MessageService
  ) {}

  ngOnInit() {
    this.load();
  }

  totalsFor(employeeId: number): EmployeeTotals {
    return this.totals().get(employeeId) || { entitlementDays: 0, consumedDays: 0, remainingDays: 0 };
  }

  load() {
    this.loading.set(true);
    this.leaveBalanceService.getEmployeesBalances({
      year: this.year,
      search: this.search?.trim() || undefined
    }).subscribe({
      next: (res) => {
        if (!res.succeeded) {
          this.rows.set([]);
          this.totals.set(new Map());
          this.loading.set(false);
          this.messageService.add({ severity: 'warn', summary: 'تنبيه', detail: res.message || 'لم يتم استرجاع البيانات' });
          return;
        }

        const data = (res.data || []).slice();
        data.sort((a, b) => a.employeeId - b.employeeId);
        this.rows.set(data);
        this.totals.set(this.computeTotals(data));
        this.loading.set(false);
      },
      error: (err) => {
        this.loading.set(false);
        this.messageService.add({ severity: 'error', summary: 'خطأ', detail: err?.error?.message || 'فشل تحميل أرصدة الموظفين' });
      }
    });
  }

  private computeTotals(data: EmployeeLeaveTypeBalance[]): Map<number, EmployeeTotals> {
    const map = new Map<number, EmployeeTotals>();
    for (const r of data) {
      const prev = map.get(r.employeeId) || { entitlementDays: 0, consumedDays: 0, remainingDays: 0 };
      map.set(r.employeeId, {
        entitlementDays: prev.entitlementDays + (r.entitlementDays || 0),
        consumedDays: prev.consumedDays + (r.consumedDays || 0),
        remainingDays: prev.remainingDays + (r.remainingDays || 0)
      });
    }
    return map;
  }
}
