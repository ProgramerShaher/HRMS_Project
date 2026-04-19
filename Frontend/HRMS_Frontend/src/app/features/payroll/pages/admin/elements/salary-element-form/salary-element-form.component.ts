import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { SelectModule } from 'primeng/select';
import { InputNumberModule } from 'primeng/inputnumber';
import { CheckboxModule } from 'primeng/checkbox';
import { MessageService } from 'primeng/api';
import { ToastModule } from 'primeng/toast';
import { ActionButtonsComponent } from '../../../../../../shared/components/action-buttons/action-buttons.component';
import { SalaryElementService } from '../../../../services/salary-element.service';

/**
 * نموذج عنصر الراتب - واجهة إدارية
 * Salary Element Form - Admin Interface
 */
@Component({
  selector: 'app-salary-element-form',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    ButtonModule,
    InputTextModule,
    SelectModule,
    InputNumberModule,
    CheckboxModule,
    ToastModule,
    ActionButtonsComponent
  ],
  providers: [MessageService],
  templateUrl: './salary-element-form.component.html',
  styleUrl: './salary-element-form.component.scss'
})
export class SalaryElementFormComponent implements OnInit {
  elementForm!: FormGroup;
  loading = signal(false);
  isEditMode = signal(false);
  elementId = signal<number | null>(null);

  typeOptions = [
    { label: 'بدل', value: 'ALLOWANCE' },
    { label: 'خصم', value: 'DEDUCTION' }
  ];

  calculationTypeOptions = [
    { label: 'ثابت', value: 'FIXED' },
    { label: 'نسبة مئوية', value: 'PERCENTAGE' }
  ];

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private elementService: SalaryElementService,
    private messageService: MessageService
  ) {
    this.initializeForm();
  }

  ngOnInit() {
    this.route.params.subscribe(params => {
      const id = +params['id'];
      if (id > 0) {
        this.isEditMode.set(true);
        this.elementId.set(id);
        this.loadElement(id);
      }
    });
  }

  initializeForm() {
    this.elementForm = this.fb.group({
      elementNameAr: ['', [Validators.required, Validators.maxLength(100)]],
      elementType: ['ALLOWANCE', Validators.required],
      isTaxable: [false],
      isGosiBase: [false],
      isRecurring: [true],
      isBasic: [false],
      defaultPercentage: [0, [Validators.required, Validators.min(0), Validators.max(100)]]
    });
  }

  loadElement(id: number) {
    this.loading.set(true);
    
    this.elementService.getElementById(id).subscribe({
      next: (response: any) => {
        if (response.succeeded && response.data) {
          this.elementForm.patchValue(response.data);
        }
        this.loading.set(false);
      },
      error: (err: any) => {
        console.error('Error loading element:', err);
        this.messageService.add({
          severity: 'error',
          summary: 'خطأ',
          detail: 'فشل تحميل بيانات العنصر'
        });
        this.loading.set(false);
      }
    });
  }

  onSubmit() {
    if (this.elementForm.invalid) {
      this.markFormGroupTouched(this.elementForm);
      this.messageService.add({
        severity: 'warn',
        summary: 'تحذير',
        detail: 'يرجى ملء جميع الحقول المطلوبة'
      });
      return;
    }

    this.loading.set(true);
    const formData = this.elementForm.value;

    if (this.isEditMode()) {
      const updateRequest = {
        ...formData,
        elementId: this.elementId()
      };
      
      this.elementService.updateElement(updateRequest).subscribe({
        next: (response: any) => {
          if (response.succeeded) {
            this.messageService.add({
              severity: 'success',
              summary: 'نجح',
              detail: 'تم تحديث العنصر بنجاح'
            });
            
            setTimeout(() => {
              this.router.navigate(['/payroll/elements']);
            }, 1500);
          }
          this.loading.set(false);
        },
        error: (err: any) => {
          console.error('Error saving element:', err);
          this.messageService.add({
            severity: 'error',
            summary: 'خطأ',
            detail: 'فشلت عملية التعديل'
          });
          this.loading.set(false);
        }
      });
    } else {
      this.elementService.createElement(formData).subscribe({
        next: (response: any) => {
          if (response.succeeded) {
            this.messageService.add({
              severity: 'success',
              summary: 'نجح',
              detail: 'تم إنشاء العنصر بنجاح'
            });
            
            setTimeout(() => {
              this.router.navigate(['/payroll/elements']);
            }, 1500);
          }
          this.loading.set(false);
        },
        error: (err: any) => {
          console.error('Error saving element:', err);
          this.messageService.add({
            severity: 'error',
            summary: 'خطأ',
            detail: 'فشلت عملية الحفظ'
          });
          this.loading.set(false);
        }
      });
    }
  }

  onCancel() {
    this.router.navigate(['/payroll/elements/list']);
  }

  private markFormGroupTouched(formGroup: FormGroup) {
    Object.keys(formGroup.controls).forEach(key => {
      const control = formGroup.get(key);
      control?.markAsTouched();
    });
  }

  isFieldInvalid(fieldName: string): boolean {
    const field = this.elementForm.get(fieldName);
    return !!(field && field.invalid && field.touched);
  }

  getFieldError(fieldName: string): string {
    const field = this.elementForm.get(fieldName);
    if (field?.errors) {
      if (field.errors['required']) return 'هذا الحقل مطلوب';
      if (field.errors['maxlength']) return 'تجاوز الحد الأقصى للأحرف';
      if (field.errors['min']) return 'القيمة يجب أن تكون أكبر من أو تساوي 0';
    }
    return '';
  }
}
