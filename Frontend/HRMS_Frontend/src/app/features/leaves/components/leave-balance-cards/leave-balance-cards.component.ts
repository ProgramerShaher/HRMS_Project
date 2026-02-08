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
    <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-3">
      @for (balance of balances(); track balance.balanceId) {
        <div class="bg-gradient-to-br from-white to-gray-50 rounded-lg shadow-sm border border-gray-200 p-4 hover:shadow-md transition-shadow">
          <div class="flex items-start justify-between mb-3">
            <h3 class="text-base font-semibold text-gray-800">{{ balance.leaveTypeName }}</h3>
            <i class="pi pi-calendar text-blue-500 text-xl"></i>
          </div>
          
          <div class="mb-3">
            <div class="flex items-baseline gap-1 mb-1">
              <span class="text-3xl font-bold text-blue-600">{{ balance.currentBalance }}</span>
              <span class="text-sm text-gray-500">يوم متاح</span>
            </div>
            <p-progressBar 
              [value]="getPercentage(balance)" 
              [showValue]="false"
              styleClass="h-2">
            </p-progressBar>
          </div>
          
          <p-button 
            label="طلب إجازة" 
            icon="pi pi-plus"
            size="small"
            styleClass="w-full"
            [outlined]="true"
            (onClick)="onRequestLeave(balance.leaveTypeId)">
          </p-button>
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
