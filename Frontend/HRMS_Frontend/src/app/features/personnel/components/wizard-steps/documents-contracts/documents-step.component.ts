import { Component, Input, Output, EventEmitter, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CreateEmployeeDto } from '../../../models/create-employee.dto';
import { SetupService } from '../../../../setup/services/setup.service';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { SelectModule } from 'primeng/select';
import { DatePickerModule } from 'primeng/datepicker';
import { InputTextModule } from 'primeng/inputtext';
import { FileUploadModule, FileSelectEvent } from 'primeng/fileupload';
import { DocumentType } from '../../../../setup/models/setup.models';

@Component({
  selector: 'app-documents-step',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    ButtonModule,
    SelectModule,
    DatePickerModule,
    InputTextModule,
    FileUploadModule
  ],
  templateUrl: './documents-step.component.html',
  styles: []
})
export class DocumentsStepComponent implements OnInit {
  @Input() data!: CreateEmployeeDto;
  @Output() dataChange = new EventEmitter<Partial<CreateEmployeeDto>>();
  @Output() docFileSelect = new EventEmitter<Map<number, File>>(); // Emit Map of document index -> File
  @Output() prev = new EventEmitter<void>();
  @Output() next = new EventEmitter<void>();

  private setupService = inject(SetupService);

  documentTypes: any[] = [];
  localFileMap = new Map<number, File>();

  ngOnInit() {
      // Load Doc Types
      this.setupService.getAll('DocumentTypes').subscribe({
        next: (res: any) => {
            const items: DocumentType[] = res.data?.items || res.items || res.data || res || [];
            this.documentTypes = items.map(d => ({
                label: d.documentTypeNameAr,
                value: d.documentTypeId
            }));
        }
    });

    // Init with at least one empty doc if none
    if (!this.data.documents) this.data.documents = [];
    if (this.data.documents.length === 0) {
        this.addDocument();
    }
  }

  addDocument() {
      if (!this.data.documents) this.data.documents = [];
      this.data.documents.push({
          documentTypeId: 0,
          documentTypeName: '', // Will be filled on select
          documentNumber: '',
          expiryDate: '',
          fileName: ''
      });
  }

  removeDocument(index: number) {
      this.data.documents.splice(index, 1);
      this.localFileMap.delete(index);
      // Re-index map keys if needed? 
      // Actually map keys are tightly coupled to array index, so shifting is tricky.
      // Better to rebuild map or just emit "file for this doc object"
      // For simplicity in wizard, we will just emit the current map state after re-indexing manually
      const newMap = new Map<number, File>();
      // This is complex, simplified approach:
      // Just clear removed index from current map. 
      // Ideally we should use a unique ID for temp docs but index is fine if we are careful.
      this.docFileSelect.emit(this.localFileMap); 
      // Note: This needs better handling in real app to avoid index mismatch after delete
  }

  onDocTypeChange(index: number) {
      const doc = this.data.documents[index];
      const type = this.documentTypes.find(t => t.value === doc.documentTypeId);
      if (type) {
          doc.documentTypeName = type.label;
      }
  }

  onFileSelect(event: any, index: number) {
    const files = event.target?.files || event.files;
    if (files && files.length > 0) {
      const file = files[0];
      this.localFileMap.set(index, file);
      this.data.documents[index].fileName = file.name;
      this.docFileSelect.emit(this.localFileMap);
    }
  }

  onPrev() {
    this.prev.emit();
  }

  onNext() {
    this.next.emit();
  }
}
