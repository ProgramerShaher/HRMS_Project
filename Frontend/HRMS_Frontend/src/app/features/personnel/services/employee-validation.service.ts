import { Injectable } from '@angular/core';
import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';

/**
 * Employee Validation Service
 * Mirrors backend validation rules and provides frontend validation
 */
@Injectable({
  providedIn: 'root'
})
export class EmployeeValidationService {

  constructor() {}

  /**
   * Get required fields for quick save (minimal employee creation)
   */
  getRequiredFields(): string[] {
    return [
      'employeeNumber',
      'firstNameAr',
      'secondNameAr',
      'thirdNameAr',
      'lastNameAr',
      'fullNameEn',
      'birthDate',
      'gender',
      'mobile',
      'nationalId',
      'nationalityId',
      'departmentId',
      'jobId',
      'hireDate',
      'basicSalary'
    ];
  }

  /**
   * Validate Saudi mobile number
   */
  saudiMobileValidator(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      if (!control.value) return null;

      const mobileRegex = /^(05|5)(5|0|3|6|4|9|1|8|7)([0-9]{7})$/;
      const isValid = mobileRegex.test(control.value.replace(/\s/g, ''));

      return isValid ? null : { invalidMobile: { value: control.value } };
    };
  }

  /**
   * Validate Saudi National ID
   */
  saudiNationalIdValidator(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      if (!control.value) return null;

      const idRegex = /^[12]\d{9}$/;
      const isValid = idRegex.test(control.value);

      return isValid ? null : { invalidNationalId: { value: control.value } };
    };
  }

  /**
   * Validate IBAN (Saudi format)
   */
  saudiIbanValidator(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      if (!control.value) return null;

      // Saudi IBAN: SA + 22 digits
      const ibanRegex = /^SA\d{22}$/;
      const isValid = ibanRegex.test(control.value.replace(/\s/g, ''));

      return isValid ? null : { invalidIban: { value: control.value } };
    };
  }

  /**
   * Validate age (minimum 18, maximum 65)
   */
  ageValidator(minAge: number = 18, maxAge: number = 65): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      if (!control.value) return null;

      const birthDate = new Date(control.value);
      const today = new Date();
      let age = today.getFullYear() - birthDate.getFullYear();
      const monthDiff = today.getMonth() - birthDate.getMonth();

      if (monthDiff < 0 || (monthDiff === 0 && today.getDate() < birthDate.getDate())) {
        age--;
      }

      if (age < minAge) {
        return { minAge: { requiredAge: minAge, actualAge: age } };
      }

      if (age > maxAge) {
        return { maxAge: { requiredAge: maxAge, actualAge: age } };
      }

      return null;
    };
  }

  /**
   * Validate date range (start date must be before end date)
   */
  dateRangeValidator(startDateField: string, endDateField: string): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      const startDate = control.get(startDateField)?.value;
      const endDate = control.get(endDateField)?.value;

      if (!startDate || !endDate) return null;

      const start = new Date(startDate);
      const end = new Date(endDate);

      return start < end ? null : { invalidDateRange: true };
    };
  }

  /**
   * Validate Arabic text
   */
  arabicTextValidator(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      if (!control.value) return null;

      const arabicRegex = /^[\u0600-\u06FF\s]+$/;
      const isValid = arabicRegex.test(control.value);

      return isValid ? null : { notArabic: { value: control.value } };
    };
  }

  /**
   * Validate English text
   */
  englishTextValidator(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      if (!control.value) return null;

      const englishRegex = /^[a-zA-Z\s]+$/;
      const isValid = englishRegex.test(control.value);

      return isValid ? null : { notEnglish: { value: control.value } };
    };
  }

  /**
   * Get validation error message in Arabic
   */
  getErrorMessage(errorKey: string, errorValue?: any): string {
    const errorMessages: { [key: string]: string } = {
      required: 'هذا الحقل مطلوب',
      invalidMobile: 'رقم الجوال غير صحيح (يجب أن يبدأ بـ 05 ويتكون من 10 أرقام)',
      invalidNationalId: 'رقم الهوية الوطنية غير صحيح (يجب أن يبدأ بـ 1 أو 2 ويتكون من 10 أرقام)',
      invalidIban: 'رقم الآيبان غير صحيح (يجب أن يبدأ بـ SA ويتكون من 24 حرف)',
      minAge: `العمر يجب أن يكون ${errorValue?.requiredAge} سنة على الأقل`,
      maxAge: `العمر يجب أن لا يتجاوز ${errorValue?.requiredAge} سنة`,
      invalidDateRange: 'تاريخ البداية يجب أن يكون قبل تاريخ النهاية',
      notArabic: 'يجب إدخال نص عربي فقط',
      notEnglish: 'يجب إدخال نص إنجليزي فقط',
      email: 'البريد الإلكتروني غير صحيح',
      min: `القيمة يجب أن تكون ${errorValue?.min} على الأقل`,
      max: `القيمة يجب أن لا تتجاوز ${errorValue?.max}`,
      minlength: `يجب إدخال ${errorValue?.requiredLength} حرف على الأقل`,
      maxlength: `يجب أن لا يتجاوز ${errorValue?.requiredLength} حرف`
    };

    return errorMessages[errorKey] || 'قيمة غير صحيحة';
  }

  /**
   * Validate complete employee data before submission
   */
  validateEmployeeData(data: any): { isValid: boolean; errors: string[] } {
    const errors: string[] = [];
    const requiredFields = this.getRequiredFields();

    // Check required fields
    requiredFields.forEach(field => {
      const value = data[field];
      if (value === undefined || value === null || value === '' || value === 0) {
        errors.push(`${field} مطلوب`);
      }
    });

    // National ID 10 digits
    if (data.nationalId && !/^\d{10}$/.test(data.nationalId)) {
      errors.push('رقم الهوية يجب أن يكون 10 أرقام');
    }

    // Email format
    if (data.email && !/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(data.email)) {
      errors.push('البريد الإلكتروني غير صحيح');
    }

    // Additional business rules
    if (data.basicSalary && data.basicSalary < 0) {
      errors.push('الراتب الأساسي يجب أن يكون أكبر من صفر');
    }

    return {
      isValid: errors.length === 0,
      errors
    };
  }
}
