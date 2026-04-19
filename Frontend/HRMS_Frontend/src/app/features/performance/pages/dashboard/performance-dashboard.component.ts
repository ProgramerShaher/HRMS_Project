import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

// PrimeNG
import { CardModule } from 'primeng/card';
import { ButtonModule } from 'primeng/button';
import { ProgressBarModule } from 'primeng/progressbar';

import { PerformanceService } from '../../services/performance.service';
import { EmployeeAppraisal, EmployeeViolation } from '../../models/performance.model';

@Component({
    selector: 'app-performance-dashboard',
    standalone: true,
    imports: [CommonModule, RouterModule, CardModule, ButtonModule, ProgressBarModule],
    templateUrl: './performance-dashboard.component.html'
})
export class PerformanceDashboardComponent implements OnInit {
    private performanceService = inject(PerformanceService);

    totalAppraisals = signal(0);
    completedAppraisals = signal(0);
    averageScore = signal(0);
    
    totalViolations = signal(0);
    pendingViolations = signal(0);
    totalDeductionDays = signal(0);

    loading = signal(true);

    ngOnInit() {
        this.loadDashboardData();
    }

    loadDashboardData() {
        this.loading.set(true);
        // We fetch everything assuming admin level. In a real system, there should be a bounded Dashboard API mapping the user's role.
        let appraisalsFinished = false;
        let violationsFinished = false;

        this.performanceService.getAppraisals().subscribe(res => {
            if (res.succeeded) {
                const list = res.data || [];
                this.totalAppraisals.set(list.length);
                const completed = list.filter(a => a.status === 'COMPLETED');
                this.completedAppraisals.set(completed.length);
                
                if (completed.length > 0) {
                    const sum = completed.reduce((acc, curr) => acc + (curr.finalScore || 0), 0);
                    this.averageScore.set(Math.round(sum / completed.length));
                }
            }
            appraisalsFinished = true;
            if (violationsFinished) this.loading.set(false);
        });

        this.performanceService.getViolations().subscribe(res => {
            if (res.succeeded) {
                const list = res.data || [];
                this.totalViolations.set(list.length);
                this.pendingViolations.set(list.filter(v => v.status === 'PENDING').length);
                
                const approved = list.filter(v => v.status === 'APPROVED');
                const sumDays = approved.reduce((acc, curr) => acc + (curr.deductionDays || 0), 0);
                this.totalDeductionDays.set(sumDays);
            }
            violationsFinished = true;
            if (appraisalsFinished) this.loading.set(false);
        });
    }
}
