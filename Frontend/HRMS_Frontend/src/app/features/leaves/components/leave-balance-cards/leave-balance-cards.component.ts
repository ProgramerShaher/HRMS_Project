import { Component, Input, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CardModule } from 'primeng/card';
import { ButtonModule } from 'primeng/button';
import { ProgressBarModule } from 'primeng/progressbar';
import { LeaveBalance } from '../../models/leave.models';

@Component({
  selector: 'app-leave-balance-cards',
  standalone: true,
  imports: [CommonModule, CardModule, ButtonModule, ProgressBarModule],
  template: `
    <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-3">
      @for (balance of balances(); track balance.balanceId) {
        <div class="bg-white dark:bg-slate-900 rounded-xl shadow-sm border border-slate-200 dark:border-slate-800 p-4 hover:border-emerald-300 dark:hover:border-emerald-700/50 transition-colors group relative overflow-hidden" dir="rtl">
          <!-- Background decoration -->
          <div class="absolute -left-6 -top-6 w-24 h-24 bg-emerald-500/5 dark:bg-emerald-500/10 rounded-full blur-2xl group-hover:bg-emerald-500/10 dark:group-hover:bg-emerald-500/20 transition-colors"></div>
          
          <div class="flex items-start justify-between mb-3 relative z-10">
            <h3 class="text-xs font-bold text-slate-800 dark:text-slate-200">{{ balance.leaveTypeName }}</h3>
            <div class="w-7 h-7 rounded-lg bg-emerald-50 dark:bg-emerald-900/30 flex items-center justify-center text-emerald-600 dark:text-emerald-400">
                <i class="pi pi-calendar text-[11px]"></i>
            </div>
          </div>
          
          <div class="mb-4 relative z-10">
            <div class="flex items-baseline gap-1 mb-1.5">
              <span class="text-2xl font-black text-slate-900 dark:text-white font-mono">{{ balance.currentBalance }}</span>
              <span class="text-[9px] font-bold text-slate-500 dark:text-slate-400">يوم متاح</span>
            </div>
            <p-progressBar 
              [value]="getPercentage(balance)" 
              [showValue]="false"
              styleClass="!h-1.5 !rounded-full !bg-slate-100 dark:!bg-slate-800"
              [style]="{'--p-progressbar-value-background': '#10b981'}">
            </p-progressBar>
          </div>
          
          <button pButton
            label="طلب إجازة" 
            icon="pi pi-plus"
            class="p-button-outlined p-button-secondary !w-full !h-8 !text-[10px] !font-bold !rounded-md hover:!bg-emerald-50 dark:hover:!bg-emerald-900/20 hover:!text-emerald-600 dark:hover:!text-emerald-400 hover:!border-emerald-200 dark:hover:!border-emerald-800/50 transition-all relative z-10"
            (click)="onRequestLeave(balance.leaveTypeId)">
          </button>
        </div>
      }
    </div>
  `,
  styles: [`
    :host { display: block; }
  `]
})
export class LeaveBalanceCardsComponent {
  @Input() set balanceData(data: LeaveBalance[]) {
    this.balances.set(data);
  }
  
  balances = signal<LeaveBalance[]>([]);

  getPercentage(balance: LeaveBalance): number {
    return (balance.currentBalance / 21) * 100; // Assuming 21 is max
  }

  onRequestLeave(leaveTypeId: number) {
    // Emit event or navigate to request form
    console.log('Request leave for type:', leaveTypeId);
  }
}
