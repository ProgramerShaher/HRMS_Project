import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, FormsModule } from '@angular/forms';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { InputTextModule } from 'primeng/inputtext';
import { InputNumberModule } from 'primeng/inputnumber';
import { ToggleButtonModule } from 'primeng/togglebutton';
import { ToggleSwitchModule } from 'primeng/toggleswitch';
import { TagModule } from 'primeng/tag';
import { MessageService } from 'primeng/api';
import { ToastModule } from 'primeng/toast';
import { LeaveConfigurationService } from '../../services/leave-configuration.service';
import { LeaveType } from '../../models/leave.models';

@Component({
  selector: 'app-leave-setup',
  standalone: true,
  imports: [
    CommonModule, ReactiveFormsModule, FormsModule, TableModule, ButtonModule, 
    DialogModule, InputTextModule, InputNumberModule, ToggleButtonModule, 
    ToggleSwitchModule, TagModule, ToastModule
  ],
  providers: [MessageService],
  templateUrl: './leave-setup.component.html',
  styles: [`
    :host { display: block; }
    .custom-table ::ng-deep .p-datatable-thead > tr > th {
      @apply bg-slate-50 dark:bg-zinc-800/50 text-slate-700 dark:text-zinc-300 font-bold border-none;
    }
  `]
})
export class LeaveSetupComponent implements OnInit {
  leaveTypes = signal<LeaveType[]>([]);
  loading = signal(false);
  saving = signal(false);
  
  showModal = false;
  editMode = false;
  typeForm!: FormGroup;
  selectedTypeId: number | null = null;

  constructor(
    private fb: FormBuilder,
    private leaveConfigService: LeaveConfigurationService,
    private messageService: MessageService
  ) {}

  ngOnInit() {
    this.initForm();
    this.loadLeaveTypes();
  }

  initForm() {
    this.typeForm = this.fb.group({
      leaveNameAr: ['', Validators.required],
      leaveNameEn: ['', Validators.required],
      defaultDays: [21, [Validators.required, Validators.min(0)]],
      isPaid: [true],
      requiresApproval: [true],
      allowCarryForward: [false],
      maxCarryForwardDays: [0],
      isActive: [true]
    });
  }

  loadLeaveTypes() {
    this.loading.set(true);
    this.leaveConfigService.getLeaveTypes().subscribe({
      next: (res) => {
        if (res.succeeded) {
          this.leaveTypes.set(res.data);
        }
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }

  showCreateModal() {
    this.editMode = false;
    this.selectedTypeId = null;
    this.typeForm.reset({
      defaultDays: 21,
      isPaid: true,
      requiresApproval: true,
      allowCarryForward: false,
      maxCarryForwardDays: 0,
      isActive: true
    });
    this.showModal = true;
  }

  edit(type: LeaveType) {
    this.editMode = true;
    this.selectedTypeId = type.leaveTypeId;
    this.typeForm.patchValue(type);
    this.showModal = true;
  }

  save() {
    if (this.typeForm.invalid) return;

    this.saving.set(true);
    const payload = this.typeForm.value;

    if (this.editMode && this.selectedTypeId) {
      this.leaveConfigService.updateLeaveType(this.selectedTypeId, payload).subscribe({
        next: (res) => {
          this.handleSuccess('تم التحديث بنجاح');
        },
        error: () => this.saving.set(false)
      });
    } else {
      this.leaveConfigService.createLeaveType(payload).subscribe({
        next: (res) => {
          this.handleSuccess('تمت الإضافة بنجاح');
        },
        error: () => this.saving.set(false)
      });
    }
  }

  private handleSuccess(message: string) {
    this.saving.set(false);
    this.showModal = false;
    this.messageService.add({ severity: 'success', summary: 'نجح', detail: message });
    this.loadLeaveTypes();
  }

  deleteType(id: number) {
    if (confirm('هل أنت متأكد من حذف هذا النوع؟ قد يؤثر ذلك على الطلبات الحالية.')) {
      this.leaveConfigService.deleteLeaveType(id).subscribe({
        next: (res) => {
          if (res.succeeded) {
            this.messageService.add({ severity: 'success', summary: 'نجح', detail: 'تم الحذف بنجاح' });
            this.loadLeaveTypes();
          }
        }
      });
    }
  }
}
