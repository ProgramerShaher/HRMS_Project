import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule, Location } from '@angular/common';
import { ButtonModule } from 'primeng/button';
import { Router } from '@angular/router';

/**
 * مكون الأزرار الموحد للواجهات الإدارية
 * Unified action buttons component for admin interfaces
 */
@Component({
  selector: 'app-action-buttons',
  standalone: true,
  imports: [CommonModule, ButtonModule],
  template: `
    <div class="action-buttons-container">
      <!-- أزرار التنقل -->
      <div class="nav-buttons">
        <p-button 
          *ngIf="showBack" 
          icon="pi pi-arrow-right" 
          [label]="backLabel"
          (onClick)="handleBack()" 
          styleClass="p-button-text p-button-secondary"
          [severity]="'secondary'" />
        
        <p-button 
          *ngIf="showHome" 
          icon="pi pi-home" 
          label="الرئيسية"
          (onClick)="handleHome()" 
          styleClass="p-button-text p-button-secondary"
          [severity]="'secondary'" />
      </div>
      
      <!-- أزرار الإجراءات -->
      <div class="action-buttons">
        <p-button 
          *ngIf="showCreate" 
          icon="pi pi-plus" 
          [label]="createLabel"
          (onClick)="handleCreate()" 
          styleClass="p-button-success"
          [severity]="'success'" />
        
        <p-button 
          *ngIf="showRefresh" 
          icon="pi pi-refresh" 
          [label]="refreshLabel"
          (onClick)="handleRefresh()" 
          styleClass="p-button-outlined"
          [severity]="'secondary'" />
        
        <p-button 
          *ngIf="showExport" 
          icon="pi pi-download" 
          [label]="exportLabel"
          (onClick)="handleExport()" 
          styleClass="p-button-outlined"
          [severity]="'info'" />
        
        <p-button 
          *ngIf="showPrint" 
          icon="pi pi-print" 
          [label]="printLabel"
          (onClick)="handlePrint()" 
          styleClass="p-button-outlined"
          [severity]="'secondary'" />
        
        <p-button 
          *ngIf="showSave" 
          icon="pi pi-save" 
          [label]="saveLabel"
          (onClick)="handleSave()" 
          styleClass="p-button-success"
          [severity]="'success'" />
      </div>
    </div>
  `,
  styles: [`
    .action-buttons-container {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 1.5rem;
      padding: 1rem 1.5rem;
      background: var(--surface-card);
      border-radius: 12px;
      box-shadow: 0 2px 8px rgba(0, 0, 0, 0.08);
      border: 1px solid var(--surface-border);
    }

    .nav-buttons,
    .action-buttons {
      display: flex;
      gap: 0.75rem;
      align-items: center;
    }

    :host ::ng-deep {
      .p-button {
        font-weight: 600;
        transition: all 0.2s ease;
      }

      .p-button:hover {
        transform: translateY(-2px);
        box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
      }

      .p-button-text:hover {
        transform: none;
        box-shadow: none;
      }
    }

    @media (max-width: 768px) {
      .action-buttons-container {
        flex-direction: column;
        gap: 1rem;
      }

      .nav-buttons,
      .action-buttons {
        width: 100%;
        justify-content: center;
      }
    }
  `]
})
export class ActionButtonsComponent {
  // Navigation buttons
  @Input() showBack = false;
  @Input() showHome = false;
  @Input() backLabel = 'رجوع';
  @Input() backRoute?: string;

  // Action buttons
  @Input() showCreate = false;
  @Input() showRefresh = false;
  @Input() showExport = false;
  @Input() showPrint = false;
  @Input() showSave = false;

  // Labels
  @Input() createLabel = 'إضافة جديد';
  @Input() refreshLabel = 'تحديث';
  @Input() exportLabel = 'تصدير';
  @Input() printLabel = 'طباعة';
  @Input() saveLabel = 'حفظ';

  // Events
  @Output() create = new EventEmitter<void>();
  @Output() refresh = new EventEmitter<void>();
  @Output() export = new EventEmitter<void>();
  @Output() print = new EventEmitter<void>();
  @Output() save = new EventEmitter<void>();
  @Output() back = new EventEmitter<void>();
  @Output() home = new EventEmitter<void>();

  constructor(
    private location: Location,
    private router: Router
  ) {}

  handleBack() {
    if (this.backRoute) {
      this.router.navigate([this.backRoute]);
    } else {
      this.location.back();
    }
    this.back.emit();
  }

  handleHome() {
    this.router.navigate(['/payroll/dashboard']);
    this.home.emit();
  }

  handleCreate() {
    this.create.emit();
  }

  handleRefresh() {
    this.refresh.emit();
  }

  handleExport() {
    this.export.emit();
  }

  handlePrint() {
    this.print.emit();
  }

  handleSave() {
    this.save.emit();
  }
}
