import { Component, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { StepperModule } from 'primeng/stepper';
import { ButtonModule } from 'primeng/button';
import { ToastModule } from 'primeng/toast';
import { MessageService } from 'primeng/api';
import { CreateEmployeeDto } from '../../models/create-employee.dto';
import { EmployeeService } from '../../services/employee.service';

// Steps (Will be implemented next)
import { PersonalInfoStepComponent } from '../../components/wizard-steps/personal-info/personal-info-step.component';
import { EmploymentInfoStepComponent } from '../../components/wizard-steps/employment-info/employment-info-step.component';
import { FinancialInfoStepComponent } from '../../components/wizard-steps/financial-info/financial-info-step.component';

@Component({
  selector: 'app-employee-wizard',
  standalone: true,
  imports: [
    CommonModule,
    StepperModule,
    ButtonModule,
    ToastModule,
    PersonalInfoStepComponent,
    EmploymentInfoStepComponent,
    FinancialInfoStepComponent
  ],
  providers: [MessageService],
  templateUrl: './employee-wizard.component.html',
  styleUrls: ['./employee-wizard.component.scss']
})
export class EmployeeWizardComponent {
  private employeeService = inject(EmployeeService);
  private messageService = inject(MessageService);
  private router = inject(Router);

  // Wizard State
  currentStep: number = 0;
  loading = signal(false);

  // Form Data State (Shared across steps)
  employeeData = signal<CreateEmployeeDto>({
    // Initial Empty State
    employeeNumber: '',
    firstNameAr: '', secondNameAr: '', thirdNameAr: '', lastNameAr: '',
    fullNameEn: '',
    birthDate: new Date(),
    gender: 'Male',
    mobile: '',
    nationalityId: 0,
    nationalId: '',
    
    departmentId: 0,
    jobId: 0,
    hireDate: new Date(),
    
    basicSalary: 0,
    housingAllowance: 0,
    transportAllowance: 0,
    medicalAllowance: 0,
    
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

  // Handlers for Step Updates
  updatePersonalInfo(data: Partial<CreateEmployeeDto>) {
    this.employeeData.update(current => ({ ...current, ...data }));
  }

  updateEmploymentInfo(data: Partial<CreateEmployeeDto>) {
    this.employeeData.update(current => ({ ...current, ...data }));
  }

  updateFinancialInfo(data: Partial<CreateEmployeeDto>) {
    this.employeeData.update(current => ({ ...current, ...data }));
  }

  // Final Submit
  submit() {
    this.loading.set(true);
    this.employeeService.create(this.employeeData()).subscribe({
      next: (id) => {
        this.messageService.add({ severity: 'success', summary: 'تم الحفظ', detail: 'تم إنشاء ملف الموظف بنجاح' });
        setTimeout(() => this.router.navigate(['/employees', id]), 1500); // Go to Profile
      },
      error: (err) => {
        this.loading.set(false);
        this.messageService.add({ severity: 'error', summary: 'خطأ', detail: 'حدث خطأ أثناء حفظ البيانات' });
        console.error(err);
      }
    });
  }
}
