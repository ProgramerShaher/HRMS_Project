import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { DatePicker } from 'primeng/datepicker';
import { SelectModule } from 'primeng/select';
import { TextareaModule } from 'primeng/textarea';
import { ButtonModule } from 'primeng/button';
import { MessageService } from 'primeng/api';
import { LeaveRequestService } from '../../services/leave-request.service';
import { LeaveBalanceService } from '../../services/leave-balance.service';
import { LeaveConfigurationService } from '../../services/leave-configuration.service';
import { LeaveType, LeaveBalance } from '../../models/leave.models';

@Component({
  selector: 'app-leave-request-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, DatePicker, SelectModule, TextareaModule, ButtonModule],
  templateUrl: './leave-request-form.component.html',
  styleUrls: ['./leave-request-form.component.scss']
})
export class LeaveRequestFormComponent implements OnInit {
  leaveForm!: FormGroup;
  leaveTypes = signal<LeaveType[]>([]);
  balances = signal<LeaveBalance[]>([]);
  selectedBalance = signal<number | null>(null);
  calculatedDays = signal<number>(0);
  loading = signal(false);
  employeeId = 1; // TODO: Get from auth service

  constructor(
    private fb: FormBuilder,
    private leaveRequestService: LeaveRequestService,
    private leaveBalanceService: LeaveBalanceService,
    private leaveConfigService: LeaveConfigurationService,
    private messageService: MessageService
  ) {}

  ngOnInit() {
    this.initForm();
    this.loadData();
  }

  initForm() {
    this.leaveForm = this.fb.group({
      leaveTypeId: [null, Validators.required],
      startDate: [null, Validators.required],
      endDate: [null, Validators.required],
      reason: ['', [Validators.required, Validators.minLength(10)]]
    });

    this.leaveForm.get('startDate')?.valueChanges.subscribe(() => this.calculateDays());
    this.leaveForm.get('endDate')?.valueChanges.subscribe(() => this.calculateDays());
    this.leaveForm.get('leaveTypeId')?.valueChanges.subscribe((id) => this.updateSelectedBalance(id));
  }

  loadData() {
    this.loading.set(true);
    
    this.leaveConfigService.getLeaveTypes().subscribe({
      next: (res) => {
        if (res.succeeded) {
          this.leaveTypes.set(res.data);
        }
      },
      error: () => this.messageService.add({ severity: 'error', summary: 'خطأ', detail: 'فشل تحميل أنواع الإجازات' })
    });

    this.leaveBalanceService.getEmployeeBalances(this.employeeId).subscribe({
      next: (res) => {
        if (res.succeeded) {
          this.balances.set(res.data);
        }
        this.loading.set(false);
      },
      error: () => {
        this.loading.set(false);
        this.messageService.add({ severity: 'error', summary: 'خطأ', detail: 'فشل تحميل الأرصدة' });
      }
    });
  }

  calculateDays() {
    const start = this.leaveForm.get('startDate')?.value;
    const end = this.leaveForm.get('endDate')?.value;
    
    if (start && end) {
      const diffTime = Math.abs(end.getTime() - start.getTime());
      const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24)) + 1;
      this.calculatedDays.set(diffDays);
    }
  }

  updateSelectedBalance(leaveTypeId: number) {
    const balance = this.balances().find(b => b.leaveTypeId === leaveTypeId);
    this.selectedBalance.set(balance?.currentBalance ?? null);
  }

  submit() {
    if (this.leaveForm.invalid) {
      this.messageService.add({ severity: 'warn', summary: 'تنبيه', detail: 'يرجى ملء جميع الحقول المطلوبة' });
      return;
    }

    const formValue = this.leaveForm.value;
    const request = {
      employeeId: this.employeeId,
      leaveTypeId: formValue.leaveTypeId,
      startDate: formValue.startDate.toISOString().split('T')[0],
      endDate: formValue.endDate.toISOString().split('T')[0],
      reason: formValue.reason
    };

    this.loading.set(true);
    this.leaveRequestService.createRequest(request).subscribe({
      next: (res) => {
        this.loading.set(false);
        if (res.succeeded) {
          this.messageService.add({ severity: 'success', summary: 'نجح', detail: 'تم تقديم الطلب بنجاح' });
          this.leaveForm.reset();
          this.loadData();
        }
      },
      error: () => {
        this.loading.set(false);
        this.messageService.add({ severity: 'error', summary: 'خطأ', detail: 'فشل تقديم الطلب' });
      }
    });
  }
}
