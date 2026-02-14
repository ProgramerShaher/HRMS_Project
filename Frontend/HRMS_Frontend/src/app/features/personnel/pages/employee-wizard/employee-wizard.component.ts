import { Component, signal, inject, OnInit, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, ActivatedRoute } from '@angular/router';
import { StepperModule } from 'primeng/stepper';
import { ButtonModule } from 'primeng/button';
import { ToastModule } from 'primeng/toast';
import { MessageService } from 'primeng/api';
import { ProgressBarModule } from 'primeng/progressbar';
import { CardModule } from 'primeng/card';
import { DividerModule } from 'primeng/divider';
import { CreateEmployeeDto } from '../../models/create-employee.dto';
import { EmployeeService } from '../../services/employee.service';
import { EmployeeValidationService } from '../../services/employee-validation.service';
import { FileUploadRequest, UploadProgress } from '../../models/file-upload.models';

// Steps (using existing components)
import { PersonalInfoStepComponent } from '../../components/wizard-steps/personal-info/personal-info-step.component';
import { JobFinancialStepComponent } from '../../components/wizard-steps/job-financial/job-financial-step.component';
import { AddressesStepComponent } from '../../components/wizard-steps/addresses-contact/addresses-step.component';
import { QualificationsStepComponent } from '../../components/wizard-steps/qualifications-experience/qualifications-step.component';
import { DocumentsStepComponent } from '../../components/wizard-steps/documents-contracts/documents-step.component';
import { FamilyStepComponent } from '../../components/wizard-steps/family-preferences/family-step.component';


/**
 * Employee Wizard Component
 * Professional multi-step wizard for creating/editing employees
 * Features:
 * - Flexible navigation (no step restrictions)
 * - Auto-save draft capability
 * - Professional file upload handling
 * - Complete validation
 * - Modern, responsive design
 */
@Component({
  selector: 'app-employee-wizard',
  standalone: true,
  imports: [
    CommonModule,
    StepperModule,
    ButtonModule,
    ToastModule,
    ProgressBarModule,
    CardModule,
    DividerModule,
    PersonalInfoStepComponent,
    JobFinancialStepComponent,
    AddressesStepComponent,
    QualificationsStepComponent,
    DocumentsStepComponent,
    FamilyStepComponent,

  ],
  providers: [MessageService],
  templateUrl: './employee-wizard.component.html',
  styleUrls: ['./employee-wizard.component.scss']
})
export class EmployeeWizardComponent implements OnInit {
  // Helpers for templates
  Math = Math;
  mathRound = Math.round;

  // Services
  private employeeService = inject(EmployeeService);
  private validationService = inject(EmployeeValidationService);
  private messageService = inject(MessageService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);

  // Wizard State
  currentStep = signal<number>(0);
  loading = signal<boolean>(false);
  isEditMode = signal<boolean>(false);
  employeeId = signal<number | null>(null);
  uploadProgress = signal<UploadProgress[]>([]);

  // Form Data State
  employeeData = signal<CreateEmployeeDto>({
    // Personal Info
    employeeNumber: '', // Backend generated
    firstNameAr: '',
    secondNameAr: '',
    thirdNameAr: '',
    lastNameAr: '',
    fullNameEn: '',
    birthDate: new Date(),
    gender: 'Male',
    mobile: '',
    email: '',
    nationalityId: 0,
    nationalId: '',
    maritalStatus: '',

    // Employment Info
    departmentId: 0,
    jobId: 0,
    jobGradeId: undefined,
    managerId: undefined,
    hireDate: new Date(),
    licenseNumber: '',
    licenseExpiryDate: undefined,
    specialty: '',

    // Financial Info
    basicSalary: 0,
    housingAllowance: 0,
    transportAllowance: 0,
    medicalAllowance: 0,

    // Bank Info
    bankId: undefined,
    ibanNumber: '',

    // Arrays
    qualifications: [],
    experiences: [],
    emergencyContacts: [],
    contracts: [],
    certifications: [],
    bankAccounts: [],
    dependents: [],
    addresses: [],
    documents: []
  });

  // File State
  profilePictureFile = signal<File | null>(null);
  documentFiles = signal<Map<number, File>>(new Map());
  qualificationFiles = signal<Map<number, File>>(new Map());
  certificationFiles = signal<Map<number, File>>(new Map());

