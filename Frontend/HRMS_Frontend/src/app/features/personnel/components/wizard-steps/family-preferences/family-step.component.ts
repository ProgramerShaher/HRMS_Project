import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { InputTextModule } from 'primeng/inputtext';
import { DatePickerModule } from 'primeng/datepicker';
import { SelectModule } from 'primeng/select';
import { RadioButtonModule } from 'primeng/radiobutton';
import { CreateEmployeeDto } from '../../../models/create-employee.dto';
import { DynamicArrayListComponent } from '../../shared/dynamic-array-list/dynamic-array-list.component';

@Component({
  selector: 'app-family-step',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    InputTextModule,
    DatePickerModule,
    SelectModule,
    RadioButtonModule,
    DynamicArrayListComponent
  ],
  templateUrl: './family-step.component.html',
  styles: [`:host { display: block; }`]
})
export class FamilyStepComponent {
  @Input() data!: CreateEmployeeDto;
  @Output() dataChange = new EventEmitter<Partial<CreateEmployeeDto>>();

  contactColumns = [
    { field: 'contactNameAr', header: 'الاسم' },
    { field: 'phonePrimary', header: 'رقم الجوال' },
    { field: 'relationship', header: 'صلة القرابة' },
    { field: 'isPrimary', header: 'أساسي' }
  ];

  dependentColumns = [
    { field: 'fullNameAr', header: 'الاسم الكامل' },
    { field: 'relationship', header: 'صلة القرابة' },
    { field: 'nationalId', header: 'رقم الهوية' }
  ];

  priorities = [
    { label: 'نعم', value: true },
    { label: 'لا', value: false }
  ];

  genders = [
    { label: 'ذكر', value: 'Male', icon: 'pi pi-mars' },
    { label: 'أنثى', value: 'Female', icon: 'pi pi-venus' }
  ];

  onSaveContact(event: { item: any, index: number }) {
    if (!this.data.emergencyContacts) this.data.emergencyContacts = [];
    if (event.index > -1) {
      this.data.emergencyContacts[event.index] = event.item;
    } else {
      this.data.emergencyContacts.push(event.item);
    }
    this.dataChange.emit(this.data);
  }

  onDeleteContact(event: { item: any, index: number }) {
    this.data.emergencyContacts?.splice(event.index, 1);
    this.dataChange.emit(this.data);
  }

  onSaveDependent(event: { item: any, index: number }) {
    if (!this.data.dependents) this.data.dependents = [];
    if (event.index > -1) {
      this.data.dependents[event.index] = event.item;
    } else {
      this.data.dependents.push(event.item);
    }
    this.dataChange.emit(this.data);
  }

  onDataChange() {
    this.dataChange.emit({ ...this.data });
  }

  onDeleteDependent(event: { item: any, index: number }) {
    this.data.dependents.splice(event.index, 1);
    this.onDataChange();
  }
}
