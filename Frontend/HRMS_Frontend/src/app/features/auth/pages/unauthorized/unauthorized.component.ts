import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { ButtonModule } from 'primeng/button';

@Component({
    selector: 'app-unauthorized',
    standalone: true,
    imports: [CommonModule, RouterModule, ButtonModule],
    template: `
    <div class="unauthorized-container">
      <div class="unauthorized-content">
        <i class="pi pi-lock" style="font-size: 5rem; color: var(--red-500);"></i>
        <h1>غير مصرح</h1>
        <h2>403 - Unauthorized</h2>
        <p>عذراً، ليس لديك الصلاحيات اللازمة للوصول إلى هذه الصفحة.</p>
        <p class="text-muted">يرجى الاتصال بمسؤول النظام إذا كنت تعتقد أن هذا خطأ.</p>
        
        <div class="actions">
          <button pButton label="العودة للصفحة الرئيسية" icon="pi pi-home" (click)="goHome()"></button>
          <button pButton label="رجوع" icon="pi pi-arrow-right" class="p-button-secondary" (click)="goBack()"></button>
        </div>
      </div>
    </div>
  `,
    styles: [`
    .unauthorized-container {
      display: flex;
      justify-content: center;
      align-items: center;
      min-height: 100vh;
      background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
      padding: 2rem;
    }

    .unauthorized-content {
      background: white;
      border-radius: 1rem;
      padding: 3rem;
      text-align: center;
      max-width: 500px;
      box-shadow: 0 10px 40px rgba(0, 0, 0, 0.2);
    }

    h1 {
      font-size: 2rem;
      margin: 1rem 0 0.5rem;
      color: var(--text-color);
    }

    h2 {
      font-size: 1.2rem;
      color: var(--text-color-secondary);
      margin: 0 0 1.5rem;
      font-weight: normal;
    }

    p {
      color: var(--text-color-secondary);
      line-height: 1.6;
      margin: 0.5rem 0;
    }

    .text-muted {
      font-size: 0.9rem;
      color: var(--text-color-secondary);
    }

    .actions {
      display: flex;
      gap: 1rem;
      justify-content: center;
      margin-top: 2rem;
      flex-wrap: wrap;
    }

    i.pi-lock {
      animation: shake 0.5s;
    }

    @keyframes shake {
      0%, 100% { transform: translateX(0); }
      25% { transform: translateX(-10px); }
      75% { transform: translateX(10px); }
    }
  `]
})
export class UnauthorizedComponent {
    constructor(private router: Router) { }

    goHome() {
        this.router.navigate(['/dashboard']);
    }

    goBack() {
        window.history.back();
    }
}
