import { Routes } from '@angular/router';
import { EmployeesListComponent } from './pages/employees-list/employees-list.component';
import { EmployeeWizardComponent } from './pages/employee-wizard/employee-wizard.component';
import { EmployeeProfileComponent } from './pages/employee-profile/employee-profile.component';

export const PERSONNEL_ROUTES: Routes = [
    { 
        path: '', 
        component: EmployeesListComponent,
        data: { title: 'الموظفين' }
    },
    { 
        path: 'new', 
        component: EmployeeWizardComponent,
        data: { title: 'إضافة موظف جديد' }
    },
    { 
        path: ':id', 
        component: EmployeeProfileComponent,
        data: { title: 'ملف الموظف' }
    },
    { 
        path: 'edit/:id', 
        component: EmployeeWizardComponent,
        data: { title: 'تعديل بيانات الموظف' }
    }
];