  // Computed Properties
  totalSalary = computed(() => {
    const data = this.employeeData();
    return (data.basicSalary || 0) +
      (data.housingAllowance || 0) +
      (data.transportAllowance || 0) +
      (data.medicalAllowance || 0);
  });

  canSave = computed(() => {
    const data = this.employeeData();
    const required = this.validationService.getRequiredFields();

    // Check if all required fields are filled
    return required.every(field => {
      const value = (data as any)[field];
      return value !== undefined && value !== null && value !== '';
    });
  });

  // Step Definitions (6 steps matching available components)
  steps = [
    { label: 'المعلومات الشخصية', icon: 'pi pi-user' },
    { label: 'الوظيفة والمالية', icon: 'pi pi-briefcase' },
    { label: 'العناوين والاتصال', icon: 'pi pi-map-marker' },
    { label: 'المؤهلات والخبرات', icon: 'pi pi-graduation-cap' },
    { label: 'المستندات والعقود', icon: 'pi pi-file' },
    { label: 'العائلة وجهات الاتصال', icon: 'pi pi-users' }
  ];

  ngOnInit(): void {
    // Check if edit mode
    this.route.paramMap.subscribe(params => {
      const id = params.get('id');
      if (id) {
        this.isEditMode.set(true);
        this.employeeId.set(+id);
        this.loadEmployeeData(+id);
      }
    });
  }

