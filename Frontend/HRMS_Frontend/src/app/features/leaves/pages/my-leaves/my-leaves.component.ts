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
    <div class="p-6 space-y-6 animate-in fade-in duration-500">
      <p-toast></p-toast>
      
      <!-- Page Header -->
      <div class="glass-panel p-6 rounded-2xl flex flex-col md:flex-row md:items-center justify-between gap-4 border-none shadow-sm">
        <div>
          <h1 class="text-3xl font-extrabold text-slate-900 dark:text-white tracking-tight flex items-center gap-3">
            <i class="pi pi-calendar-plus text-blue-600"></i>
            طلبات إجازاتي
          </h1>
          <p class="text-slate-500 dark:text-zinc-400 mt-1">إدارة طلبات الإجازة ومتابعة الأرصدة المتاحة للعام الحالي.</p>
        </div>
        <p-button 
            label="تقديم طلب جديد" 
            icon="pi pi-plus" 
            styleClass="p-button-raised p-button-primary rounded-xl px-6"
            (onClick)="showRequestDialog.set(true)">
        </p-button>
      </div>

      <!-- Balance Section -->
      <div class="space-y-4">
        <h2 class="text-xl font-bold text-slate-800 dark:text-white flex items-center gap-2">
          <span class="w-1.5 h-6 bg-blue-600 rounded-full"></span>
          أرصدة الإجازات المتاحة
        </h2>
        <app-leave-balance-cards [balanceData]="balances()"></app-leave-balance-cards>
      </div>

      <!-- Requests List -->
      <div class="space-y-4">
        <h2 class="text-xl font-bold text-slate-800 dark:text-white flex items-center gap-2">
          <span class="w-1.5 h-6 bg-emerald-600 rounded-full"></span>
          سجل الطلبات السابقة
        </h2>
        <app-leave-request-list 
          [requests]="requests()"
          (cancelRequest)="handleCancelRequest($event)">
        </app-leave-request-list>
      </div>

      <!-- Request Modal -->
      <p-dialog 
        [(visible)]="showRequestDialog" 
        [modal]="true" 
        [header]="'تقديم طلب إجازة جديد'" 
        [style]="{ width: '500px' }"
        [draggable]="false"
        [resizable]="false"
        styleClass="custom-dialog">
        <app-leave-request-form 
          (submitted)="onFormSubmitted()" 
          (cancelled)="showRequestDialog.set(false)">
        </app-leave-request-form>
      </p-dialog>
    </div>
  `,
  styles: [`
    :host { display: block; }
    ::ng-deep .custom-dialog .p-dialog-header {
      @apply bg-slate-50 dark:bg-zinc-900 border-b border-slate-100 dark:border-zinc-800 rounded-t-2xl;
    }
    ::ng-deep .custom-dialog .p-dialog-content {
      @apply bg-white dark:bg-zinc-900 p-0 rounded-b-2xl;
    }
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
