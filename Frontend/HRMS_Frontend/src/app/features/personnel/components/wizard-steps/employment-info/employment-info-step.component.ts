import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { DatePickerModule } from 'primeng/datepicker';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { SelectModule } from 'primeng/select';
import { CreateEmployeeDto } from '../../../models/create-employee.dto';

@Component({
  selector: 'app-employment-info-step',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    DatePickerModule,
    ButtonModule,
    InputTextModule,
    SelectModule
  ],
  templateUrl: './employment-info-step.component.html',
  styleUrls: ['./employment-info-step.component.scss']
})
export class EmploymentInfoStepComponent {
  @Input() data!: CreateEmployeeDto;
  @Output() dataChange = new EventEmitter<Partial<CreateEmployeeDto>>();
  @Output() prev = new EventEmitter<void>();
  @Output() next = new EventEmitter<void>();

  // Mock Data (Should be loaded from Services)
  departments = [
    { label: 'الموارد البشرية', value: 1 },
    { label: 'الإدارة المالية', value: 2 },
    { label: 'تقنية المعلومات', value: 3 },
    { label: 'الارشفة', value: 4 }
  ];

  jobs = [
    { label: 'مدير موارد بشرية', value: 1 },
    { label: 'محاسب', value: 2 },
    { label: 'مطور برمجيات', value: 3 },
    { label: 'اخصائي مخ واعصاب', value: 4 }
  ];

  onChange() {
    this.dataChange.emit(this.data);
  }

  onPrev() {
    this.prev.emit();
  }

  onNext() {
    if (!this.data.departmentId || !this.data.jobId || !this.data.hireDate) {
      return; 
    }
    this.next.emit();
  }
}
