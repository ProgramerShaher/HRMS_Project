import { Component, EventEmitter, Input, Output, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { InputTextModule } from 'primeng/inputtext';
import { TextareaModule } from 'primeng/textarea';
import { SelectModule } from 'primeng/select';
import { InputNumberModule } from 'primeng/inputnumber';
import { CreateEmployeeDto } from '../../../models/create-employee.dto';
import { Address, BankAccount } from '../../../models/sub-models';
import { LookupService, Country, Bank, City } from '../../../../../core/services/lookup.service';
import { DynamicArrayListComponent } from '../../shared/dynamic-array-list/dynamic-array-list.component';

@Component({
  selector: 'app-addresses-step',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    InputTextModule,
    TextareaModule,
    SelectModule,
    InputNumberModule,
    DynamicArrayListComponent
  ],
  templateUrl: './addresses-step.component.html',
  styles: [`:host { display: block; }`]
})
export class AddressesStepComponent implements OnInit {
  @Input() data!: CreateEmployeeDto;
  @Output() dataChange = new EventEmitter<Partial<CreateEmployeeDto>>();

  private lookupService = inject(LookupService);
  countries: Country[] = [];
  banks = signal<Bank[]>([]);
  cities = signal<City[]>([]);

  addressColumns = [
    { field: 'countryName', header: 'البلد' },
    { field: 'street', header: 'الشارع' },
    { field: 'zipCode', header: 'الرمز البريدي' },
    { field: 'additionalDetails', header: 'تفاصيل إضافية' }
  ];

  bankAccountColumns = [
    { field: 'bankName', header: 'البنك' },
    { field: 'accountHolderName', header: 'اسم صاحب الحساب' },
    { field: 'ibanNumber', header: 'رقم IBAN' }
  ];

  addressTypes = [
    { label: 'المنزل', value: 'Home' },
    { label: 'العمل', value: 'Work' },
    { label: 'آخر', value: 'Other' }
  ];

  ngOnInit() {
    this.lookupService.getCountries().subscribe(c => this.countries = c);
    this.lookupService.getBanks().subscribe(b => this.banks.set(b));
  }

  onSaveAddress(event: { item: Address, index: number }) {
    const { item, index } = event;
    
    // Map country name for display
    const country = this.countries.find(c => c.countryId === item.countryId);
    if (country) {
      (item as any).countryName = country.countryNameAr;
    }

    // Map city name for display
    const city = this.cities().find(c => c.cityId === item.cityId);
    if (city) {
      (item as any).cityName = city.cityNameAr;
    }

    if (index > -1) {
      // Edit
      this.data.addresses[index] = item;
    } else {
      // Add
      if (!this.data.addresses) this.data.addresses = [];
      this.data.addresses.push(item);
    }
    this.dataChange.emit(this.data);
  }

  onDeleteAddress(event: { item: Address, index: number }) {
    this.data.addresses.splice(event.index, 1);
    this.dataChange.emit(this.data);
  }

  onCountryChange(countryId: number) {
    if (countryId) {
      this.lookupService.getCities(countryId).subscribe(c => this.cities.set(c));
    } else {
      this.cities.set([]);
    }
  }

  onSaveBankAccount(event: { item: BankAccount, index: number }) {
    const { item, index } = event;
    
    // Map bank name for display
    const bank = this.banks().find(b => b.bankId === item.bankId);
    if (bank) {
      (item as any).bankName = bank.bankNameAr;
    }

    if (!this.data.bankAccounts) this.data.bankAccounts = [];
    
    if (index > -1) {
      this.data.bankAccounts[index] = item;
    } else {
      this.data.bankAccounts.push(item);
    }
    this.dataChange.emit(this.data);
  }

  onDataChange() {
    this.dataChange.emit({ ...this.data });
  }

  onDeleteBankAccount(event: { item: BankAccount, index: number }) {
    this.data.bankAccounts.splice(event.index, 1);
    this.onDataChange();
  }
}
