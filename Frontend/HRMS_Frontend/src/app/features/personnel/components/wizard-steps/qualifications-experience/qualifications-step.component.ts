import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { InputTextModule } from 'primeng/inputtext';
import { InputNumberModule } from 'primeng/inputnumber';
import { DatePickerModule } from 'primeng/datepicker';
import { FileUploadModule } from 'primeng/fileupload';
import { TextareaModule } from 'primeng/textarea';
import { CreateEmployeeDto } from '../../../models/create-employee.dto';
import { Qualification, Certification } from '../../../models/sub-models';
import { DynamicArrayListComponent } from '../../shared/dynamic-array-list/dynamic-array-list.component';

interface QualificationWithFile extends Qualification {
  file?: File;
}

interface CertificationWithFile extends Certification {
  file?: File;
}

@Component({
  selector: 'app-qualifications-step',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    InputTextModule,
    InputNumberModule,
    DatePickerModule,
    FileUploadModule,
    TextareaModule,
    DynamicArrayListComponent
  ],
  templateUrl: './qualifications-step.component.html',
  styles: [`:host { display: block; }`]
})
export class QualificationsStepComponent {
  @Input() data!: CreateEmployeeDto;
  @Output() dataChange = new EventEmitter<Partial<CreateEmployeeDto>>();
  @Output() filesChange = new EventEmitter<Map<number, File>>();
  @Output() certificationFilesChange = new EventEmitter<Map<number, File>>();

  qualColumns = [
    { field: 'degreeType', header: 'الدرجة العلمية' },
    { field: 'majorAr', header: 'التخصص' },
    { field: 'universityAr', header: 'الجامعة/المعهد' },
    { field: 'graduationYear', header: 'سنة التخرج' }
  ];

  certColumns = [
    { field: 'certificationName', header: 'اسم الشهادة' },
    { field: 'issuingOrganization', header: 'جهة الإصدار' },
    { field: 'issueDate', header: 'تاريخ الإصدار' }
  ];

  expColumns = [
    { field: 'jobTitleAr', header: 'المسمى الوظيفي' },
    { field: 'companyNameAr', header: 'اسم الشركة' },
    { field: 'startDate', header: 'تاريخ البدء' }
  ];

  // Qualifications Logic
  onSaveQualification(event: { item: QualificationWithFile, index: number }) {
    if (!this.data.qualifications) this.data.qualifications = [];
    
    if (event.index > -1) {
      this.data.qualifications[event.index] = event.item;
    } else {
      this.data.qualifications.push(event.item);
    }
    
    this.emitChanges();
  }

  onDeleteQualification(event: { item: any, index: number }) {
    this.data.qualifications.splice(event.index, 1);
    this.emitChanges();
  }

  onQualFileSelect(event: any, item: QualificationWithFile) {
    if (event.files && event.files.length > 0) {
      item.file = event.files[0];
    }
  }

  // Certifications Logic
  onSaveCertification(event: { item: CertificationWithFile, index: number }) {
    if (!this.data.certifications) this.data.certifications = [];
    
    if (event.index > -1) {
      this.data.certifications[event.index] = event.item;
    } else {
      this.data.certifications.push(event.item);
    }
    
    this.emitChanges();
  }

  onDeleteCertification(event: { item: any, index: number }) {
    this.data.certifications.splice(event.index, 1);
    this.emitChanges();
  }

  onCertFileSelect(event: any, item: CertificationWithFile) {
    if (event.files && event.files.length > 0) {
      item.file = event.files[0];
    }
  }

  // Experience Logic
  onSaveExperience(event: { item: any, index: number }) {
    if (!this.data.experiences) this.data.experiences = [];
    
    if (event.index > -1) {
      this.data.experiences[event.index] = event.item;
    } else {
      this.data.experiences.push(event.item);
    }
    this.emitChanges();
  }

  onDeleteExperience(event: { item: any, index: number }) {
    this.data.experiences.splice(event.index, 1);
    this.emitChanges();
  }

  onDataChange() {
    this.dataChange.emit({ ...this.data });
  }

  private emitChanges() {
    this.onDataChange();
    
    // Rebuild file map for qualifications 
    // (Note: Wizard expects map<number, File>. Current logic maps index -> file)
    const filesMap = new Map<number, File>();
    this.data.qualifications.forEach((q: QualificationWithFile, index) => {
      if (q.file) {
        filesMap.set(index, q.file);
      }
    });
    
    this.filesChange.emit(filesMap);
    
    // Certifications Files
    const certFilesMap = new Map<number, File>();
    this.data.certifications.forEach((c: CertificationWithFile, index) => {
      if (c.file) {
        certFilesMap.set(index, c.file);
      }
    });

    this.certificationFilesChange.emit(certFilesMap);
  }
}