  /**
   * Load employee data for editing
   */
  loadEmployeeData(id: number): void {
    this.loading.set(true);

    this.employeeService.getFullProfile(id).subscribe({
      next: (profile: any) => {
        // Helper to get property regardless of casing
        const get = (obj: any, key: string) => {
          if (!obj) return undefined;
          const camel = key.charAt(0).toLowerCase() + key.slice(1);
          const pascal = key.charAt(0).toUpperCase() + key.slice(1);
          return obj[camel] !== undefined ? obj[camel] : obj[pascal];
        };

        const core = get(profile, 'coreProfile');
        const comp = get(profile, 'compensation');

        const mapCollection = (arr: any[], mapper: (item: any) => any) => {
          return (arr || []).map(item => mapper(item));
        };

        this.employeeData.set({
          // Personal Info
          employeeNumber: get(core, 'employeeNumber') || '',
          firstNameAr: get(core, 'firstNameAr') || '',
          secondNameAr: get(core, 'secondNameAr') || '',
          thirdNameAr: get(core, 'thirdNameAr') || '',
          lastNameAr: get(core, 'lastNameAr') || get(core, 'hijriLastNameAr') || '',
          fullNameEn: get(core, 'fullNameEn') || '',
          birthDate: get(core, 'birthDate') ? new Date(get(core, 'birthDate')) : new Date(),
          gender: get(core, 'gender') || 'Male',
          mobile: get(core, 'mobile') || '',
          email: get(core, 'email') || '',
          nationalityId: get(core, 'nationalityId') || 0,
          nationalId: get(core, 'nationalId') || '',
          maritalStatus: get(core, 'maritalStatus') || '',

          // Employment Info
          departmentId: get(core, 'deptId') || get(core, 'departmentId') || 0,
          jobId: get(core, 'jobId') || 0,
          jobGradeId: get(core, 'jobGradeId'),
          managerId: get(core, 'managerId'),
          hireDate: get(core, 'hireDate') ? new Date(get(core, 'hireDate')) : new Date(),
          licenseNumber: get(core, 'licenseNumber') || '',
          licenseExpiryDate: get(core, 'licenseExpiryDate') ? new Date(get(core, 'licenseExpiryDate')) : undefined,
          specialty: get(core, 'specialty') || '',

          // Financial Info
          basicSalary: get(comp, 'basicSalary') || 0,
          housingAllowance: get(comp, 'housingAllowance') || 0,
          transportAllowance: get(comp, 'transportAllowance') || 0,
          medicalAllowance: get(comp, 'medicalAllowance') || get(comp, 'otherAllowances') || 0,

          // Bank (from compensation or separate if available)
          bankId: get(comp, 'bankId'),
          ibanNumber: get(comp, 'ibanNumber') || '',

          // Collections - CRITICAL: Map to new property names
          qualifications: mapCollection(get(profile, 'qualifications'), q => ({
            qualificationId: get(q, 'qualificationId'),
            degreeType: get(q, 'degreeType'),
            majorAr: get(q, 'majorAr') || get(q, 'fieldOfStudy'),
            universityAr: get(q, 'universityAr') || get(q, 'institution'),
            graduationYear: get(q, 'graduationYear'),
            grade: get(q, 'grade'),
            countryId: get(q, 'countryId')
          })),

          experiences: mapCollection(get(profile, 'experiences'), e => ({
            experienceId: get(e, 'experienceId'),
            companyNameAr: get(e, 'companyNameAr') || get(e, 'companyName'),
            jobTitleAr: get(e, 'jobTitleAr') || get(e, 'jobTitle'),
            startDate: get(e, 'startDate'),
            endDate: get(e, 'endDate'),
            isCurrent: get(e, 'isCurrent') || get(e, 'isCurrentJob'),
            responsibilities: get(e, 'responsibilities'),
            reasonForLeaving: get(e, 'reasonForLeaving')
          })),

          emergencyContacts: mapCollection(get(profile, 'emergencyContacts'), c => ({
            contactId: get(c, 'contactId') || get(c, 'emergencyContactId'),
            contactNameAr: get(c, 'contactNameAr') || get(c, 'contactName'),
            relationship: get(c, 'relationship'),
            phonePrimary: get(c, 'phonePrimary') || get(c, 'primaryPhone'),
            phoneSecondary: get(c, 'phoneSecondary') || get(c, 'secondaryPhone'),
            isPrimary: get(c, 'isPrimary')
          })),

          contracts: mapCollection(get(profile, 'contracts'), con => ({
            contractId: get(con, 'contractId'),
            contractType: get(con, 'contractType'),
            startDate: get(con, 'startDate'),
            endDate: get(con, 'endDate'),
            isRenewable: get(con, 'isRenewable'),
            basicSalary: get(con, 'basicSalary'),
            housingAllowance: get(con, 'housingAllowance'),
            transportAllowance: get(con, 'transportAllowance'),
            otherAllowances: get(con, 'otherAllowances'),
            workingHoursDaily: get(con, 'workingHoursDaily'),
            vacationDays: get(con, 'vacationDays')
          })),

          certifications: mapCollection(get(profile, 'certifications'), cert => ({
            certificationId: get(cert, 'certificationId'),
            certificationName: get(cert, 'certificationName'),
            issuingOrganization: get(cert, 'issuingOrganization'),
            issueDate: get(cert, 'issueDate'),
            expiryDate: get(cert, 'expiryDate'),
            credentialId: get(cert, 'credentialId') || get(cert, 'certificationNumber'),
            credentialUrl: get(cert, 'credentialUrl') || get(cert, 'verificationUrl')
          })),

          bankAccounts: mapCollection(get(profile, 'bankAccounts'), b => ({
            employeeBankAccountId: get(b, 'employeeBankAccountId') || get(b, 'bankAccountId'),
            bankId: get(b, 'bankId'),
            accountHolderName: get(b, 'accountHolderName'),
            ibanNumber: get(b, 'ibanNumber'),
            isPrimary: get(b, 'isPrimary')
          })),

          dependents: mapCollection(get(profile, 'dependents'), dep => ({
            dependentId: get(dep, 'dependentId'),
            fullNameAr: get(dep, 'fullNameAr') || get(dep, 'fullName'),
            fullNameEn: get(dep, 'fullNameEn'),
            relationship: get(dep, 'relationship'),
            birthDate: get(dep, 'birthDate'),
            nationalId: get(dep, 'nationalId'),
            gender: get(dep, 'gender')
          })),

          addresses: mapCollection(get(profile, 'addresses'), a => ({
            addressId: get(a, 'addressId'),
            addressType: get(a, 'addressType'),
            street: get(a, 'street'),
            cityId: get(a, 'cityId') || get(a, 'city'),
            buildingNumber: get(a, 'buildingNumber'),
            zipCode: get(a, 'zipCode') || get(a, 'postalCode'),
            additionalDetails: get(a, 'additionalDetails') || get(a, 'district')
          })),

          documents: mapCollection(get(profile, 'documents'), d => ({
            documentId: get(d, 'documentId'),
            documentTypeId: get(d, 'documentTypeId'),
            documentNumber: get(d, 'documentNumber'),
            expiryDate: get(d, 'expiryDate'),
            fileName: get(d, 'fileName'),
            filePath: get(d, 'filePath')
          }))
        });

        this.loading.set(false);
        this.messageService.add({
          severity: 'success',
          summary: 'تم التحميل',
          detail: 'تم تحميل بيانات الموظف بنجاح'
        });
      },
      error: (err) => {
        console.error('Failed to load employee:', err);
        this.loading.set(false);
        this.messageService.add({
          severity: 'error',
          summary: 'خطأ',
          detail: 'فشل تحميل بيانات الموظف'
        });
        this.router.navigate(['/personnel']);
      }
    });
  }

