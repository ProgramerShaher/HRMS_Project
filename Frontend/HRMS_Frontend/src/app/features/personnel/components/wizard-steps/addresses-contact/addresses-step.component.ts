import { Component, Input, Output, EventEmitter, signal, inject, OnInit, ChangeDetectorRef } from '@angular/core';
import { SetupService } from '../../../../setup/services/setup.service';
import { CommonModule } from '@angular/common';
import { CreateEmployeeDto } from '../../../models/create-employee.dto';
import { Address } from '../../../models/sub-models'; // Import Address interface
import { ProfileEmergencyContactsComponent } from '../../profile-sub-sections/profile-emergency-contacts.component'; // Reuse existing component logic if possible, or adapt
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { InputTextModule } from 'primeng/inputtext';
import { ButtonModule } from 'primeng/button';
import { SelectModule } from 'primeng/select';

@Component({
  selector: 'app-addresses-step',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    InputTextModule,
    ButtonModule,
    SelectModule
    // Note: ProfileEmergencyContactsComponent might expect an EmployeeID which we don't have yet in Create Wizard.
    // So we need a simplified version or the component needs to handle "local" data mode.
    // For now, I will implement a local version of Emergency Contacts management here fitting the DTO.
  ],
  templateUrl: './addresses-step.component.html',
  styles: []
})
export class AddressesStepComponent implements OnInit {
  @Input() data!: CreateEmployeeDto;
  @Output() dataChange = new EventEmitter<Partial<CreateEmployeeDto>>();
  @Output() prev = new EventEmitter<void>();
  @Output() next = new EventEmitter<void>();
  
  // Local state for UI
  showContactDialog = false;
  editingContactIndex: number | null = null;
  
  // New Contact Form Data
  newContact = {
      contactNameAr: '',
      relationship: '',
      phonePrimary: '',
      phoneSecondary: ''
  };

  // Address Data (National Address)
  // TODO: Map to DTO.addresses structure
  currentAddress = {
      buildingNo: '',
      street: '',
      district: '',
      cityId: null,
      postalCode: ''
  };

  private setupService = inject(SetupService); // Inject SetupService
  private cdr = inject(ChangeDetectorRef); // Inject ChangeDetectorRef

  cities: any[] = [];

  ngOnInit() {
      this.loadLookups();
  }

  loadLookups() {
      this.setupService.getAll('Cities').subscribe({
          next: (res: any) => {
               const items = res.data?.items || res.items || res.data || res || [];
               this.cities = items.map((c: any) => ({ label: c.cityNameAr, value: c.cityId }));
               this.cdr.markForCheck();
          },
          error: (err) => {
              console.error('Failed to load cities', err);
          }
      });
  }

  onChange() {
      if (!this.data.addresses) this.data.addresses = [];
      
      // Basic mapping to ensure we send explicit address data
      // We assume index 0 is the primary/national address for this wizard flow
      if (!this.currentAddress.cityId) return; // Wait for city selection

      const addressDto: Address = {
          addressId: 0,
          addressTypeId: 1, // Assuming 1 = National Address/Home
          countryId: 1, // Default to Yemen or fetch if needed
          cityId: this.currentAddress.cityId,
          district: this.currentAddress.district,
          street: this.currentAddress.street,
          buildingNo: this.currentAddress.buildingNo,
          postalCode: this.currentAddress.postalCode,
          isPrimary: true
      };

      if (this.data.addresses.length > 0) {
          this.data.addresses[0] = { ...this.data.addresses[0], ...addressDto };
      } else {
          this.data.addresses.push(addressDto);
      }

      this.dataChange.emit(this.data);
  }

  // Emergency Contact Management (Local Array)
  addContact() {
      // Validate
      if (!this.newContact.contactNameAr || !this.newContact.relationship || !this.newContact.phonePrimary) return;

      if (!this.data.emergencyContacts) this.data.emergencyContacts = [];
      
      const contactDto = {
          contactNameAr: this.newContact.contactNameAr,
          relationship: this.newContact.relationship,
          phonePrimary: this.newContact.phonePrimary,
          phoneSecondary: this.newContact.phoneSecondary,
          isPrimary: false,
          contactId: 0
      };

      if (this.editingContactIndex !== null) {
          const existing = this.data.emergencyContacts[this.editingContactIndex];
          this.data.emergencyContacts[this.editingContactIndex] = { ...existing, ...contactDto };
      } else {
          this.data.emergencyContacts.push(contactDto);
      }
      
      this.closeDialog();
  }

  removeContact(index: number) {
      this.data.emergencyContacts?.splice(index, 1);
  }

  openAddDialog() {
      this.editingContactIndex = null;
      this.newContact = { contactNameAr: '', relationship: '', phonePrimary: '', phoneSecondary: '' };
      this.showContactDialog = true;
  }

  openEditDialog(index: number) {
      if (!this.data.emergencyContacts) return;
      this.editingContactIndex = index;
      const c = this.data.emergencyContacts[index];
      this.newContact = { 
          contactNameAr: c.contactNameAr,
          relationship: c.relationship,
          phonePrimary: c.phonePrimary,
          phoneSecondary: c.phoneSecondary? c.phoneSecondary : '' 
      };
      this.showContactDialog = true;
  }

  closeDialog() {
      this.showContactDialog = false;
  }

  onPrev() {
    this.prev.emit();
  }

  onNext() {
    this.next.emit();
  }
}
