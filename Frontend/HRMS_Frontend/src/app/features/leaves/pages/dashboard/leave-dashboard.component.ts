import { Component, OnInit, OnDestroy, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { TagModule } from 'primeng/tag';
import { Chart, registerables } from 'chart.js';
import { LeaveRequestService } from '../../services/leave-request.service';
import { LeaveDashboardStats, LeaveRequest } from '../../models/leave.models';

Chart.register(...registerables);

@Component({
  selector: 'app-leave-dashboard',
  standalone: true,
  imports: [CommonModule, ButtonModule, TagModule],
  templateUrl: './leave-dashboard.component.html',
  styles: [`:host { display: block; } canvas { max-height: 280px; width: 100% !important; }`]
})
export class LeaveDashboardComponent implements OnInit, OnDestroy {
  private service = inject(LeaveRequestService);
  private router = inject(Router);

  stats = signal<LeaveDashboardStats | null>(null);
  recentPending = signal<LeaveRequest[]>([]);
  loading = signal(false);
  private chart: Chart | null = null;

  ngOnInit() { this.load(); }

  ngOnDestroy() { this.chart?.destroy(); }

  load() {
    this.loading.set(true);
    // Manager stats — organization-wide
    this.service.getManagerStats().subscribe({
      next: (res) => {
        if (res.succeeded) {
          this.stats.set(res.data);
          setTimeout(() => this.renderChart(res.data), 100);
        }
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
    // Load 5 latest pending requests for quick preview
    this.service.getPendingRequests().subscribe({
      next: (res) => {
        if (res.succeeded) {
          this.recentPending.set((res.data || []).slice(0, 5));
        }
      }
    });
  }

  private renderChart(stats: LeaveDashboardStats) {
    const ctx = document.getElementById('leaveTypeChart') as HTMLCanvasElement;
    if (!ctx) return;
    this.chart?.destroy();

    const summaries = stats.leaveTypeSummaries || [];
    const labels  = summaries.map(s => s.leaveTypeNameAr);
    const data    = summaries.map(s => s.consumedDays);
    const colors  = ['#3b82f6','#10b981','#f59e0b','#8b5cf6','#ef4444','#06b6d4'];

    this.chart = new Chart(ctx, {
      type: 'doughnut',
      data: {
        labels,
        datasets: [{
          data,
          backgroundColor: colors,
          borderWidth: 0,
          spacing: 4,
          hoverOffset: 10
        }]
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,
        cutout: '72%',
        plugins: {
          legend: {
            position: 'bottom',
            labels: {
              usePointStyle: true,
              padding: 20,
              font: { family: 'Cairo', size: 12 }
            }
          }
        }
      }
    });
  }

  getStatusSeverity(status: string): 'success' | 'warn' | 'danger' | 'info' {
    const map: Record<string, any> = {
      PENDING:          'warn',
      MANAGER_APPROVED: 'info',
      HR_APPROVED:      'info',
      APPROVED:         'success',
      REJECTED:         'danger',
      CANCELLED:        'danger'
    };
    return map[status] ?? 'info';
  }

  getStatusLabel(status: string): string {
    const labels: Record<string, string> = {
      PENDING:          'معلق',
      MANAGER_APPROVED: 'معتمد من المدير',
      HR_APPROVED:      'معتمد من HR',
      APPROVED:         'معتمد',
      REJECTED:         'مرفوض',
      CANCELLED:        'ملغي'
    };
    return labels[status] ?? status;
  }

  navigateTo(path: string) { this.router.navigate(['/leaves', path]); }
}
