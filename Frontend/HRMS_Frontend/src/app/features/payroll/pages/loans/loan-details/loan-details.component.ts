import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { TableModule } from 'primeng/table';
import { TagModule } from 'primeng/tag';
import { LoanService } from '../../../services/loan.service';
import { Loan, LoanInstallment } from '../../../models/loan.models';

@Component({
  selector: 'app-loan-details',
  standalone: true,
  imports: [CommonModule, ButtonModule, TableModule, TagModule],
  templateUrl: './loan-details.component.html',
  styleUrls: ['./loan-details.component.scss']
})
export class LoanDetailsComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private loanService = inject(LoanService);

  loan = signal<Loan | null>(null);

  ngOnInit() {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    if (id) {
      this.loanService.getLoanById(id).subscribe({
        next: (res) => {
          if (res.succeeded && res.data) {
            this.loan.set(res.data);
          }
        }
      });
    }
  }

  formatCurrency(value: number): string {
    return new Intl.NumberFormat('ar-YE', { style: 'decimal' }).format(value);
  }

  formatDate(date: Date | string | undefined): string {
    if (!date) return '-';
    return new Date(date).toLocaleDateString('ar-YE');
  }

  getStatusLabel(status: string): string {
    const labels: any = { 'ACTIVE': 'نشط', 'PENDING': 'قيد الانتظار', 'SETTLED': 'مسدد', 'CLOSED': 'مغلق' };
    return labels[status] || status;
  }

  getStatusSeverity(status: string): any {
    const severities: any = { 'ACTIVE': 'success', 'PENDING': 'warn', 'SETTLED': 'info', 'CLOSED': 'danger' };
    return severities[status] || 'info';
  }

  goBack() {
    this.router.navigate(['/payroll/loans/my-loans']);
  }
}
