import { FileUploadModule, FileSelectEvent } from 'primeng/fileupload';
import { CreateEmployeeDto } from '../../../models/create-employee.dto';
import { TableModule } from 'primeng/table';
import { InputTextModule } from 'primeng/inputtext';
import { DatePickerModule } from 'primeng/datepicker';
import { ButtonModule } from 'primeng/button';
import { Component, EventEmitter, Input, Output, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SelectModule } from 'primeng/select';
import { DepartmentService } from '../../../../setup/services/department.service';
import { SetupService } from '../../../../setup/services/setup.service';
import { Job, DocumentType } from '../../../../setup/models/setup.models';
import { MessageService } from 'primeng/api';

@Component({
  selector: 'app-employment-info-step',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    DatePickerModule,
    ButtonModule,
    SelectModule,
    FileUploadModule,
    TableModule,
    InputTextModule
  ],
  templateUrl: './employment-info-step.component.html',
  styleUrls: ['./employment-info-step.component.scss']
})
export class EmploymentInfoStepComponent implements OnInit {
  @Input() data!: CreateEmployeeDto;
  @Output() dataChange = new EventEmitter<Partial<CreateEmployeeDto>>();
  @Output() docFileSelect = new EventEmitter<Map<number, File>>(); // Emit Map of index -> File
  @Output() prev = new EventEmitter<void>();
  @Output() next = new EventEmitter<void>();

  private departmentService = inject(DepartmentService);
  private setupService = inject(SetupService);
  private messageService = inject(MessageService);

  private fileMap = new Map<number, File>();

  // Real Data Lists
  departments: any[] = []; // mapped to label/value
  jobs: any[] = [];
  documentTypes: any[] = [];

  ngOnInit() {
    this.loadLookups();
  }

  loadLookups() {
    // 1. Departments
    this.departmentService.getAll().subscribe({
        next: (res) => {
            this.departments = res.map(d => ({
                label: d.deptNameAr,
                value: d.deptId
            }));
        },
        error: () => console.error('Failed to load departments')
    });

    // 2. Jobs
    this.setupService.getAll('Jobs').subscribe({
        next: (res: any) => {
             const items: Job[] = res.data?.items || res.items || res.data || res || [];
             this.jobs = items.map(j => ({
                 label: j.jobTitleAr,
                 value: j.jobId
             }));
        },
        error: () => console.error('Failed to load jobs')
    });

    // 3. Document Types
    this.setupService.getAll('DocumentTypes').subscribe({
        next: (res: any) => {
            const items: DocumentType[] = res.data?.items || res.items || res.data || res || [];
            this.documentTypes = items.map(d => ({
                label: d.documentTypeNameAr,
                value: d.documentTypeId
            }));
        },
        error: () => console.error('Failed to load document types')
    });
  }

  onChange() {
    this.dataChange.emit(this.data);
  }

  // Document Management
  addDocument() {
    if (!this.data.documents) {
        this.data.documents = [];
    }
    this.data.documents.push({
        documentTypeId: 0,
        documentNumber: '',
        expiryDate: '',
        filePath: '',
        // documentTypeName: '' // Optional for UI display
    });
    this.onChange();
  }

  removeDocument(index: number) {
    this.data.documents.splice(index, 1);
    this.fileMap.delete(index);
    this.docFileSelect.emit(this.fileMap);
    this.onChange();
  }

  onDocFileSelect(event: FileSelectEvent, index: number) {
    if (event.files && event.files.length > 0) {
      const file = event.files[0];
      this.fileMap.set(index, file);
      this.docFileSelect.emit(this.fileMap);
      
      // Update fileName for UI feedback (optional)

    }
  }

  onPrev() {
    this.prev.emit();
  }

  onNext() {
    if (!this.data.departmentId || !this.data.jobId || !this.data.hireDate) {
       this.messageService.add({ severity: 'warn', summary: 'تنبيه', detail: 'يرجى تعبئة الحقول الإلزامية' });
       return; 
    }
    this.next.emit();
  }
}
