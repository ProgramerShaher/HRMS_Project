import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';

// PrimeNG
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { ToastModule } from 'primeng/toast';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { MessageService, ConfirmationService } from 'primeng/api';
import { TagModule } from 'primeng/tag';
import { TooltipModule } from 'primeng/tooltip';

import { PerformanceService } from '../../services/performance.service';
import { EmployeeAppraisal } from '../../models/performance.model';
import { AuthService } from '../../../../core/auth/services/auth.service';

@Component({
  selector: 'app-team-appraisals',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    TableModule,
    ButtonModule,
    InputTextModule,
    ToastModule,
    ConfirmDialogModule,
    TagModule,
    TooltipModule
  ],
  providers: [MessageService, ConfirmationService],
  templateUrl: './team-appraisals.component.html'
})
export class TeamAppraisalsComponent implements OnInit {
  private performanceService = inject(PerformanceService);
  private messageService = inject(MessageService);
  public router = inject(Router);

  appraisals = signal<EmployeeAppraisal[]>([]);
  loading = signal<boolean>(false);

  ngOnInit() {
    this.loadData();
  }

  loadData() {
    this.loading.set(true);
    this.performanceService.getAppraisals().subscribe({
      next: (res) => {
        this.loading.set(false);
        if (res.succeeded) {
          this.appraisals.set(res.data);
        }
      },
      error: () => this.loading.set(false)
    });
  }

  openReview(appraisal: EmployeeAppraisal) {
    this.router.navigate(['/performance/appraisals', appraisal.appraisalId, 'execute']);
  }

  viewResult(appraisal: EmployeeAppraisal) {
    this.router.navigate(['/performance/appraisals', appraisal.appraisalId, 'result']);
  }
}
