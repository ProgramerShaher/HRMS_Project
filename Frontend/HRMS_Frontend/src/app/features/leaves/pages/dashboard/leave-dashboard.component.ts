import { Component, OnInit, signal, effect, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { Chart, registerables } from 'chart.js';
import { LeaveRequestService } from '../../services/leave-request.service';
import { LeaveDashboardStats } from '../../models/leave.models';
import { AuthService } from '../../../../core/auth/services/auth.service';

Chart.register(...registerables);

@Component({
  selector: 'app-leave-dashboard',
  standalone: true,
  imports: [CommonModule, ButtonModule],
  templateUrl: './leave-dashboard.component.html',
  styles: [`
    :host { display: block; }
    canvas { max-height: 250px; width: 100% !important; }
  `]
})
export class LeaveDashboardComponent implements OnInit, OnDestroy {
  stats = signal<LeaveDashboardStats | null>(null);
  loading = signal(false);
  private chart: Chart | null = null;

  constructor(
    private leaveRequestService: LeaveRequestService,
    private authService: AuthService,
    private router: Router
  ) {
    // Re-render chart when stats update
    effect(() => {
      const currentStats = this.stats();
      if (currentStats) {
        this.renderChart(currentStats);
      }
    });
  }

  ngOnInit() {
    this.loadStats();
  }

  ngOnDestroy() {
    if (this.chart) {
      this.chart.destroy();
    }
  }

  loadStats() {
    const employeeId = this.authService.currentUser()?.employeeId;
    if (!employeeId) return;

    this.loading.set(true);
    this.leaveRequestService.getEmployeeStats(employeeId).subscribe({
      next: (res) => {
        if (res.succeeded) {
          this.stats.set(res.data);
        }
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }

  calculatePercentage(part: number | undefined, total: number | undefined): number {
    if (!part || !total || total === 0) return 0;
    return Math.round((part / total) * 100);
  }

  calculateAcceptanceRate(): number {
    const s = this.stats();
    if (!s) return 0;
    const totalProcessed = s.approvedRequestsCount + s.rejectedRequestsCount;
    if (totalProcessed === 0) return 0;
    return Math.round((s.approvedRequestsCount / totalProcessed) * 100);
  }

  navigateToNewRequest() {
    this.router.navigate(['/leaves/my-leaves']);
  }

  private renderChart(stats: LeaveDashboardStats) {
    const ctx = document.getElementById('leaveDistributionChart') as HTMLCanvasElement;
    if (!ctx) return;

    if (this.chart) {
      this.chart.destroy();
    }

    const labels = stats.leaveTypeSummaries.map(s => s.leaveTypeNameAr);
    const data = stats.leaveTypeSummaries.map(s => s.consumedDays);

    this.chart = new Chart(ctx, {
      type: 'doughnut',
      data: {
        labels: labels,
        datasets: [{
          data: data,
          backgroundColor: [
            '#3b82f6', // blue
            '#10b981', // emerald
            '#f59e0b', // amber
            '#8b5cf6', // violet
            '#ef4444'  // red
          ],
          borderWidth: 0,
          spacing: 5
        }]
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,
        plugins: {
          legend: {
            position: 'bottom',
            labels: {
              usePointStyle: true,
              padding: 20,
              font: {
                family: 'Cairo',
                size: 12
              }
            }
          }
        },
        cutout: '70%'
      }
    });
  }
}
