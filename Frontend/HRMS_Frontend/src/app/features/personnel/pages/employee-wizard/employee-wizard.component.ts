import { Component, signal, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, ActivatedRoute } from '@angular/router';
import { StepperModule } from 'primeng/stepper';
import { ButtonModule } from 'primeng/button';
import { ToastModule } from 'primeng/toast';
import { MessageService } from 'primeng/api';
import { CreateEmployeeDto } from '../../models/create-employee.dto';
import { EmployeeService } from '../../services/employee.service';

// Steps
import { PersonalInfoStepComponent } from '../../components/wizard-steps/personal-info/personal-info-step.component';
import { JobFinancialStepComponent } from '../../components/wizard-steps/job-financial/job-financial-step.component';
import { AddressesStepComponent } from '../../components/wizard-steps/addresses-contact/addresses-step.component';
import { QualificationsStepComponent } from '../../components/wizard-steps/qualifications-experience/qualifications-step.component';
import { DocumentsStepComponent } from '../../components/wizard-steps/documents-contracts/documents-step.component';
import { FamilyStepComponent } from '../../components/wizard-steps/family-preferences/family-step.component';

@Component({
  selector: 'app-employee-wizard',
  standalone: true,
  imports: [
    CommonModule,
    StepperModule,
    ButtonModule,
    ToastModule,
    PersonalInfoStepComponent,
    JobFinancialStepComponent,
    AddressesStepComponent,
    QualificationsStepComponent,
    DocumentsStepComponent,
    FamilyStepComponent
  ],
  providers: [MessageService],
  templateUrl: './employee-wizard.component.html',
  styleUrls: ['./employee-wizard.component.scss']
})
export class EmployeeWizardComponent implements OnInit {
  private employeeService = inject(EmployeeService);
  private messageService = inject(MessageService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);

  // Wizard State
  currentStep: number = 0;
  loading = signal(false);
  editMode = signal(false);
  employeeId = signal<number | null>(null);

