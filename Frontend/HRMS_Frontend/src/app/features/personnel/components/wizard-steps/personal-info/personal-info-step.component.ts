import { Component, Input, Output, EventEmitter, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { InputTextModule } from 'primeng/inputtext';
import { DatePickerModule } from 'primeng/datepicker';
import { ButtonModule } from 'primeng/button';
import { InputMaskModule } from 'primeng/inputmask';
import { SelectModule } from 'primeng/select';
import { CreateEmployeeDto } from '../../../models/create-employee.dto';

@Component({
  selector: 'app-personal-info-step',
  standalone: true,
  imports: [
    CommonModule, 
    ReactiveFormsModule, 
    FormsModule,
    InputTextModule,
    InputTextModule,
    DatePickerModule,
    ButtonModule,
    InputMaskModule,
    SelectModule
  ],
  templateUrl: './personal-info-step.component.html',
  styleUrls: ['./personal-info-step.component.scss']
})
export class PersonalInfoStepComponent {
  @Input() data!: CreateEmployeeDto;
  @Output() dataChange = new EventEmitter<Partial<CreateEmployeeDto>>();
  @Output() next = new EventEmitter<void>();

  genders = [
    { label: 'ذكر', value: 'Male' },
    { label: 'أنثى', value: 'Female' }
  ];

  // Lookup data should ideally come from a service (Nationalities etc.)
  nationalities = [
    { label: 'يمني', value: 1 },
    { label: 'سعودي', value: 2 }
  ];

  onChange() {
    this.dataChange.emit(this.data);
  }

  onNext() {
    // Validation logic can go here
    if (!this.data.firstNameAr || !this.data.lastNameAr || !this.data.mobile) {
      // Simple validation for now
      return; 
    }
    this.next.emit();
  }
}
