import { Routes } from '@angular/router';
import { EmployeesListComponent } from './pages/employees-list/employees-list.component';
import { EmployeeWizardComponent } from './pages/employee-wizard/employee-wizard.component';
import { EmployeeProfileComponent } from './pages/employee-profile/employee-profile.component';
import { permissionGuard } from '../../core/auth/guards/permission.guard';

export const PERSONNEL_ROUTES: Routes = [
    {
        path: '',
        component: EmployeesListComponent,
        canActivate: [permissionGuard(['Employees.View'])],
        data: { title: 'الموظفين' }
    },
    {
        path: 'new',
        component: EmployeeWizardComponent,
        canActivate: [permissionGuard(['Employees.Create'])],
        data: { title: 'إضافة موظف جديد' }
    },
    {
        path: 'wizard',
        component: EmployeeWizardComponent,
        canActivate: [permissionGuard(['Employees.Create'])],
        data: { title: 'إضافة موظف جديد' }
    },
    {
        path: ':id',
        component: EmployeeProfileComponent,
        canActivate: [permissionGuard(['Employees.View'])],
        data: { title: 'ملف الموظف' }
    },
    {
        path: 'edit/:id',
        component: EmployeeWizardComponent,
        canActivate: [permissionGuard(['Employees.Edit'])],
        data: { title: 'تعديل بيانات الموظف' }
    }
];
