import { Component, OnInit, signal, viewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MessageService } from 'primeng/api';
import { ToastModule } from 'primeng/toast';
import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { LeaveRequestFormComponent } from '../../components/leave-request-form/leave-request-form.component';
import { LeaveBalanceCardsComponent } from '../../components/leave-balance-cards/leave-balance-cards.component';
import { LeaveRequestListComponent } from '../../components/leave-request-list/leave-request-list.component';
import { LeaveRequestService } from '../../services/leave-request.service';
import { LeaveBalanceService } from '../../services/leave-balance.service';
import { LeaveRequest, LeaveBalance } from '../../models/leave.models';
import { AuthService } from '../../../../core/auth/services/auth.service';

@Component({
  selector: 'app-my-leaves',
  standalone: true,
  imports: [
    CommonModule, 
    ToastModule,
    ButtonModule,
    DialogModule,
    LeaveRequestFormComponent, 
    LeaveBalanceCardsComponent,
    LeaveRequestListComponent
  ],
  providers: [MessageService],
  template: `
    <div class="bg-white dark:bg-slate-900 rounded-xl p-4 border border-slate-200 dark:border-slate-800 shadow-sm flex flex-col h-full gap-4 animate-in fade-in slide-in-from-bottom-5 duration-700" dir="rtl">
      <p-toast></p-toast>
      
      <!-- Page Header -->
      <div class="flex flex-col md:flex-row justify-between items-center pb-3 border-b border-slate-100 dark:border-slate-800 gap-4 md:gap-0">
        <div class="flex items-center gap-3">
          <div class="w-7 h-7 rounded-md bg-emerald-50 dark:bg-emerald-900/20 text-emerald-600 dark:text-emerald-400 flex items-center justify-center">
            <i class="pi pi-calendar-plus text-[11px]"></i>
          </div>
          <div>
            <h3 class="text-xs font-bold text-slate-800 dark:text-slate-100 leading-tight">طلبات إجازاتي</h3>
            <p class="text-[9px] text-slate-500 dark:text-slate-400 font-medium mt-0.5">إدارة طلبات الإجازة ومتابعة الأرصدة المتاحة للعام الحالي</p>
          </div>
        </div>
        <button pButton icon="pi pi-plus" label="تقديم طلب جديد"
            class="p-button-outlined p-button-secondary !h-8 !text-[10px] !font-bold !rounded-md !px-3 hover:!bg-slate-50 dark:hover:!bg-slate-800 !border-slate-200 dark:!border-slate-700 !text-slate-700 dark:!text-slate-300 transition-all whitespace-nowrap"
            (click)="showRequestDialog.set(true)">
        </button>
      </div>

      <div class="flex-grow flex flex-col gap-4 overflow-y-auto pr-1 custom-scrollbar">
        <!-- Balance Section -->
        <div class="space-y-3">
          <h2 class="text-[11px] font-bold text-slate-700 dark:text-slate-300 flex items-center gap-2">
            <i class="pi pi-wallet text-blue-500"></i>
            أرصدة الإجازات المتاحة
          </h2>
          <app-leave-balance-cards [balanceData]="balances()"></app-leave-balance-cards>
        </div>

        <!-- Requests List -->
        <div class="space-y-3 mt-2">
          <h2 class="text-[11px] font-bold text-slate-700 dark:text-slate-300 flex items-center gap-2">
            <i class="pi pi-history text-emerald-500"></i>
            سجل الطلبات السابقة
          </h2>
          <app-leave-request-list 
            [requests]="requests()"
            (cancelRequest)="handleCancelRequest($event)">
          </app-leave-request-list>
        </div>
      </div>

      <!-- Request Modal -->
      <p-dialog 
        [(visible)]="showRequestDialog" 
        [modal]="true" 
        [dismissableMask]="true"
        [showHeader]="false"
        [style]="{ width: '500px' }"
        styleClass="clean-dialog !bg-transparent !border-none !shadow-none"
        contentStyleClass="!p-0 !overflow-visible">
        <app-leave-request-form 
          (submitted)="onFormSubmitted()" 
          (cancelled)="showRequestDialog.set(false)">
        </app-leave-request-form>
      </p-dialog>
    </div>
  `,
  styles: [`
    :host { display: block; height: 100%; }
    .custom-scrollbar::-webkit-scrollbar { width: 4px; }
    .custom-scrollbar::-webkit-scrollbar-track { background: transparent; }
    .custom-scrollbar::-webkit-scrollbar-thumb { background: #cbd5e1; border-radius: 4px; }
    .dark .custom-scrollbar::-webkit-scrollbar-thumb { background: #334155; }
  `]
})
export class MyLeavesComponent implements OnInit {
  balances = signal<LeaveBalance[]>([]);
  requests = signal<LeaveRequest[]>([]);
  showRequestDialog = signal(false);
  loading = signal(false);

  constructor(
    private leaveRequestService: LeaveRequestService,
    private leaveBalanceService: LeaveBalanceService,
    private messageService: MessageService,
    private authService: AuthService
  ) {}

  ngOnInit() {
    this.loadData();
  }

  loadData() {
    const employeeId = this.authService.currentUser()?.employeeId;
    if (!employeeId) return;

    this.loading.set(true);
    
    this.leaveBalanceService.getEmployeeBalances(employeeId).subscribe({
      next: (res) => {
        if (res.succeeded) {
          this.balances.set(res.data);
        }
      }
    });

    this.leaveRequestService.getEmployeeRequests(employeeId).subscribe({
      next: (res) => {
        if (res.succeeded) {
          this.requests.set(res.data);
        }
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }

  onFormSubmitted() {
    this.showRequestDialog.set(false);
    this.loadData();
    this.messageService.add({ 
      severity: 'success', 
      summary: 'نجح', 
      detail: 'تم تقديم الطلب بنجاح' 
    });
  }

  handleCancelRequest(requestId: number) {
    if (confirm('هل أنت متأكد من إلغاء هذا الطلب؟')) {
      this.leaveRequestService.cancelRequest(requestId).subscribe({
        next: (res) => {
          if (res.succeeded) {
            this.messageService.add({ 
              severity: 'success', 
              summary: 'نجح', 
              detail: 'تم إلغاء الطلب بنجاح' 
            });
            this.loadData();
          }
        }
      });
    }
  }
}
