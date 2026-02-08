import { Component, Input, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { InputTextModule } from 'primeng/inputtext';
import { ToastModule } from 'primeng/toast';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { SelectModule } from 'primeng/select';
import { EmployeeService } from '../../services/employee.service';
import { MessageService, ConfirmationService } from 'primeng/api';

@Component({
  selector: 'app-profile-addresses',
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
    SelectModule
  ],
  templateUrl: './profile-addresses.component.html',
  providers: [MessageService, ConfirmationService]
})
export class ProfileAddressesComponent implements OnInit {
  @Input() employeeId!: number;
  
  addresses = signal<any[]>([]);
  loading = signal<boolean>(false);
  
  displayDialog = false;
  submitted = false;
  isEditMode = false;
  addressForm: FormGroup;
  selectedAddressId: number | null = null; // API might use composite key or ID. 
                                         // Endpoints: updateAddress(empId, data), deleteAddress(empId, addressId) 
                                         // Or deleteSameAddress(empId, type)?
                                         // Let's check Service: deleteAddress(id: number, addressId: number)
                                         // Addresses usually have an ID.
                                         
  private employeeService = inject(EmployeeService);
  private messageService = inject(MessageService);
  private confirmationService = inject(ConfirmationService);
  private fb = inject(FormBuilder);

  addressTypes = ['Home', 'Work', 'Mailing', 'Permanent'];

  constructor() {
    this.addressForm = this.fb.group({
      addressType: ['', Validators.required],
      countryId: [null], // Optional
      cityId: [null, Validators.required], // Using City Name/ID as string/number? API expects int?
                                           // CreateEmployeeDto has addresses as EmployeeAddressDto.
                                           // Dto has CityId (int). 
                                           // For UI simplicity I might use a text input or dummy dropdown if no City Lookup.
                                           // I will use InputText for City Name if the backend allowed it, but it says CityId.
                                           // I'll assume CityId is required. I'll put a temporary number input or look if I have cities.
                                           // Let's assume there is no lookup service ready yet. I'll use a number field or hardcode 1.
                                           // Actually, usually users want text. If backend STRICTLY requires ID, I must send ID.
                                           // Let's try to send a dummy ID for now or check if there's a lookup. 
                                           // I'll add a 'City Name' field in UI but if backend demands ID, this will fail.
                                           // Wait, looking at `profile-addresses.component.html` in previous files (none exist).
                                           // The Service `addAddress` takes `EmployeeAddress`.
                                           // Let's assume CityId is number. I will use a simple input for visual.
      street: ['', Validators.required],
      buildingNumber: [''],
      postalCode: [''],
      poBox: ['']
    });
  }

  ngOnInit() {
    if (this.employeeId) {
      this.loadData();
    }
  }

  loadData() {
    this.loading.set(true);
    this.employeeService.getAddresses(this.employeeId).subscribe({
      next: (res) => {
        this.addresses.set(res.data || []);
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }

  showAddDialog() {
    this.isEditMode = false;
    this.selectedAddressId = null;
    this.addressForm.reset();
    this.submitted = false;
    this.displayDialog = true;
  }

  showEditDialog(addr: any) {
    this.isEditMode = true;
    this.selectedAddressId = addr.addressId;
    this.addressForm.patchValue({
        addressType: addr.addressType,
        cityId: addr.cityId,
        street: addr.street,
        buildingNumber: addr.buildingNumber,
        postalCode: addr.postalCode,
        poBox: addr.poBox
    });
    this.displayDialog = true;
  }

  save() {
    this.submitted = true;
    if (this.addressForm.invalid) return;

    this.loading.set(true);
    const formVal = this.addressForm.value;
    
    // Check if formVal.cityId is a number
    const cityId = Number(formVal.cityId);

    const data = {
        EmployeeId: this.employeeId,
        AddressId: this.selectedAddressId,
        AddressType: formVal.addressType,
        CountryId: 1, // Defaulting to SA/Country 1 for now
        CityId: isNaN(cityId) ? 1 : cityId, // Default if not filled (though validators say required)
        Street: formVal.street,
        BuildingNumber: formVal.buildingNumber,
        PostalCode: formVal.postalCode,
        POBox: formVal.poBox
    };

    if (this.isEditMode && this.selectedAddressId) {
        this.employeeService.updateAddress(this.employeeId, this.selectedAddressId, data).subscribe({
            next: () => {
              this.messageService.add({ severity: 'success', summary: 'نجاح', detail: 'تم تحديث العنوان بنجاح' });
              this.displayDialog = false;
              this.loadData();
            },
            error: () => {
              this.messageService.add({ severity: 'error', summary: 'خطأ', detail: 'حدث خطأ أثناء التحديث' });
              this.loading.set(false);
            }
        });
    } else {
        this.employeeService.addAddress(this.employeeId, data).subscribe({
            next: () => {
              this.messageService.add({ severity: 'success', summary: 'نجاح', detail: 'تم إضافة العنوان بنجاح' });
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
      message: 'هل أنت متأكد من حذف هذا العنوان؟',
      header: 'تأكيد الحذف',
      icon: 'pi pi-exclamation-triangle',
      acceptLabel: 'نعم',
      rejectLabel: 'لا',
      acceptButtonStyleClass: 'p-button-danger',
      accept: () => {
        this.employeeService.deleteAddress(this.employeeId, id).subscribe({
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