  // Form Data State
  employeeData = signal<CreateEmployeeDto>({
    employeeNumber: '',
    firstNameAr: '', secondNameAr: '', thirdNameAr: '', lastNameAr: '',
    fullNameEn: '', birthDate: new Date(), gender: 'M',
    mobile: '', nationalityId: 0, nationalId: '',
    
    // Default Nulls for safety
    departmentId: 0, jobId: 0, hireDate: new Date(),
    
    basicSalary: 0, housingAllowance: 0, transportAllowance: 0, medicalAllowance: 0,
    
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

  ngOnInit() {
    this.route.paramMap.subscribe(params => {
      const id = params.get('id');
      if (id) {
        this.editMode.set(true);
        this.employeeId.set(+id);
        this.loadEmployeeData(+id);
      }
    });
  }

  loadEmployeeData(id: number) {
    this.loading.set(true);
    this.employeeService.getFullProfile(id).subscribe({
      next: (profile) => {
        const core = profile.coreProfile;
        const comp = profile.compensation;

        this.employeeData.update(current => ({
          ...current,
          // Personal
          employeeNumber: core.employeeNumber,
          firstNameAr: core.firstNameAr || '',
          secondNameAr: core.secondNameAr || '',
          thirdNameAr: core.thirdNameAr || '',
          lastNameAr: core.hijriLastNameAr || '', // Mapping check
          fullNameEn: core.fullNameEn,
          birthDate: core.birthDate ? new Date(core.birthDate) : new Date(),
          gender: core.gender || 'M',
          mobile: core.mobile,
          email: core.email,
          nationalityId: core.nationalityId || 0,
          nationalId: core.nationalId || '',
          maritalStatus: core.maritalStatus, // If exists in DTO
          
          // Job
          departmentId: core.deptId || 0,
          jobId: core.jobId || 0,
          hireDate: core.hireDate ? new Date(core.hireDate) : new Date(),
          managerId: 0, // Not in CoreProfile directly, might be in mapping or need separate fetch if critical

          // Financial
          basicSalary: comp?.basicSalary || 0,
          housingAllowance: comp?.housingAllowance || 0,
          transportAllowance: comp?.transportAllowance || 0,
          medicalAllowance: comp?.medicalAllowance || 0,

          // Collections
          qualifications: profile.qualifications || [],
          experiences: profile.experiences || [],
          emergencyContacts: profile.emergencyContacts || [],
          contracts: profile.contracts || [],
          certifications: profile.certifications || [],
          bankAccounts: profile.bankAccounts || [],
          dependents: profile.dependents || [],
          addresses: profile.addresses || [],
          documents: profile.documents || []
        }));
        this.loading.set(false);
      },
      error: (err) => {
        console.error('Failed to load employee', err);
        this.messageService.add({ severity: 'error', summary: 'خطأ', detail: 'فشل تحميل بيانات الموظف' });
        this.loading.set(false);
      }
    });
  }

  // --- Step Handlers ---

  updateData(data: Partial<CreateEmployeeDto>) {
    this.employeeData.update(current => ({ ...current, ...data }));
  }

  updateProfilePic(file: File) {
    this.profilePictureFile.set(file);
  }

  updateDocFiles(files: Map<number, File>) {
      // Merge maps or replace? Replace for now as component emits full state usually
      this.documentFiles.set(files);
  }

  nextStep() {
    this.currentStep++;
  }

  prevStep() {
    this.currentStep--;
  }

  // --- Final Submit ---
  submit() {
    const d = this.employeeData();
    // Final check
    if (!d.firstNameAr || !d.nationalId) {
         this.messageService.add({ severity: 'error', summary: 'خطأ', detail: 'يرجى التأكد من البيانات الأساسية' });
         return;
    }

    // Sync Financial Info to Contracts Array (Backend Matching)
    // User requested "Employment Contract" to be added matching backend structure.
    // We create a default contract based on the entered financial data.
    if (!d.contracts || d.contracts.length === 0) {
      d.contracts = [{
        contractType: 'FULL_TIME', // Default matching DTO comment
        startDate: d.hireDate,
        endDate: new Date(new Date(d.hireDate).setFullYear(new Date(d.hireDate).getFullYear() + 1)), // Default 1 year
        isRenewable: true,
        basicSalary: d.basicSalary,
        housingAllowance: d.housingAllowance,
        transportAllowance: d.transportAllowance,
        otherAllowances: d.medicalAllowance, // Mapping medical to other for now
        vacationDays: 30, // Default
        workingHoursDaily: 8 // Default
      }];
    }

    // Fix Emergency Contacts IsPrimary type (Backend expects byte 0/1, Frontend uses boolean)
    if (d.emergencyContacts && d.emergencyContacts.length > 0) {
        // Create a deep copy to avoid mutating the signal state directly in a way that affects UI if submission fails
        d.emergencyContacts = d.emergencyContacts.map(c => ({
            ...c,
            isPrimary: (c.isPrimary === true || c.isPrimary === 1) ? 1 : 0
        }));
    }

    this.loading.set(true);
    const action = this.editMode() ? 'تحديث' : 'حفظ';
    this.messageService.add({ severity: 'info', summary: `جاري ${action}`, detail: `جاري ${action} ملف الموظف...`, sticky: true });
    
    console.log('Final Payload:', JSON.stringify(d, null, 2)); // Log payload for debugging

    const request = this.editMode() && this.employeeId() 
        ? this.employeeService.update(this.employeeId()!, d)
        : this.employeeService.create(d);

    (request as any).subscribe({
      next: (response: any) => {
        // Create returns ID, Update returns void. If update, use existing ID.
        const id = this.editMode() ? this.employeeId()! : response;
        this.handleUploads(id);
      },
      error: (err: any) => {
        this.loading.set(false);
        console.error('Submission Error:', err);

        let errorMessage = 'حدث خطأ أثناء حفظ البيانات';
        if (err.status === 400 && err.error?.errors) {
            const validationErrors = [];
            for (const key in err.error.errors) {
                if (err.error.errors.hasOwnProperty(key)) {
                    validationErrors.push(`${key}: ${err.error.errors[key].join(', ')}`);
                }
            }
            errorMessage = 'خطأ في التحقق: ' + validationErrors.join(' | ');
            console.error('Validation Errors:', validationErrors);
        }

        this.messageService.clear();
        this.messageService.add({ severity: 'error', summary: 'فشل الحفظ', detail: errorMessage, life: 10000 });
      }
    });
  }

  private handleUploads(employeeId: number) {
    const uploadTasks: Promise<any>[] = [];

    // 1. Profile Pic
    const photo = this.profilePictureFile();
    if (photo) {
      uploadTasks.push(this.employeeService.uploadProfilePicture(employeeId, photo).toPromise());
    }

    // 2. Documents
    const docs = this.employeeData().documents;
    const docFiles = this.documentFiles();
    
    if (docs) {
        docs.forEach((doc, index) => {
            const file = docFiles.get(index);
            if (file && doc.documentTypeId) {
                const expiry = doc.expiryDate ? new Date(doc.expiryDate).toISOString() : undefined;
                uploadTasks.push(
                    this.employeeService.uploadDocument(employeeId, file, doc.documentTypeId, doc.documentNumber, expiry).toPromise()
                );
            }
        });
    }

    Promise.allSettled(uploadTasks).then((results) => {
      this.loading.set(false);
      this.messageService.clear();
      this.messageService.add({ severity: 'success', summary: 'تم بنجاح', detail: 'تم حفظ بيانات الموظف بنجاح' });
      setTimeout(() => this.router.navigate(['/personnel', employeeId]), 1000);
    });
  }
}