  /**
   * Update employee data from step components
   */
  updateData(data: Partial<CreateEmployeeDto>): void {
    this.employeeData.update(current => ({ ...current, ...data }));
  }

  /**
   * Update profile picture
   */
  updateProfilePicture(file: File): void {
    this.profilePictureFile.set(file);
  }

  /**
   * Update document files
   */
  updateDocumentFiles(files: Map<number, File>): void {
    this.documentFiles.set(files);
  }

  /**
   * Update qualification files
   */
  updateQualificationFiles(files: Map<number, File>): void {
    this.qualificationFiles.set(files);
  }

  /**
   * Update certification files
   */
  updateCertificationFiles(files: Map<number, File>): void {
    this.certificationFiles.set(files);
  }

  /**
   * Navigate to specific step (flexible navigation)
   */
  goToStep(stepIndex: number): void {
    if (stepIndex >= 0 && stepIndex < this.steps.length) {
      this.currentStep.set(stepIndex);
    }
  }

  /**
   * Navigate to next step
   */
  nextStep(): void {
    if (this.currentStep() < this.steps.length - 1) {
      this.currentStep.update(step => step + 1);
    }
  }

  /**
   * Navigate to previous step
   */
  prevStep(): void {
    if (this.currentStep() > 0) {
      this.currentStep.update(step => step - 1);
    }
  }

  /**
   * Save draft (save with current data, even if incomplete)
   */
  saveDraft(): void {
    this.messageService.add({
      severity: 'info',
      summary: 'حفظ مسودة',
      detail: 'سيتم إضافة هذه الميزة قريباً'
    });
  }

  /**
   * Submit employee data
   */
  submit(): void {
    const data = this.employeeData();

    // Validate required fields
    const validation = this.validationService.validateEmployeeData(data);
    if (!validation.isValid) {
      this.messageService.add({
        severity: 'error',
        summary: 'خطأ في البيانات',
        detail: validation.errors.join(' | '),
        life: 5000
      });
      console.error('Validation errors:', validation.errors);
      return;
    }

    this.loading.set(true);
    const action = this.isEditMode() ? 'تحديث' : 'إنشاء';

    this.messageService.add({
      severity: 'info',
      summary: `جاري ${action} الموظف`,
      detail: 'يرجى الانتظار...',
      sticky: true
    });

    const submissionPayload = this.prepareDataForSubmission(data);

    // Create or update employee
    // Using explicit Observable<any> to avoid type mismatch
    const request$: import('rxjs').Observable<any> = this.isEditMode() && this.employeeId()
      ? this.employeeService.update(this.employeeId()!, submissionPayload as any)
      : this.employeeService.create(submissionPayload as any);

    request$.subscribe({
      next: (response: any) => {
        // Extract ID from Result<T> wrapper
        // The backend returns Result<int> for create and Result<bool> for update
        const id = this.isEditMode() ? this.employeeId()! : (response.data || response);

        if (typeof id !== 'number' && !this.isEditMode()) {
          console.warn('ID from backend is not a number:', id);
        }

        const employeeId = Number(id);

        // Handle file uploads
        this.handleFileUploads(employeeId);
      },
      error: (err: any) => {
        this.loading.set(false);
        this.messageService.clear();

        let errorMessage = 'حدث خطأ أثناء حفظ البيانات';

        // Parse validation errors from backend
        if (err.status === 400 && err.error?.errors) {
          const errors = Object.entries(err.error.errors)
            .map(([key, value]) => `${key}: ${(value as string[]).join(', ')}`)
            .join('\n');
          errorMessage = `خطأ في التحقق:\n${errors}`;
        }

        this.messageService.add({
          severity: 'error',
          summary: 'فشل الحفظ',
          detail: errorMessage,
          life: 10000
        });

        console.error('Submission error:', err);
      }
    });
  }

