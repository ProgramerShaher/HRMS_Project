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
import { SelectModule } from 'primeng/select';
import { CheckboxModule } from 'primeng/checkbox';
import { EmployeeService } from '../../services/employee.service';
import { MessageService, ConfirmationService } from 'primeng/api';
import { Tag } from "primeng/tag";

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
    DatePickerModule,
    SelectModule,
    CheckboxModule,
    Tag
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
  
  relationships = [
    { label: 'Son / Daughter', value: 'CHILD' },
    { label: 'Spouse', value: 'SPOUSE' },
    { label: 'Parent', value: 'PARENT' },
    { label: 'Other', value: 'OTHER' }
  ];

  getRelationshipLabel(val: string): string {
    return this.relationships.find(r => r.value === val)?.label || val;
  }

  constructor() {
    this.dependentForm = this.fb.group({
      fullNameAr: ['', Validators.required],
      relationship: ['', Validators.required],
      nationalId: ['', [Validators.required, Validators.pattern('^[0-9]+$')]],
      birthDate: [null, Validators.required],
      gender: ['M'], // Optional if not in HTML
      hasHealthInsurance: [false]
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
    console.log('Dependent Form Status:', this.dependentForm.status);
    console.log('Dependent Form Errors:', this.dependentForm.errors);
    console.log('Dependent Form Value:', this.dependentForm.value);
    
    if (this.dependentForm.invalid) {
        Object.keys(this.dependentForm.controls).forEach(key => {
            const controlErrors = this.dependentForm.get(key)?.errors;
            if (controlErrors) {
                console.log(`Key control: ${key}, Errors:`, controlErrors);
            }
        });
        return;
    }

    this.loading.set(true);
    const formVal = this.dependentForm.value;
    const data = { 
        ...formVal,
        NameAr: formVal.fullNameAr, // Backend expects NameAr
        Relation: formVal.relationship, // Backend expects Relation
        employeeId: this.employeeId,
        dependentId: this.selectedDependentId,
        hasMedicalInsurance: formVal.hasHealthInsurance,
        BirthDate: formVal.birthDate ? new Date(formVal.birthDate).toISOString() : null,
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
            error: (err) => {
              console.error('Save Error:', err);
              const errorMessage = err.error?.message || err.error?.detail || 'حدث خطأ أثناء الحفظ';
              this.messageService.add({ severity: 'error', summary: 'خطأ', detail: errorMessage });
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
