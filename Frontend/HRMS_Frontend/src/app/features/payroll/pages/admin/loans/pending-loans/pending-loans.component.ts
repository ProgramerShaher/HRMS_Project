import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { TableModule } from 'primeng/table';
import { TagModule } from 'primeng/tag';
import { SelectModule } from 'primeng/select';
import { DialogModule } from 'primeng/dialog';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { ConfirmationService, MessageService } from 'primeng/api';
import { ToastModule } from 'primeng/toast';
import { LoanService } from '../../../../services/loan.service';
import { Loan, LoanStatus } from '../../../../models/loan.models';
import { ActionButtonsComponent } from '../../../../../../shared/components/action-buttons/action-buttons.component';

/**
 * السلف المعلقة - واجهة إدارية
 * Pending Loans - Admin Interface
 */
@Component({
  selector: 'app-pending-loans',
  standalone: true,
  imports: [
    CommonModule, FormsModule, ButtonModule, TableModule, TagModule,
    SelectModule, DialogModule, ConfirmDialogModule, ToastModule,
    ActionButtonsComponent
  ],
  providers: [ConfirmationService, MessageService],
  templateUrl: './pending-loans.component.html',
  styleUrl: 'pending-loans.component.scss'
})
export class PendingLoansComponent implements OnInit {
  private loanService = inject(LoanService);
  private router = inject(Router);
  private confirmationService = inject(ConfirmationService);
  private messageService = inject(MessageService);

  pendingLoans = signal<Loan[]>([]);
  loading = signal(false);

  // Dialog
  showApprovalDialog = signal(false);
  selectedLoan: Loan | null = null;
  approvalNotes = '';

  ngOnInit() {
    this.loadPendingLoans();
  }

  loadPendingLoans() {
    this.loading.set(true);
    
    this.loanService.getAllLoans({ status: 'PENDING' }).subscribe({
      next: (response: any) => {
        let data = [];
        if (Array.isArray(response)) {
          data = response;
        } else if (response?.data) {
          data = Array.isArray(response.data) ? response.data : (response.data.items || []);
        }
        
        if (Array.isArray(data)) {
          this.pendingLoans.set(data);
        }
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
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
          this.loadPendingLoans();
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
              this.loadPendingLoans();
            }
          }
        });
      }
    });
  }

  formatCurrency(value: number): string {
    return new Intl.NumberFormat('ar-YE', { style: 'decimal' }).format(value);
  }

  formatDate(date: Date | string | undefined): string {
    if (!date) return '-';
    return new Date(date).toLocaleDateString('ar-YE');
  }

  calculateMonthlyInstallment(loan: Loan): number {
    return loan.loanAmount / loan.installmentCount;
  }
}
