import { Component, OnInit, signal, computed, Output, EventEmitter } from '@angular/core';
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
import { AuthService } from '../../../../core/auth/services/auth.service';
import { EmployeeService } from '../../../personnel/services/employee.service';
import { Employee } from '../../../personnel/models/employee.model';

@Component({
  selector: 'app-leave-request-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, DatePicker, SelectModule, TextareaModule, ButtonModule],
  templateUrl: './leave-request-form.component.html',
  styleUrls: ['./leave-request-form.component.scss']
})
export class LeaveRequestFormComponent implements OnInit {
  @Output() submitted = new EventEmitter<void>();
  @Output() cancelled = new EventEmitter<void>();

  isAdmin = computed(() => {
    const roles = this.authService.currentUser()?.roles || [];
    return roles.includes('Admin') || roles.includes('System_Admin') || roles.includes('HR_Manager') || roles.includes('HR') || roles.includes('Manager');
  });

  leaveForm!: FormGroup;
  leaveTypes = signal<LeaveType[]>([]);
  employees = signal<Employee[]>([]);
  balances = signal<LeaveBalance[]>([]);
  selectedBalance = signal<number | null>(null);
  calculatedDays = signal<number>(0);
  loading = signal(false);

  constructor(
    private fb: FormBuilder,
    private leaveRequestService: LeaveRequestService,
    private leaveBalanceService: LeaveBalanceService,
    private leaveConfigService: LeaveConfigurationService,
    private employeeService: EmployeeService,
    private messageService: MessageService,
    private authService: AuthService
  ) { }

  ngOnInit() {
    this.initForm();
    this.loadData();
  }

  initForm() {
    this.leaveForm = this.fb.group({
      employeeId: [this.authService.currentUser()?.employeeId, Validators.required],
      leaveTypeId: [null, Validators.required],
      startDate: [null, Validators.required],
      endDate: [null, Validators.required],
      reason: ['', [Validators.required, Validators.minLength(10)]]
    });

    this.leaveForm.get('startDate')?.valueChanges.subscribe(() => this.calculateDays());
    this.leaveForm.get('endDate')?.valueChanges.subscribe(() => this.calculateDays());
    this.leaveForm.get('leaveTypeId')?.valueChanges.subscribe((id) => this.updateSelectedBalance(id));
    this.leaveForm.get('employeeId')?.valueChanges.subscribe((id) => {
      if (id) this.loadBalances(id);
    });
  }

  loadData() {
    const currentUser = this.authService.currentUser();

    this.loading.set(true);

    // Load Leave Types - Independent of employeeId
    this.leaveConfigService.getLeaveTypes().subscribe({
      next: (res) => {
        let list = Array.isArray(res) ? res : (res?.data || []);
        // Normalize names to handle both leaveNameAr and leaveTypeNameAr
        list = list.map((item: any) => ({
          ...item,
          leaveNameAr: item.leaveNameAr || item.leaveTypeNameAr || item.NameAr || item.nameAr,
          leaveNameEn: item.leaveNameEn || item.leaveTypeNameEn || item.NameEn || item.nameEn
        }));
        this.leaveTypes.set(list);
      },
      error: () => this.messageService.add({ severity: 'error', summary: 'خطأ', detail: 'فشل تحميل أنواع الإجازات' })
    });

    // Load Employees if Admin
    if (this.isAdmin()) {
      this.employeeService.getAll(1, 1000).subscribe({
        next: (res) => {
          const list = res.items || (res.data?.items) || (Array.isArray(res.data) ? res.data : []);
          this.employees.set(list);
          this.loading.set(false);
        },
        error: () => this.loading.set(false)
      });
    }

    // Load Balances for current user if they have an employeeId
    if (currentUser?.employeeId) {
      this.loadBalances(currentUser.employeeId);
    } else if (!this.isAdmin()) {
      this.loading.set(false);
      this.messageService.add({
        severity: 'error',
        summary: 'خطأ في الحساب',
        detail: 'حسابك غير مرتبط بملف موظف. لا يمكنك تقديم طلبات إجازة.'
      });
    } else {
      this.loading.set(false);
    }
  }

  loadBalances(employeeId: number) {
    this.loading.set(true);
    this.leaveBalanceService.getEmployeeBalances(employeeId).subscribe({
      next: (res) => {
        if (res.succeeded) {
          this.balances.set(res.data);
          // Refresh balance if leave type is already selected
          const currentType = this.leaveForm.get('leaveTypeId')?.value;
          if (currentType) this.updateSelectedBalance(currentType);
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
    const employeeId = Number(formValue.employeeId);

    if (!employeeId) {
      this.messageService.add({ severity: 'error', summary: 'خطأ', detail: 'لم يتم العثور على معرف الموظف' });
      return;
    }


    // Helper to format date without timezone shift
    const formatToLocalDate = (date: Date) => {
      const year = date.getFullYear();
      const month = String(date.getMonth() + 1).padStart(2, '0');
      const day = String(date.getDate()).padStart(2, '0');
      return `${year}-${month}-${day}`;
    };

    const request = {
      employeeId: Number(formValue.employeeId),
      leaveTypeId: Number(formValue.leaveTypeId),
      startDate: formatToLocalDate(formValue.startDate),
      endDate: formatToLocalDate(formValue.endDate),
      reason: formValue.reason.trim()
    };

    this.loading.set(true);
    this.leaveRequestService.createRequest(request).subscribe({
      next: (res) => {
        this.loading.set(false);
        if (res.succeeded) {
          this.leaveForm.reset();
          this.submitted.emit();
        } else {
          this.messageService.add({
            severity: 'error',
            summary: 'خطأ',
            detail: res.message || 'فشل تقديم الطلب'
          });
        }
      },
      error: (err) => {
        this.loading.set(false);
        this.messageService.add({
          severity: 'error',
          summary: 'خطأ',
          detail: err.error?.message || 'فشل تقديم الطلب - تأكد من الرصيد والبيانات'
        });
      }
    });
  }

  cancel() {
    this.leaveForm.reset();
    this.cancelled.emit();
  }
}
