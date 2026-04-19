import { Component, EventEmitter, Input, Output, inject, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { InputTextModule } from 'primeng/inputtext';
import { DatePickerModule } from 'primeng/datepicker';
import { SelectModule } from 'primeng/select';
import { FileUploadModule } from 'primeng/fileupload';
import { InputNumberModule } from 'primeng/inputnumber';
import { CreateEmployeeDto } from '../../../models/create-employee.dto';
import { EmployeeDocument } from '../../../models/sub-models';
import { LookupService, DocumentType } from '../../../../../core/services/lookup.service';
import { DynamicArrayListComponent } from '../../shared/dynamic-array-list/dynamic-array-list.component';

interface DocumentWithFile extends EmployeeDocument {
  file?: File;
  documentTypeName?: string; // For display
}

@Component({
  selector: 'app-documents-step',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    InputTextModule,
    DatePickerModule,
    SelectModule,
    FileUploadModule,
    InputNumberModule,
    DynamicArrayListComponent
  ],
  templateUrl: './documents-step.component.html',
  styles: [`:host { display: block; }`]
})
export class DocumentsStepComponent implements OnInit {
  @Input() data!: CreateEmployeeDto;
  @Output() dataChange = new EventEmitter<Partial<CreateEmployeeDto>>();
  @Output() filesChange = new EventEmitter<Map<number, File>>();

  private lookupService = inject(LookupService);
  private cd = inject(ChangeDetectorRef);
  documentTypes: DocumentType[] = [];

  docColumns = [
    { field: 'documentTypeName', header: 'نوع الوثيقة' },
    { field: 'documentNumber', header: 'رقم الوثيقة' },
    { field: 'expiryDate', header: 'تاريخ الانتهاء' }
  ];

  contractColumns = [
    { field: 'contractType', header: 'نوع العقد' },
    { field: 'startDate', header: 'تاريخ البداية' },
    { field: 'endDate', header: 'تاريخ النهاية' }
  ];

  contractTypes = [
    { label: 'عقد جديد', value: 'New' },
    { label: 'تجديد', value: 'Renewal' }
  ];

  contractStatuses = [
    { label: 'نشط', value: 'Active' },
    { label: 'منتهي', value: 'Expired' },
    { label: 'ملغى', value: 'Terminated' }
  ];

  ngOnInit() {
    this.lookupService.getDocumentTypes().subscribe(types => {
      this.documentTypes = types;
      this.cd.detectChanges();
    });
  }

  onSaveDocument(event: { item: DocumentWithFile, index: number }) {
    if (!this.data.documents) this.data.documents = [];
    
    // Set display name
    const type = this.documentTypes.find(t => t.documentTypeId === event.item.documentTypeId);
    if (type) {
      event.item.documentTypeName = type.documentTypeNameAr;
    }

    if (event.index > -1) {
      this.data.documents[event.index] = event.item;
    } else {
      this.data.documents.push(event.item);
    }
    
    this.emitChanges();
  }

  onDeleteDocument(event: { item: any, index: number }) {
    this.data.documents.splice(event.index, 1);
    this.emitChanges();
  }

  onFileSelect(event: any, item: DocumentWithFile) {
    if (event.files && event.files.length > 0) {
      item.file = event.files[0];
    }
  }

  // Contracts Logic
  onSaveContract(event: { item: any, index: number }) {
    if (!this.data.contracts) this.data.contracts = [];
    
    if (event.index > -1) {
      this.data.contracts[event.index] = event.item;
    } else {
      this.data.contracts.push(event.item);
    }
    this.dataChange.emit(this.data);
  }

  onDeleteContract(event: { item: any, index: number }) {
    this.data.contracts.splice(event.index, 1);
    this.dataChange.emit(this.data);
  }

  onDataChange() {
    this.dataChange.emit({ ...this.data });
  }

  private emitChanges() {
    this.onDataChange();
    
    // Rebuild file map
    const filesMap = new Map<number, File>();
    this.data.documents.forEach((d: DocumentWithFile, index) => {
      if (d.file) {
        filesMap.set(index, d.file);
      }
    });
    
    this.filesChange.emit(filesMap);
  }
}
