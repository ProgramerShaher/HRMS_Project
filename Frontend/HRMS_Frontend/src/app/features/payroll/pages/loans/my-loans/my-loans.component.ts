import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { TableModule } from 'primeng/table';
import { TagModule } from 'primeng/tag';
import { SelectModule } from 'primeng/select';
import { FormsModule } from '@angular/forms';
import { LoanService } from '../../../services/loan.service';
import { Loan, LoanStatus } from '../../../models/loan.models';
import { AuthService } from '../../../../../core/auth/services/auth.service';

/**
 * شاشة سلفي
 * My Loans Component
 */
@Component({
  selector: 'app-my-loans',
  standalone: true,
  imports: [CommonModule, ButtonModule, TableModule, TagModule, SelectModule, FormsModule],
  templateUrl: './my-loans.component.html',
  styleUrls: ['./my-loans.component.scss']
})
export class MyLoansComponent implements OnInit {
  private loanService = inject(LoanService);
  private authService = inject(AuthService);
  private router = inject(Router);

  loans = signal<Loan[]>([]);
  filteredLoans = signal<Loan[]>([]);
  loading = signal(false);
  
  selectedStatus: string = 'ALL';
  statusOptions = [
    { label: 'الكل', value: 'ALL' },
    { label: 'قيد الانتظار', value: 'PENDING' },
    { label: 'نشط', value: 'ACTIVE' },
    { label: 'مسدد', value: 'SETTLED' },
    { label: 'مغلق', value: 'CLOSED' }
  ];

  ngOnInit() {
    this.loadLoans();
  }

  loadLoans() {
    const employeeId = this.authService.currentUser()?.employeeId;
    if (!employeeId) return;

    this.loading.set(true);
    this.loanService.getEmployeeLoans(employeeId).subscribe({
      next: (response) => {
        if (response.succeeded && response.data) {
          this.loans.set(response.data);
          this.applyFilter();
        }
        this.loading.set(false);
      },
      error: (err) => {
        console.error('Error loading loans:', err);
        this.loading.set(false);
      }
    });
  }

  applyFilter() {
    const allLoans = this.loans();
    if (this.selectedStatus === 'ALL') {
      this.filteredLoans.set(allLoans);
    } else {
      this.filteredLoans.set(allLoans.filter(l => l.status === this.selectedStatus));
    }
  }

  onStatusChange() {
    this.applyFilter();
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
    switch (status) {
      case 'ACTIVE': return 'نشط';
      case 'PENDING': return 'قيد الانتظار';
      case 'SETTLED': return 'مسدد';
      case 'CLOSED': return 'مغلق';
      default: return status;
    }
  }

  formatCurrency(value: number): string {
    return new Intl.NumberFormat('ar-YE', {
      style: 'decimal',
      minimumFractionDigits: 0,
      maximumFractionDigits: 0
    }).format(value);
  }

  formatDate(date: Date | string | undefined): string {
    if (!date) return '-';
    return new Date(date).toLocaleDateString('ar-YE', {
      year: 'numeric',
      month: 'long',
      day: 'numeric'
    });
  }

  calculateProgress(loan: Loan): number {
    if (loan.loanAmount === 0) return 0;
    return Math.round((loan.paidAmount / loan.loanAmount) * 100);
  }

  navigateToNewLoan() {
    this.router.navigate(['/payroll/loans/new']);
  }

  viewLoanDetails(loan: Loan) {
    this.router.navigate(['/payroll/loans', loan.loanId]);
  }
}
