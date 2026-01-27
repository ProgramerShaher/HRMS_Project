import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ChartModule } from 'primeng/chart';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, ChartModule],
  templateUrl: './dashboard.component.html'
})
export class DashboardComponent implements OnInit {
  data: any;
  options: any;

  ngOnInit() {
      const documentStyle = getComputedStyle(document.documentElement);
      const textColor = documentStyle.getPropertyValue('--text-color');
      const textColorSecondary = documentStyle.getPropertyValue('--text-color-secondary');
      const surfaceBorder = documentStyle.getPropertyValue('--surface-border');

      this.data = {
          labels: ['يناير', 'فبراير', 'مارس', 'أبريل', 'مايو', 'يونيو', 'يوليو'],
          datasets: [
              {
                  label: 'الحضور',
                  data: [65, 59, 80, 81, 56, 55, 40],
                  fill: true,
                  borderColor: '#3b82f6', // brand-accent
                  tension: 0.4,
                  backgroundColor: 'rgba(59, 130, 246, 0.1)'
              },
              {
                  label: 'الإجازات',
                  data: [28, 48, 40, 19, 86, 27, 90],
                  fill: true,
                  borderColor: '#10b981', // emerald-500
                  tension: 0.4,
                  backgroundColor: 'rgba(16, 185, 129, 0.1)'
              }
          ]
      };

      this.options = {
          maintainAspectRatio: false,
          aspectRatio: 0.6,
          plugins: {
              legend: {
                  labels: {
                      color: '#475569', // slate-600
                      font: {
                          family: 'Cairo',
                          weight: 'bold'
                      }
                  },
                  rtl: true
              },
              tooltip: {
                titleFont: { family: 'Cairo' },
                bodyFont: { family: 'Cairo' },
                rtl: true
              }
          },
          scales: {
              x: {
                  ticks: {
                      color: '#64748b', // slate-500
                      font: {
                          family: 'Cairo'
                      }
                  },
                  grid: {
                      color: 'rgba(226, 232, 240, 0.5)', // slate-200/50
                      drawBorder: false
                  },
                  position: 'right' // RTL X-axis? ChartJS RTL tweaks
              },
              y: {
                  ticks: {
                      color: '#64748b', // slate-500
                       font: {
                          family: 'Cairo'
                      }
                  },
                  grid: {
                      color: 'rgba(226, 232, 240, 0.5)', // slate-200/50
                      drawBorder: false
                  },
                  position: 'right'
              }
          }
      };
  }
}
