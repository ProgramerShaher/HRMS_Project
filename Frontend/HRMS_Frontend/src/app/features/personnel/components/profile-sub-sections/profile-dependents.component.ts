import { Component, Input, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { InputTextModule } from 'primeng/inputtext';
import { ToastModule } from 'primeng/toast';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { DatePickerModule } from 'primeng/datepicker';
import { EmployeeService } from '../../services/employee.service';
import { MessageService, ConfirmationService } from 'primeng/api';

@Component({
  selector: 'app-profile-dependents',
  standalone: true,
  imports: [
    CommonModule,
    TableModule,
    ButtonModule,
    DialogModule,
    ReactiveFormsModule,
    InputTextModule,
    ToastModule,
    ConfirmDialogModule,
    DatePickerModule
  ],
  templateUrl: './profile-dependents.component.html',
  providers: [MessageService, ConfirmationService]
})
export class ProfileDependentsComponent implements OnInit {
  @Input() employeeId!: number;
  
  dependents = signal<any[]>([]);
  loading = signal<boolean>(false);
  
  displayDialog = false;
  submitted = false;
  isEditMode = false;
  dependentForm: FormGroup;
  selectedDependentId: number | null = null;

  private employeeService = inject(EmployeeService);
  private messageService = inject(MessageService);
  private confirmationService = inject(ConfirmationService);
  private fb = inject(FormBuilder);

  constructor() {
    this.dependentForm = this.fb.group({
      fullNameAr: ['', Validators.required],
      relationship: ['', Validators.required],
      nationalId: ['', [Validators.required, Validators.pattern('^[0-9]+$')]],
      birthDate: [null, Validators.required],
      gender: ['M', Validators.required]
    });
  }

  ngOnInit() {
    if (this.employeeId) {
      this.loadData();
    }
  }

  loadData() {
    this.loading.set(true);
    this.employeeService.getDependents(this.employeeId).subscribe({
      next: (res) => {
        this.dependents.set(res.data || []);
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }

  showAddDialog() {
    this.isEditMode = false;
    this.selectedDependentId = null;
    this.dependentForm.reset({ gender: 'M' });
    this.submitted = false;
    this.displayDialog = true;
  }

  showEditDialog(dep: any) {
    this.isEditMode = true;
    this.selectedDependentId = dep.dependentId;
    this.dependentForm.patchValue({
        fullNameAr: dep.fullNameAr,
        relationship: dep.relationship,
        nationalId: dep.nationalId,
        birthDate: dep.birthDate ? new Date(dep.birthDate) : null,
        gender: dep.gender
    });
    this.displayDialog = true;
  }

  save() {
    this.submitted = true;
    if (this.dependentForm.invalid) return;

    this.loading.set(true);
    const formVal = this.dependentForm.value;
    const data = {
        EmployeeId: this.employeeId,
        DependentId: this.selectedDependentId, // For update
        FullNameAr: formVal.fullNameAr,
        Relationship: formVal.relationship,
        NationalId: formVal.nationalId,
        BirthDate: formVal.birthDate ? new Date(formVal.birthDate).toISOString() : null,
        Gender: formVal.gender
    };

    if (this.isEditMode && this.selectedDependentId) {
        this.employeeService.updateDependent(this.employeeId, this.selectedDependentId, data).subscribe({
            next: () => {
              this.messageService.add({ severity: 'success', summary: 'نجاح', detail: 'تم تحديث البيانات بنجاح' });
              this.displayDialog = false;
              this.loadData();
            },
            error: () => {
              this.messageService.add({ severity: 'error', summary: 'خطأ', detail: 'حدث خطأ أثناء التحديث' });
              this.loading.set(false);
            }
        });
    } else {
        this.employeeService.addDependent(this.employeeId, data).subscribe({
            next: () => {
              this.messageService.add({ severity: 'success', summary: 'نجاح', detail: 'تم إضافة المعال بنجاح' });
              this.displayDialog = false;
              this.loadData();
            },
            error: () => {
              this.messageService.add({ severity: 'error', summary: 'خطأ', detail: 'حدث خطأ أثناء الحفظ' });
              this.loading.set(false);
            }
        });
    }
  }

  delete(id: number) {
    this.confirmationService.confirm({
      message: 'هل أنت متأكد من حذف هذا السجل؟',
      header: 'تأكيد الحذف',
      icon: 'pi pi-exclamation-triangle',
      acceptLabel: 'نعم',
      rejectLabel: 'لا',
      acceptButtonStyleClass: 'p-button-danger',
      accept: () => {
        this.employeeService.deleteDependent(this.employeeId, id).subscribe({
          next: () => {
             this.messageService.add({ severity: 'success', summary: 'تم الحذف', detail: 'تم الحذف بنجاح' });
             this.loadData();
          },
          error: () => {
             this.messageService.add({ severity: 'error', summary: 'خطأ', detail: 'لا يمكن حذف العنصر' });
          }
        });
      }
    });
  }
}
