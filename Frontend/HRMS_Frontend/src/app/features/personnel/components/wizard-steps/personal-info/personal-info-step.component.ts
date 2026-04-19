import { Component, EventEmitter, Input, Output, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { InputTextModule } from 'primeng/inputtext';
import { DatePickerModule } from 'primeng/datepicker';
import { SelectModule } from 'primeng/select';
import { RadioButtonModule } from 'primeng/radiobutton';
import { FileUploadModule } from 'primeng/fileupload';
import { ImageModule } from 'primeng/image';
import { TooltipModule } from 'primeng/tooltip';
import { CreateEmployeeDto } from '../../../models/create-employee.dto';
import { LookupService, Country } from '../../../../../core/services/lookup.service';

@Component({
  selector: 'app-personal-info-step',
  standalone: true,
  imports: [
    CommonModule, 
    FormsModule,
    InputTextModule,
    DatePickerModule,
    SelectModule,
    RadioButtonModule,
    FileUploadModule,
    ImageModule,
    TooltipModule
  ],
  templateUrl: './personal-info-step.component.html',
  styles: [`
    :host {
      display: block;
    }
  `]
})
export class PersonalInfoStepComponent implements OnInit {
  @Input() data!: CreateEmployeeDto;
  @Output() dataChange = new EventEmitter<Partial<CreateEmployeeDto>>();
  @Output() profilePictureChange = new EventEmitter<File>();

  private lookupService = inject(LookupService);
  
  countries: Country[] = [];
  profileImagePreview: string | null = null;
  maxDate = new Date();
  minDate = new Date(1900, 0, 1);

  genders = [
    { label: 'ذكر', value: 'Male', icon: 'pi pi-mars' },
    { label: 'أنثى', value: 'Female', icon: 'pi pi-venus' }
  ];

  maritalStatuses = [
    { label: 'أعزب', value: 'Single' },
    { label: 'متزوج', value: 'Married' },
    { label: 'مطلق', value: 'Divorced' },
    { label: 'أرمل', value: 'Widowed' }
  ];

  ngOnInit() {
    this.loadLookups();
  }

  loadLookups() {
    this.lookupService.getCountries().subscribe(countries => {
      this.countries = countries;
    });
  }

  onFileSelect(event: any) {
    const file = event.files[0];
    if (file) {
      this.profilePictureChange.emit(file);
      
      const reader = new FileReader();
      reader.onload = (e) => this.profileImagePreview = e.target?.result as string;
      reader.readAsDataURL(file);
    }
  }

  onDataChange() {
    this.dataChange.emit({ ...this.data });
  }

  // Helper to validate English name
  validateEnglishName(event: any) {
    const pattern = /^[a-zA-Z\s]*$/;
    if (!pattern.test(event.key)) {
      event.preventDefault();
    }
  }

  // Helper to validate Arabic name
  validateArabicName(event: any) {
    const pattern = /^[\u0600-\u06FF\s]*$/;
    if (!pattern.test(event.key)) {
      event.preventDefault();
    }
  }

  // Helper to validate Numbers only
  validateNumber(event: any) {
    const pattern = /[0-9]/;
    if (!pattern.test(event.key)) {
      event.preventDefault();
    }
  }
}
