import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { TableModule } from 'primeng/table';
import { TagModule } from 'primeng/tag';
import { SelectModule } from 'primeng/select';
import { InputTextModule } from 'primeng/inputtext';
import { DatePickerModule } from 'primeng/datepicker';
import { DialogModule } from 'primeng/dialog';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { ConfirmationService, MessageService } from 'primeng/api';
import { ToastModule } from 'primeng/toast';
import { LoanService } from '../../../../services/loan.service';
import { Loan, LoanStatus } from '../../../../models/loan.models';
import { EmployeeSelectorComponent } from '../../../../../../shared/components/employee-selector/employee-selector.component';
import { ActionButtonsComponent } from '../../../../../../shared/components/action-buttons/action-buttons.component';

@Component({
  selector: 'app-all-loans-admin',
  standalone: true,
  imports: [
    CommonModule, FormsModule, ButtonModule, TableModule, TagModule,
    SelectModule, InputTextModule, DatePickerModule, DialogModule,
    ConfirmDialogModule, ToastModule, EmployeeSelectorComponent,
    ActionButtonsComponent
  ],
  providers: [ConfirmationService, MessageService],
  templateUrl: './all-loans-admin.component.html',
  styleUrls: ['./all-loans-admin.component.scss']
})
export class AllLoansAdminComponent implements OnInit {
  private loanService = inject(LoanService);
  private router = inject(Router);
  private confirmationService = inject(ConfirmationService);
  private messageService = inject(MessageService);

  loans = signal<Loan[]>([]);
  filteredLoans = signal<Loan[]>([]);
  loading = signal(false);

  // Filters
  selectedStatus: string | null = null;
  selectedEmployeeId: number | null = null;
  searchTerm = '';
  dateFrom: Date | null = null;
  dateTo: Date | null = null;

  statusOptions = [
    { label: 'الكل', value: null },
    { label: 'قيد الانتظار', value: 'PENDING' },
    { label: 'نشط', value: 'ACTIVE' },
    { label: 'مسدد', value: 'SETTLED' },
    { label: 'مغلق', value: 'CLOSED' }
  ];

  // Dialog
  showApprovalDialog = signal(false);
  selectedLoan: Loan | null = null;
  approvalNotes = '';

  ngOnInit() {
    this.loadAllLoans();
  }

  loadAllLoans() {
    this.loading.set(true);
    const filters = {
      status: this.selectedStatus || undefined,
      employeeId: this.selectedEmployeeId || undefined,
      departmentId: undefined,
    };

    this.loanService.getAllLoans(filters).subscribe({
      next: (response: any) => {
        let data = [];
        if (Array.isArray(response)) {
          data = response;
        } else if (response?.data) {
          data = Array.isArray(response.data) ? response.data : (response.data.items || []);
        }
        
        if (Array.isArray(data)) {
          this.loans.set(data);
          this.filteredLoans.set(data);
        }
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }

  applyFilters() {
    this.loadAllLoans(); // This is fine if we don't call it inside loadAllLoans
  }

  clearFilters() {
    this.selectedStatus = null;
    this.selectedEmployeeId = null;
    this.searchTerm = '';
    this.dateFrom = null;
    this.dateTo = null;
    this.loadAllLoans();
  }

  viewDetails(loan: Loan) {
    this.router.navigate(['/payroll/loans/details', loan.loanId]);
  }

  openApprovalDialog(loan: Loan) {
    this.selectedLoan = loan;
    this.approvalNotes = '';
    this.showApprovalDialog.set(true);
  }

  approveLoan() {
    if (!this.selectedLoan) return;

    this.loanService.changeLoanStatus({
      loanId: this.selectedLoan.loanId,
      newStatus: 'ACTIVE'
    }).subscribe({
      next: (response) => {
        if (response.succeeded) {
          this.messageService.add({
            severity: 'success',
            summary: 'تمت الموافقة',
            detail: 'تمت الموافقة على السلفة بنجاح'
          });
          this.showApprovalDialog.set(false);
          this.loadAllLoans();
        }
      },
      error: () => {
        this.messageService.add({
          severity: 'error',
          summary: 'خطأ',
          detail: 'فشلت عملية الموافقة'
        });
      }
    });
  }

  rejectLoan() {
    if (!this.selectedLoan) return;

    this.confirmationService.confirm({
      message: 'هل أنت متأكد من رفض هذه السلفة؟',
      header: 'تأكيد الرفض',
      icon: 'pi pi-exclamation-triangle',
      acceptLabel: 'نعم، رفض',
      rejectLabel: 'إلغاء',
      acceptButtonStyleClass: 'p-button-danger',
      accept: () => {
        this.loanService.changeLoanStatus({
          loanId: this.selectedLoan!.loanId,
          newStatus: 'CLOSED'
        }).subscribe({
          next: (response) => {
            if (response.succeeded) {
              this.messageService.add({
                severity: 'info',
                summary: 'تم الرفض',
                detail: 'تم رفض السلفة'
              });
              this.showApprovalDialog.set(false);
              this.loadAllLoans();
            }
          }
        });
      }
    });
  }

  settleLoan(loan: Loan) {
    this.confirmationService.confirm({
      message: 'هل تريد تسوية هذه السلفة مبكراً؟',
      header: 'تسوية مبكرة',
      icon: 'pi pi-question-circle',
      acceptLabel: 'نعم، تسوية',
      rejectLabel: 'إلغاء',
      accept: () => {
        this.loanService.earlySettlement(loan.loanId, 'تسوية مبكرة من الإدارة').subscribe({
          next: (response) => {
            if (response.succeeded) {
              this.messageService.add({
                severity: 'success',
                summary: 'تمت التسوية',
                detail: 'تمت تسوية السلفة بنجاح'
              });
              this.loadAllLoans();
            }
          }
        });
      }
    });
  }

  getStatusSeverity(status: LoanStatus): 'success' | 'info' | 'warn' | 'danger' {
    switch (status) {
      case 'ACTIVE': return 'success';
      case 'PENDING': return 'warn';
      case 'SETTLED': return 'info';
      case 'CLOSED': return 'danger';
      default: return 'info';
    }
  }

  getStatusLabel(status: LoanStatus): string {
    const labels: Record<LoanStatus, string> = {
      'ACTIVE': 'نشط',
      'PENDING': 'قيد الانتظار',
      'SETTLED': 'مسدد',
      'CLOSED': 'مغلق'
    };
    return labels[status] || status;
  }

  formatCurrency(value: number): string {
    return new Intl.NumberFormat('ar-YE', { style: 'decimal' }).format(value);
  }

  formatDate(date: Date | string | undefined): string {
    if (!date) return '-';
    return new Date(date).toLocaleDateString('ar-YE');
  }

  calculateProgress(loan: Loan): number {
    if (loan.loanAmount === 0) return 0;
    return Math.round((loan.paidAmount / loan.loanAmount) * 100);
  }

  goBack() {
    this.router.navigate(['/payroll/dashboard']);
  }

  createNewLoan() {
    this.router.navigate(['/payroll/loans/admin/create']);
  }
}
