import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MessageService } from 'primeng/api';
import { ToastModule } from 'primeng/toast';
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
    LeaveRequestFormComponent, 
    LeaveBalanceCardsComponent,
    LeaveRequestListComponent
  ],
  providers: [MessageService],
  template: `
    <div class="p-4 space-y-4">
      <p-toast></p-toast>
      
      <!-- Page Header -->
      <div class="bg-gradient-to-r from-blue-600 to-blue-700 rounded-lg shadow-md p-4 text-white">
        <h1 class="text-2xl font-bold flex items-center gap-2">
          <i class="pi pi-calendar"></i>
          إجازاتي
        </h1>
        <p class="text-sm text-blue-100 mt-1">إدارة طلبات الإجازة والأرصدة المتاحة</p>
      </div>

      <!-- Balance Cards -->
      <div>
        <h2 class="text-lg font-semibold text-gray-800 mb-3">أرصدة الإجازات</h2>
        <app-leave-balance-cards [balanceData]="balances()"></app-leave-balance-cards>
      </div>

      <!-- Request Form -->
      <div>
        <app-leave-request-form></app-leave-request-form>
      </div>

      <!-- Requests List -->
      <div>
        <app-leave-request-list 
          [requests]="requests()"
          (cancelRequest)="handleCancelRequest($event)">
        </app-leave-request-list>
      </div>
    </div>
  `,
  styles: [`
    :host { display: block; }
  `]
})
export class MyLeavesComponent implements OnInit {
  balances = signal<LeaveBalance[]>([]);
  requests = signal<LeaveRequest[]>([]);
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
    // Get employee ID from logged-in user
    const currentUser = this.authService.currentUser();
    const employeeId = currentUser?.employeeId;

    if (!employeeId) {
      console.error('❌ No employee ID found for current user');
      this.messageService.add({ 
        severity: 'error', 
        summary: 'خطأ', 
        detail: 'لم يتم العثور على معرف الموظف. يرجى تسجيل الدخول مرة أخرى.' 
      });
      return;
    }

    this.loading.set(true);
    console.log('🔄 Loading leaves data for employee:', employeeId);

    this.leaveBalanceService.getEmployeeBalances(employeeId).subscribe({
      next: (res) => {
        console.log('✅ Balance API Response:', res);
        if (res.succeeded) {
          this.balances.set(res.data);
          console.log('📊 Balances loaded:', res.data);
        } else {
          console.error('❌ Balance API failed:', res.message);
          this.messageService.add({ 
            severity: 'warn', 
            summary: 'تحذير', 
            detail: res.message || 'لا توجد أرصدة متاحة' 
          });
        }
      },
      error: (err) => {
        console.error('❌ Balance API Error:', err);
        this.messageService.add({ 
          severity: 'error', 
          summary: 'خطأ', 
          detail: err.error?.message || 'فشل تحميل الأرصدة' 
        });
      }
    });

    this.leaveRequestService.getEmployeeRequests(employeeId).subscribe({
      next: (res) => {
        console.log('✅ Requests API Response:', res);
        if (res.succeeded) {
          this.requests.set(res.data);
          console.log('📋 Requests loaded:', res.data);
        } else {
          console.error('❌ Requests API failed:', res.message);
          this.messageService.add({ 
            severity: 'warn', 
            summary: 'تحذير', 
            detail: res.message || 'لا توجد طلبات' 
          });
        }
        this.loading.set(false);
      },
      error: (err) => {
        console.error('❌ Requests API Error:', err);
        this.loading.set(false);
        this.messageService.add({ 
          severity: 'error', 
          summary: 'خطأ', 
          detail: err.error?.message || 'فشل تحميل الطلبات' 
        });
      }
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
        },
        error: () => {
          this.messageService.add({ 
            severity: 'error', 
            summary: 'خطأ', 
            detail: 'فشل إلغاء الطلب' 
          });
        }
      });
    }
  }
}