  /**
   * Handle file uploads after employee creation
   */
  private handleFileUploads(employeeId: number): void {
    const uploadTasks: Promise<any>[] = [];

    // 1. Profile Picture
    const profilePic = this.profilePictureFile();
    if (profilePic) {
      uploadTasks.push(
        this.employeeService.uploadProfilePicture(employeeId, profilePic).toPromise()
      );
    }

    // 2. Documents
    const documents = this.employeeData().documents || [];
    const docFiles = this.documentFiles();

    documents.forEach((doc, index) => {
      const file = docFiles.get(index);
      if (file && doc.documentTypeId) {
        const expiryDate = doc.expiryDate
          ? new Date(doc.expiryDate).toISOString()
          : undefined;

        uploadTasks.push(
          this.employeeService.uploadDocument(
            employeeId,
            file,
            doc.documentTypeId,
            doc.documentNumber,
            expiryDate
          ).toPromise()
        );
      }
    });

    // 3. Qualification Files
    const qualifications = this.employeeData().qualifications || [];
    const qualFiles = this.qualificationFiles();

    qualifications.forEach((qual, index) => {
      const file = qualFiles.get(index);
      if (file) {
        uploadTasks.push(
          this.employeeService.addQualification(employeeId, qual, file).toPromise()
        );
      }
    });

    // 4. Certification Files
    const certifications = this.employeeData().certifications || [];
    const certFiles = this.certificationFiles();

    certifications.forEach((cert, index) => {
      const file = certFiles.get(index);
      if (file) {
        uploadTasks.push(
          this.employeeService.addCertification(employeeId, cert, file).toPromise()
        );
      }
    });

    // Execute all uploads
    if (uploadTasks.length > 0) {
      Promise.allSettled(uploadTasks).then((results) => {
        const failed = results.filter(r => r.status === 'rejected').length;

        this.loading.set(false);
        this.messageService.clear();

        if (failed === 0) {
          this.messageService.add({
            severity: 'success',
            summary: 'تم بنجاح',
            detail: 'تم حفظ بيانات الموظف والملفات بنجاح'
          });
        } else {
          this.messageService.add({
            severity: 'warn',
            summary: 'تم الحفظ مع تحذيرات',
            detail: `تم حفظ البيانات لكن فشل رفع ${failed} ملف(ات)`
          });
        }

        // Navigate to employee list
        setTimeout(() => {
          this.router.navigate(['/personnel']);
        }, 1500);
      });
    } else {
      // No files to upload
      this.loading.set(false);
      this.messageService.clear();
      this.messageService.add({
        severity: 'success',
        summary: 'تم بنجاح',
        detail: 'تم حفظ بيانات الموظف بنجاح'
      });

      setTimeout(() => {
        this.router.navigate(['/personnel']);
      }, 1500);
    }
  }

