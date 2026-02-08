import { Component, Input, Output, EventEmitter, inject, OnInit, ChangeDetectorRef } from '@angular/core';
import { SetupService } from '../../../../setup/services/setup.service';
import { CommonModule } from '@angular/common';
import { CreateEmployeeDto } from '../../../models/create-employee.dto';
import { ReactiveFormsModule, FormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { InputTextModule } from 'primeng/inputtext';
import { ButtonModule } from 'primeng/button';
import { SelectModule } from 'primeng/select';
import { DatePickerModule } from 'primeng/datepicker';
import { InputNumberModule } from 'primeng/inputnumber';
import { DialogModule } from 'primeng/dialog';
import { FileUploadModule } from 'primeng/fileupload';

@Component({
  selector: 'app-qualifications-step',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    InputTextModule,
    ButtonModule,
    SelectModule,
    DatePickerModule,
    InputNumberModule,
    DialogModule,
    FileUploadModule
  ],
  templateUrl: './qualifications-step.component.html',
  styles: []
})
export class QualificationsStepComponent implements OnInit {
  @Input() data!: CreateEmployeeDto;
  @Output() dataChange = new EventEmitter<Partial<CreateEmployeeDto>>();
  @Output() prev = new EventEmitter<void>();
  @Output() next = new EventEmitter<void>();
  
  private fb = inject(FormBuilder);
  private setupService = inject(SetupService);
  private cdr = inject(ChangeDetectorRef);

  countries: any[] = [];
  
  // Qualifications State
  showQualDialog = false;
  editingQualIndex: number | null = null;
  qualForm: FormGroup;
  selectedQualFile: File | null = null;

  // Experience State
  showExpDialog = false;
  editingExpIndex: number | null = null;
  expForm: FormGroup;

  currentYear = new Date().getFullYear();

  constructor() {
      // Init Qual Form
      this.qualForm = this.fb.group({
          degreeType: ['', Validators.required],
          majorAr: ['', [Validators.required, Validators.pattern(/^[\u0600-\u06FF\s]+$/)]],
          universityAr: ['', Validators.required],
          graduationYear: [this.currentYear, [Validators.required, Validators.min(1950), Validators.max(this.currentYear + 5)]],
          grade: [''],
          countryId: [null, Validators.required] // Added Country
      });

      // Init Exp Form
      this.expForm = this.fb.group({
          companyNameAr: ['', Validators.required],
          jobTitleAr: ['', Validators.required],
          startDate: [null, [Validators.required]],
          endDate: [null],
          isCurrent: [false]
      });
  }

  ngOnInit() {
      this.loadLookups();
  }

  loadLookups() {
      this.setupService.getAll('Countries').subscribe({
          next: (res: any) => {
              const items = res.data?.items || res.items || res.data || res || [];
              this.countries = items.map((c: any) => ({ label: c.countryNameAr, value: c.countryId }));
              this.cdr.markForCheck();
          }
      });
  }

  // --- Qualifications Handling ---
  openAddQual() {
      this.editingQualIndex = null;
      this.selectedQualFile = null;
      this.qualForm.reset({ graduationYear: this.currentYear });
      this.showQualDialog = true;
  }

  saveQual() {
      if (this.qualForm.invalid) return;
      
      if (!this.data.qualifications) this.data.qualifications = [];

      const val = this.qualForm.value;
      const dto = {
          qualificationId: 0,
          degreeType: val.degreeType,
          majorAr: val.majorAr,
          universityAr: val.universityAr,
          graduationYear: val.graduationYear,
          grade: val.grade,
          countryId: val.countryId // Used selected ID
      };

      if (this.editingQualIndex !== null) {
          const existing = this.data.qualifications[this.editingQualIndex];
          this.data.qualifications[this.editingQualIndex] = { ...existing, ...dto };
      } else {
          this.data.qualifications.push(dto);
      }
      this.showQualDialog = false;
  }

  removeQual(index: number) {
      this.data.qualifications.splice(index, 1);
  }

  // --- Experience Handling ---
  openAddExp() {
      this.editingExpIndex = null;
      this.expForm.reset({ isCurrent: false });
      this.showExpDialog = true;
  }

  saveExp() {
      if (this.expForm.invalid) return;

      if (!this.data.experiences) this.data.experiences = [];

      const val = this.expForm.value;
      // Date Validation
      if (val.startDate && val.endDate && new Date(val.startDate) > new Date(val.endDate)) {
          // Ideally show error on form
          return; 
      }

      const dto = {
          experienceId: 0,
          companyNameAr: val.companyNameAr,
          jobTitleAr: val.jobTitleAr,
          startDate: val.startDate,
          endDate: val.endDate,
          isCurrent: val.isCurrent ? 1 : 0,
          responsibilities: '',
          reasonForLeaving: ''
      };

      if (this.editingExpIndex !== null) {
          const existing = this.data.experiences[this.editingExpIndex];
          this.data.experiences[this.editingExpIndex] = { ...existing, ...dto };
      } else {
          this.data.experiences.push(dto);
      }
      this.showExpDialog = false;
  }

  removeExp(index: number) {
      this.data.experiences.splice(index, 1);
  }

  onPrev() {
    this.prev.emit();
  }

  onNext() {
    this.next.emit();
  }
}
