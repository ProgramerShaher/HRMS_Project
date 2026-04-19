import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MessageService } from 'primeng/api';
import { ToastModule } from 'primeng/toast';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { InputNumberModule } from 'primeng/inputnumber';
import { DialogModule } from 'primeng/dialog';
import { TextareaModule } from 'primeng/textarea';
import { SelectModule } from 'primeng/select';
import { LeaveBalanceService } from '../../services/leave-balance.service';
import { LeaveConfigurationService } from '../../services/leave-configuration.service';
import { EmployeeLeaveTypeBalance, LeaveType } from '../../models/leave.models';

type EmpTotals = { entitlementDays: number; consumedDays: number; remainingDays: number };

@Component({
  selector: 'app-employee-balances',
  standalone: true,
  imports: [
    CommonModule, FormsModule, ReactiveFormsModule,
    ToastModule, TableModule, ButtonModule, InputTextModule,
    InputNumberModule, DialogModule, TextareaModule, SelectModule
  ],
  providers: [MessageService],
  templateUrl: './employee-balances.component.html',
  styles: [`:host { display: block; }`]
})
export class EmployeeBalancesComponent implements OnInit {
  private balanceSvc = inject(LeaveBalanceService);
  private cfgSvc     = inject(LeaveConfigurationService);
  private msgSvc     = inject(MessageService);
  private fb         = inject(FormBuilder);

  rows       = signal<EmployeeLeaveTypeBalance[]>([]);
  loading    = signal(false);
  leaveTypes = signal<LeaveType[]>([]);

  year   = new Date().getFullYear();
  search = '';

  private totalsMap = new Map<number, EmpTotals>();

  // ── Initialize dialog ──────────────────────────
  showInitDialog  = false;
  initSaving      = signal(false);
  initForm!: FormGroup;
  yearOptions     = Array.from({ length: 6 }, (_, i) => new Date().getFullYear() - i + 1);

  // ── Adjust dialog ──────────────────────────────
  showAdjustDialog    = false;
  adjustSaving        = signal(false);
  adjustForm!: FormGroup;
  selectedEmployee: EmployeeLeaveTypeBalance | null = null;

  ngOnInit() {
    this.buildForms();
    this.loadLeaveTypes();
    this.load();
  }

  buildForms() {
    this.initForm = this.fb.group({
      year:           [this.year, Validators.required],
      leaveTypeId:    [null],
      departmentId:   [null],
      customDays:     [null],
      enableProration:[false]
    });

    this.adjustForm = this.fb.group({
      leaveTypeId:    [null, Validators.required],
      adjustmentDays: [0, [Validators.required]],
      reason:         ['', Validators.required]
    });
  }

  loadLeaveTypes() {
    this.cfgSvc.getLeaveTypes().subscribe({
      next: (res) => { if (res.succeeded) this.leaveTypes.set(res.data); }
    });
  }

  load() {
    this.loading.set(true);
    this.balanceSvc.getEmployeesBalances({ year: this.year, search: this.search?.trim() || undefined }).subscribe({
      next: (res) => {
        if (!res.succeeded) {
          this.msgSvc.add({ severity: 'warn', summary: 'تنبيه', detail: res.message });
          this.rows.set([]);
          this.loading.set(false);
          return;
        }
        const data = [...(res.data || [])].sort((a, b) => a.employeeId - b.employeeId);
        this.rows.set(data);
        this.computeTotals(data);
        this.loading.set(false);
      },
      error: () => {
        this.msgSvc.add({ severity: 'error', summary: 'خطأ', detail: 'فشل تحميل الأرصدة' });
        this.loading.set(false);
      }
    });
  }

  totalsFor(empId: number): EmpTotals {
    return this.totalsMap.get(empId) ?? { entitlementDays: 0, consumedDays: 0, remainingDays: 0 };
  }

  private computeTotals(data: EmployeeLeaveTypeBalance[]) {
    this.totalsMap = new Map();
    for (const r of data) {
      const prev = this.totalsMap.get(r.employeeId) ?? { entitlementDays: 0, consumedDays: 0, remainingDays: 0 };
      this.totalsMap.set(r.employeeId, {
        entitlementDays: prev.entitlementDays + (r.entitlementDays || 0),
        consumedDays:    prev.consumedDays    + (r.consumedDays    || 0),
        remainingDays:   prev.remainingDays   + (r.remainingDays   || 0)
      });
    }
  }

  // ── Initialize ─────────────────────────────────
  openInitDialog() {
    this.initForm.patchValue({ year: this.year, leaveTypeId: null, departmentId: null, customDays: null, enableProration: false });
    this.showInitDialog = true;
  }

  saveInitialize() {
    if (this.initForm.invalid) return;
    this.initSaving.set(true);
    this.balanceSvc.initializeBalances(this.initForm.value).subscribe({
      next: (res) => {
        this.initSaving.set(false);
        if (res.succeeded) {
          this.msgSvc.add({ severity: 'success', summary: 'نجح', detail: 'تم تهيئة الأرصدة بنجاح' });
          this.showInitDialog = false;
          this.load();
        } else {
          this.msgSvc.add({ severity: 'error', summary: 'خطأ', detail: res.message });
        }
      },
      error: () => {
        this.initSaving.set(false);
        this.msgSvc.add({ severity: 'error', summary: 'خطأ', detail: 'فشلت تهيئة الأرصدة' });
      }
    });
  }

  // ── Adjust ─────────────────────────────────────
  openAdjustDialog(row: EmployeeLeaveTypeBalance) {
    this.selectedEmployee = row;
    this.adjustForm.reset({ leaveTypeId: row.leaveTypeId, adjustmentDays: 0, reason: '' });
    this.showAdjustDialog = true;
  }

  saveAdjust() {
    if (this.adjustForm.invalid || !this.selectedEmployee) return;
    this.adjustSaving.set(true);
    const cmd = {
      employeeId:     this.selectedEmployee.employeeId,
      leaveTypeId:    this.adjustForm.value.leaveTypeId,
      year:           this.year,
      adjustmentDays: this.adjustForm.value.adjustmentDays,
      reason:         this.adjustForm.value.reason
    };
    this.balanceSvc.adjustBalance(cmd).subscribe({
      next: (res) => {
        this.adjustSaving.set(false);
        if (res.succeeded) {
          this.msgSvc.add({ severity: 'success', summary: 'نجح', detail: 'تم تعديل الرصيد بنجاح' });
          this.showAdjustDialog = false;
          this.load();
        } else {
          this.msgSvc.add({ severity: 'error', summary: 'خطأ', detail: res.message });
        }
      },
      error: () => {
        this.adjustSaving.set(false);
        this.msgSvc.add({ severity: 'error', summary: 'خطأ', detail: 'فشل تعديل الرصيد' });
      }
    });
  }

  leaveTypeOptions = () =>
    this.leaveTypes().map(t => ({ label: t.leaveTypeNameAr, value: t.leaveTypeId }));
}
