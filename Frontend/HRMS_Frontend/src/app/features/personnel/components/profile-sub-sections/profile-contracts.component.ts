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
import { EmployeeService } from '../../services/employee.service';
import { MessageService, ConfirmationService } from 'primeng/api';

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
    SelectModule
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
  contractForm: FormGroup;
  selectedContractId: number | null = null;

  private employeeService = inject(EmployeeService);
  private messageService = inject(MessageService);
  private confirmationService = inject(ConfirmationService);
  private fb = inject(FormBuilder);

  contractTypes = [
    { label: 'عقد محدد المدة', value: 'Fixed-Term' },
    { label: 'عقد غير محدد المدة', value: 'Open-Ended' },
    { label: 'عقد جزئي', value: 'Part-Time' },
    { label: 'عقد مؤقت', value: 'Temporary' }
  ];

  constructor() {
    this.contractForm = this.fb.group({
      contractType: ['Fixed-Term', Validators.required],
      startDate: [null, Validators.required],
      endDate: [null, Validators.required],
      isRenewable: [true],
      basicSalary: [0, [Validators.required, Validators.min(0)]],
      housingAllowance: [0],
      transportAllowance: [0],
      otherAllowances: [0],
      vacationDays: [30],
      workingHoursDaily: [8],
      notes: ['']
    });
  }

  ngOnInit() {
    if (this.employeeId) {
      this.loadData();
    }
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
    this.contractForm.reset({ 
      contractType: 'Fixed-Term', 
      isRenewable: true,
      basicSalary: 0,
      housingAllowance: 0,
      transportAllowance: 0,
      otherAllowances: 0,
      vacationDays: 30,
      workingHoursDaily: 8
    });
    this.displayDialog = true;
  }

  showRenewDialog(contract: any) {
    this.isRenewMode = true;
    this.selectedContractId = contract.contractId;
    
    // Pre-fill with current contract values for easier renewal
    this.contractForm.patchValue({
        basicSalary: contract.basicSalary,
        housingAllowance: contract.housingAllowance,
        transportAllowance: contract.transportAllowance,
        otherAllowances: contract.otherAllowances,
        startDate: null, // Reset dates for new contract
        endDate: null
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
            NewOtherAllowances: formVal.otherAllowances,
            Notes: formVal.notes // Or some remarks field
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
        // Create Logic
        const data = {
            EmployeeId: this.employeeId,
            ContractType: formVal.contractType,
            StartDate: formVal.startDate ? new Date(formVal.startDate).toISOString() : null,
            EndDate: formVal.endDate ? new Date(formVal.endDate).toISOString() : null,
            IsRenewable: formVal.isRenewable ? 1 : 0,
            BasicSalary: formVal.basicSalary,
            HousingAllowance: formVal.housingAllowance,
            TransportAllowance: formVal.transportAllowance,
            OtherAllowances: formVal.otherAllowances,
            VacationDays: formVal.vacationDays,
            WorkingHoursDaily: formVal.workingHoursDaily
        };

        this.employeeService.addContract(this.employeeId, data).subscribe({
            next: () => {
              this.messageService.add({ severity: 'success', summary: 'نجاح', detail: 'تم إنشاء العقد بنجاح' });
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
}
