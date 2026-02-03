import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { CheckboxModule } from 'primeng/checkbox';
import { InputNumberModule } from 'primeng/inputnumber';
import { DynamicDialogRef, DynamicDialogConfig } from 'primeng/dynamicdialog';
import { SetupService } from '../../services/setup.service';
import { CreateDocumentTypeCommand, DocumentType } from '../../models/setup.models';
import { MessageService } from 'primeng/api';

@Component({
  selector: 'app-document-type-form',
  standalone: true,
  imports: [
    CommonModule, 
    ReactiveFormsModule, 
    ButtonModule, 
    InputTextModule, 
    CheckboxModule, 
    InputNumberModule
  ],
  templateUrl: './document-type-form.component.html',
  styleUrls: ['./document-type-form.component.scss']
})
export class DocumentTypeFormComponent implements OnInit {
  fb = inject(FormBuilder);
  ref = inject(DynamicDialogRef);
  config = inject(DynamicDialogConfig);
  setupService = inject(SetupService);
  messageService = inject(MessageService);

  form!: FormGroup;
  isEditMode = false;
  loading = false;

  ngOnInit() {
    this.initForm();
    if (this.config.data && this.config.data.documentTypeId) {
      this.isEditMode = true;
      this.form.patchValue(this.config.data);
    }
  }

  initForm() {
    this.form = this.fb.group({
      documentTypeId: [null],
      documentTypeNameAr: ['', [Validators.required]],
      documentTypeNameEn: [''],
      description: [''],
      allowedExtensions: [''],
      isRequired: [false],
      hasExpiry: [false],
      defaultExpiryDays: [null],
      maxFileSizeMB: [null]
    });

    // Optional: Add conditional validation or logic here
    // e.g. if hasExpiry is true, maybe defaultExpiryDays should be validated? 
    // Backend allows nulls, so Frontend can too unless strict requirement.
  }

  onSubmit() {
    if (this.form.invalid) return;

    this.loading = true;
    const formData = this.form.value;

    if (this.isEditMode) {
      this.setupService.update('DocumentTypes', formData.documentTypeId, formData).subscribe({
        next: () => {
          this.messageService.add({ severity: 'success', summary: 'نجاح', detail: 'تم التعديل بنجاح' });
          this.ref.close(true);
          this.loading = false;
        },
        error: () => this.loading = false
      });
    } else {
      this.setupService.create('DocumentTypes', formData).subscribe({
        next: () => {
          this.messageService.add({ severity: 'success', summary: 'نجاح', detail: 'تم الإضافة بنجاح' });
          this.ref.close(true);
          this.loading = false;
        },
        error: () => this.loading = false
      });
    }
  }

  onCancel() {
    this.ref.close(false);
  }
}
