import { Component, Input, Output, EventEmitter, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CreateEmployeeDto } from '../../../models/create-employee.dto';
import { ReactiveFormsModule, FormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { SelectModule } from 'primeng/select';
import { MessageService } from 'primeng/api';
import { DatePickerModule } from 'primeng/datepicker';
import { InputTextModule } from 'primeng/inputtext';

@Component({
  selector: 'app-family-step',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    ButtonModule,
    SelectModule,
    DatePickerModule,
    InputTextModule
  ],
  templateUrl: './family-step.component.html',
  styles: []
})
export class FamilyStepComponent {
  @Input() data!: CreateEmployeeDto;
  @Output() dataChange = new EventEmitter<Partial<CreateEmployeeDto>>();
  @Output() prev = new EventEmitter<void>();
  @Output() submitForm = new EventEmitter<void>();

  private fb = inject(FormBuilder);
  showDependentDialog = false;
  editingIndex: number | null = null;
  depForm: FormGroup;
  
  relations = [
    { label: 'زوج/زوجة', value: 'Spouse' },
    { label: 'ابن/ابنة', value: 'Child' },
    { label: 'والد/والدة', value: 'Parent' }
  ];

  genders = [
    { label: 'ذكر', value: 'M' },
    { label: 'أنثى', value: 'F' }
  ];

  constructor() {
      this.depForm = this.fb.group({
          fullNameAr: ['', Validators.required],
          relationship: ['', Validators.required],
          birthDate: [null, Validators.required],
          nationalId: [''],
          gender: ['M']
      });
  }

  addDependent() {
      this.editingIndex = null;
      this.depForm.reset({ gender: 'M' });
      this.showDependentDialog = true;
  }

  saveDependent() {
      if (this.depForm.invalid) return;

      if (!this.data.dependents) this.data.dependents = [];
      
      const val = this.depForm.value;
      const dto = {
          dependentId: 0,
          fullNameAr: val.fullNameAr,
          fullNameEn: val.fullNameAr, // Default copy
          relationship: val.relationship,
          birthDate: val.birthDate,
          nationalId: val.nationalId,
          gender: val.gender
      };

      if (this.editingIndex !== null) {
          const existing = this.data.dependents[this.editingIndex];
          this.data.dependents[this.editingIndex] = { ...existing, ...dto };
      } else {
          this.data.dependents.push(dto);
      }
      this.showDependentDialog = false;
  }

  removeDependent(index: number) {
      this.data.dependents.splice(index, 1);
  }

  onPrev() {
    this.prev.emit();
  }

  onSubmit() {
    // Final Validation if needed
    this.submitForm.emit();
  }
}
