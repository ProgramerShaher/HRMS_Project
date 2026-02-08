import { Component, EventEmitter, Input, Output, inject, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { FileUploadModule, FileSelectEvent } from 'primeng/fileupload';
import { InputTextModule } from 'primeng/inputtext';
import { DatePickerModule } from 'primeng/datepicker';
import { ButtonModule } from 'primeng/button';
import { SelectModule } from 'primeng/select';
import { CreateEmployeeDto } from '../../../models/create-employee.dto';
import { MessageService } from 'primeng/api';
import { SetupService } from '../../../../setup/services/setup.service';

@Component({
  selector: 'app-personal-info-step',
  standalone: true,
  imports: [
    CommonModule, 
    ReactiveFormsModule, 
    FormsModule,
    InputTextModule,
    DatePickerModule,
    ButtonModule,
    SelectModule,
    FileUploadModule
  ],
  templateUrl: './personal-info-step.component.html',
  styles: []
})
export class PersonalInfoStepComponent implements OnInit {
  @Input() data!: CreateEmployeeDto;
  @Output() dataChange = new EventEmitter<Partial<CreateEmployeeDto>>();
  @Output() next = new EventEmitter<void>();

  private messageService = inject(MessageService);
  
  profileImagePreview: string | ArrayBuffer | null = null;

  genders = [
    { label: 'ذكر', value: 'M' },
    { label: 'أنثى', value: 'F' }
  ];

  private setupService = inject(SetupService);
  private cdr = inject(ChangeDetectorRef);

  nationalities: any[] = [];

  ngOnInit() {
    this.loadLookups();
  }

  loadLookups() {
      this.setupService.getAll('Countries').subscribe({
          next: (res: any) => {
              const items = res.data?.items || res.items || res.data || res || [];
              this.nationalities = items.map((c: any) => ({ label: c.countryNameAr, value: c.countryId }));
              this.cdr.markForCheck();
          },
          error: (err) => {
              console.error('Failed to load countries', err);
              // Fallback or show error
          }
      });
  }

  onFileSelect(event: any) {
    const files = event.target?.files;
    if (files && files.length > 0) {
      const file = files[0];
      this.fileSelect.emit(file);

      const reader = new FileReader();
      reader.onload = (e) => this.profileImagePreview = e.target?.result as string;
      reader.readAsDataURL(file);
    }
  }

  @Output() fileSelect = new EventEmitter<File>();

  onChange() {
    this.dataChange.emit(this.data);
  }

  onNext() {
    // strict validation
    if (!this.data.employeeNumber) {
        this.messageService.add({ severity: 'error', summary: 'خطأ', detail: 'الرقم الوظيفي مطلوب' });
        return;
    }
    if (!this.data.firstNameAr || !this.data.lastNameAr) {
        this.messageService.add({ severity: 'error', summary: 'خطأ', detail: 'الاسم الأول والأخير مطلوبان' });
        return;
    }
    if (!this.data.nationalId || !/^\d{10}$/.test(this.data.nationalId)) {
        this.messageService.add({ severity: 'error', summary: 'خطأ', detail: 'رقم الهوية يجب أن يتكون من 10 أرقام' });
        return;
    }
    if (!this.data.birthDate) {
        this.messageService.add({ severity: 'error', summary: 'خطأ', detail: 'تاريخ الميلاد مطلوب' });
        return;
    }
    this.next.emit();
  }
}
