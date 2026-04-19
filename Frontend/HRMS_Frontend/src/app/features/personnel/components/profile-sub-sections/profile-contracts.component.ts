import { Component, Input, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { InputTextModule } from 'primeng/inputtext';
import { InputNumberModule } from 'primeng/inputnumber';
import { ToastModule } from 'primeng/toast';
import { DatePickerModule } from 'primeng/datepicker';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { CheckboxModule } from 'primeng/checkbox';
import { SelectModule } from 'primeng/select';
import { LookupService } from '../../../../core/services/lookup.service';
import { EmployeeService } from '../../services/employee.service';
import { MessageService, ConfirmationService } from 'primeng/api';
import { Tag } from "primeng/tag";

@Component({
  selector: 'app-profile-contracts',
  standalone: true,
  imports: [
    CommonModule,
    TableModule,
    ButtonModule,
    DialogModule,
    ReactiveFormsModule,
    InputTextModule,
    InputNumberModule,
    ToastModule,
    DatePickerModule,
    ConfirmDialogModule,
    CheckboxModule,
    SelectModule,
    Tag
],
  templateUrl: './profile-contracts.component.html',
  providers: [MessageService, ConfirmationService]
})
export class ProfileContractsComponent implements OnInit {
  @Input() employeeId!: number;
  
  contracts = signal<any[]>([]);
  loading = signal<boolean>(false);
  
  displayDialog = false;
  isRenewMode = false;
  isEditMode = false;
  contractForm: FormGroup;
  selectedContractId: number | null = null;

  private employeeService = inject(EmployeeService);
  private lookupService = inject(LookupService);
  private messageService = inject(MessageService);
  private confirmationService = inject(ConfirmationService);
  private fb = inject(FormBuilder);

  jobGrades = signal<any[]>([]);
  medicalStatuses = signal<any[]>([
    { statusNameAr: 'VIP Gold Protection', statusId: 1 },
    { statusNameAr: 'Standard Coverage', statusId: 2 },
    { statusNameAr: 'Executive Shield', statusId: 3 }
  ]);

  contractTypes = [
    { label: 'عقد محدد المدة', value: 'Fixed-Term' },
    { label: 'عقد غير محدد المدة', value: 'Open-Ended' },
    { label: 'عقد جزئي', value: 'Part-Time' },
    { label: 'عقد مؤقت', value: 'Temporary' }
  ];

  constructor() {
    this.contractForm = this.fb.group({
      contractType: ['Fixed-Term', Validators.required],
      jobGradeId: [null, Validators.required],
      medicalStatusId: [null, Validators.required],
      startDate: [null, Validators.required],
      endDate: [null, Validators.required],
      isRenewable: [true],
      basicSalary: [0, [Validators.required, Validators.min(0)]],
      housingAllowance: [0],
      transportAllowance: [0],
      medicalAllowance: [0],
      vacationDays: [30],
      workingHoursDaily: [8],
      isActive: [true],
      notes: ['']
    });
  }

  ngOnInit() {
    if (this.employeeId) {
      this.loadData();
      this.loadLookups();
    }
  }

  loadLookups() {
    this.lookupService.getJobGrades().subscribe(data => this.jobGrades.set(data));
  }

  loadData() {
    this.loading.set(true);
    this.employeeService.getContracts(this.employeeId).subscribe({
      next: (res) => {
        this.contracts.set(res.data || []);
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }

  showAddDialog() {
    this.isRenewMode = false;
    this.isEditMode = false;
    this.selectedContractId = null;
    this.contractForm.reset({ 
      contractType: 'Fixed-Term', 
      isRenewable: true,
      basicSalary: 0,
      housingAllowance: 0,
      transportAllowance: 0,
      medicalAllowance: 0,
      vacationDays: 30,
      workingHoursDaily: 8,
      isActive: true
    });
    this.displayDialog = true;
  }

  showEditDialog(contract: any) {
    this.isRenewMode = false;
    this.isEditMode = true;
    this.selectedContractId = contract.contractId;
    this.contractForm.patchValue({
        contractType: contract.contractType,
        jobGradeId: contract.jobGradeId,
        medicalStatusId: contract.medicalStatusId,
        startDate: contract.startDate ? new Date(contract.startDate) : null,
        endDate: contract.endDate ? new Date(contract.endDate) : null,
        isRenewable: contract.isRenewable,
        basicSalary: contract.basicSalary,
        housingAllowance: contract.housingAllowance,
        transportAllowance: contract.transportAllowance,
        medicalAllowance: contract.medicalAllowance,
        vacationDays: contract.vacationDays,
        workingHoursDaily: contract.workingHoursDaily,
        isActive: contract.isActive,
        notes: contract.notes
    });
    this.displayDialog = true;
  }

  showRenewDialog(contract: any) {
    this.isRenewMode = true;
    this.isEditMode = false;
    this.selectedContractId = contract.contractId;
    
    // Pre-fill with current contract values for easier renewal
    this.contractForm.patchValue({
        jobGradeId: contract.jobGradeId,
        medicalStatusId: contract.medicalStatusId,
        basicSalary: contract.basicSalary,
        housingAllowance: contract.housingAllowance,
        transportAllowance: contract.transportAllowance,
        medicalAllowance: contract.medicalAllowance,
        startDate: null, // Reset dates for new contract
        endDate: null,
        isActive: true
    });

    this.displayDialog = true;
  }

  save() {
    if (this.contractForm.invalid) return;

    this.loading.set(true);
    const formVal = this.contractForm.value;

    if (this.isRenewMode) {
        // Renewal Logic
        const data = {
            ContractId: this.selectedContractId,
            NewStartDate: formVal.startDate ? new Date(formVal.startDate).toISOString() : null,
            NewEndDate: formVal.endDate ? new Date(formVal.endDate).toISOString() : null,
            NewBasicSalary: formVal.basicSalary,
            NewHousingAllowance: formVal.housingAllowance,
            NewTransportAllowance: formVal.transportAllowance,
            NewMedicalAllowance: formVal.medicalAllowance,
            Notes: formVal.notes
        };

        this.employeeService.renewContract(this.employeeId, data).subscribe({
            next: () => {
              this.messageService.add({ severity: 'success', summary: 'نجاح', detail: 'تم تجديد العقد بنجاح' });
              this.displayDialog = false;
              this.loadData();
            },
            error: () => {
              this.messageService.add({ severity: 'error', summary: 'خطأ', detail: 'حدث خطأ أثناء التجديد' });
              this.loading.set(false);
            }
        });

    } else {
        // Create Logic - Edit is not supported by backend
            const data = {
            EmployeeId: this.employeeId,
            // JobGradeId and MedicalStatusId are not part of CreateContractDto
            ContractType: formVal.contractType,
            StartDate: formVal.startDate ? new Date(formVal.startDate).toISOString() : null,
            EndDate: formVal.endDate ? new Date(formVal.endDate).toISOString() : null,
            IsRenewable: !!formVal.isRenewable, // Send boolean
            BasicSalary: Number(formVal.basicSalary),
            HousingAllowance: Number(formVal.housingAllowance),
            TransportAllowance: Number(formVal.transportAllowance),
            OtherAllowances: Number(formVal.medicalAllowance), // Map medicalAllowance to OtherAllowances
            VacationDays: Number(formVal.vacationDays),
            WorkingHoursDaily: Number(formVal.workingHoursDaily)
            // IsActive is not part of CreateContractDto
        };

        this.employeeService.addContract(this.employeeId, data).subscribe({
            next: () => {
              this.messageService.add({ severity: 'success', summary: 'نجاح', detail: 'تم إنشاء العقد بنجاح' });
              this.displayDialog = false;
              this.loadData();
            },
            error: (err) => {
              console.error('Add Contract Error:', err);
              const errorMessage = err.error?.message || err.error?.detail || 'حدث خطأ أثناء الحفظ';
              this.messageService.add({ severity: 'error', summary: 'خطأ', detail: errorMessage });
              this.loading.set(false);
            }
        });
    }
  }

  /* Delete not supported by backend */
}