  /**
   * Prepare data for backend (convert booleans to 0/1, format dates, etc.)
   */
  private prepareDataForSubmission(data: CreateEmployeeDto): any {
    const result = JSON.parse(JSON.stringify(data));

    // Helper for ISO format dates
    const formatDate = (date: any) => {
      if (!date) return null;
      try {
        const d = new Date(date);
        return d.toISOString();
      } catch (e) {
        return null;
      }
    };

    // 1. Root Level transformations
    if (result.gender === 'Male' || result.gender === 'M') result.gender = 'M';
    else if (result.gender === 'Female' || result.gender === 'F') result.gender = 'F';

    result.birthDate = formatDate(result.birthDate);
    result.hireDate = formatDate(result.hireDate);
    if (result.licenseExpiryDate) {
      result.licenseExpiryDate = formatDate(result.licenseExpiryDate);
    }

    // Ensure Nullable IDs are actually null if not selected
    if (!result.bankId || result.bankId <= 0) result.bankId = null;
    if (!result.jobGradeId || result.jobGradeId <= 0) result.jobGradeId = null;
    if (!result.managerId || result.managerId <= 0) result.managerId = null;
    if (!result.nationalityId || result.nationalityId <= 0) result.nationalityId = null;

    // 2. Sub-Entities - CRITICAL: byte vs bool handling

    // Emergency Contacts: isPrimary is byte (0/1) in C#
    result.emergencyContacts = result.emergencyContacts?.map((c: any) => ({
      contactId: c.contactId || c.emergencyContactId,
      contactNameAr: c.contactNameAr || c.contactName,
      relationship: c.relationship,
      phonePrimary: c.phonePrimary || c.primaryPhone,
      phoneSecondary: c.phoneSecondary || c.secondaryPhone,
      isPrimary: c.isPrimary ? 1 : 0
    }));

    // Experiences: isCurrent is byte (0/1) in C#
    result.experiences = result.experiences?.map((e: any) => ({
      companyNameAr: e.companyNameAr || e.companyName,
      jobTitleAr: e.jobTitleAr || e.jobTitle,
      startDate: formatDate(e.startDate),
      endDate: formatDate(e.endDate),
      isCurrent: e.isCurrent ? 1 : 0, // In C# it's IsCurrent
      responsibilities: e.responsibilities,
      reasonForLeaving: e.reasonForLeaving
    }));

    // Bank Accounts: isPrimary is bool (true/false) in C#
    result.bankAccounts = result.bankAccounts?.map((b: any) => ({
      employeeBankAccountId: b.employeeBankAccountId || b.bankAccountId,
      bankId: Number(b.bankId),
      accountHolderName: b.accountHolderName,
      ibanNumber: b.ibanNumber,
      isPrimary: !!b.isPrimary
    }));

    // Qualifications: short GraduationYear
    result.qualifications = result.qualifications?.map((q: any) => ({
      degreeType: q.degreeType,
      majorAr: q.majorAr || q.fieldOfStudy,
      universityAr: q.universityAr || q.institution,
      countryId: q.countryId ? Number(q.countryId) : null,
      graduationYear: q.graduationYear ? Number(q.graduationYear) : null,
      grade: q.grade
    }));

    // Addresses
    result.addresses = result.addresses?.map((a: any) => ({
      addressType: a.addressType || 'Home',
      cityId: (a.cityId && a.cityId > 0) ? Number(a.cityId) : null,
      street: a.street,
      buildingNumber: a.buildingNumber,
      zipCode: a.zipCode || a.postalCode,
      additionalDetails: a.additionalDetails || a.district
    }));

    // Documents
    result.documents = result.documents?.map((d: any) => ({
      documentId: Number(d.documentId || 0),
      documentTypeId: Number(d.documentTypeId || 1),
      documentNumber: d.documentNumber,
      expiryDate: formatDate(d.expiryDate),
      filePath: d.filePath || '',
      fileName: d.fileName || ''
    }));

    // Dependents
    result.dependents = result.dependents?.map((dep: any) => ({
      fullNameAr: dep.fullNameAr || dep.fullName,
      fullNameEn: dep.fullNameEn,
      relationship: dep.relationship,
      birthDate: formatDate(dep.birthDate),
      nationalId: dep.nationalId,
      gender: dep.gender
    }));

    // 3. Financials (ensure numeric)
    result.basicSalary = Number(result.basicSalary || 0);
    result.housingAllowance = Number(result.housingAllowance || 0);
    result.transportAllowance = Number(result.transportAllowance || 0);
    result.medicalAllowance = Number(result.medicalAllowance || 0);

    return result;
  }

  /**
   * Helper to convert camelCase object to PascalCase for .NET backend
   */
  private toPascalCase(obj: any): any {
    if (Array.isArray(obj)) {
      return obj.map(v => this.toPascalCase(v));
    } else if (obj !== null && obj.constructor === Object) {
      return Object.keys(obj).reduce((result, key) => {
        const pascalKey = key.charAt(0).toUpperCase() + key.slice(1);
        result[pascalKey] = this.toPascalCase(obj[key]);
        return result;
      }, {} as any);
    }
    return obj;
  }

  /**
   * Cancel and go back
   */
  cancel(): void {
    if (confirm('هل أنت متأكد من الإلغاء؟ سيتم فقدان جميع التغييرات غير المحفوظة.')) {
      this.router.navigate(['/personnel']);
    }
  }
}
