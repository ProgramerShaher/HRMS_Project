import { Component, ContentChild, EventEmitter, Input, Output, TemplateRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ButtonModule } from 'primeng/button';
import { TableModule } from 'primeng/table';
import { DialogModule } from 'primeng/dialog';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { ConfirmationService } from 'primeng/api';

@Component({
  selector: 'app-dynamic-array-list',
  standalone: true,
  imports: [
    CommonModule,
    ButtonModule,
    TableModule,
    DialogModule,
    ConfirmDialogModule
  ],
  providers: [ConfirmationService],
  templateUrl: 'dynamic-array-list.component.html',
  styles: []
})
export class DynamicArrayListComponent {
  @Input() items: any[] = [];
  @Input() title: string = '';
  @Input() itemLabel: string = 'عنصر';
  @Input() icon: string = 'pi pi-list';
  @Input() columns: { field: string, header: string }[] = [];
  
  @Output() save = new EventEmitter<any>();
  @Output() delete = new EventEmitter<any>();

  @ContentChild('formTemplate') formTemplate!: TemplateRef<any>;

  displayDialog: boolean = false;
  currentItem: any = {};
  isEdit: boolean = false;
  selectedIndex: number = -1;

  constructor(private confirmationService: ConfirmationService) {}

  showDialogToAdd() {
    this.isEdit = false;
    this.currentItem = {};
    this.displayDialog = true;
  }

  showDialogToEdit(item: any, index: number) {
    this.isEdit = true;
    this.selectedIndex = index;
    this.currentItem = { ...item }; // Clone
    this.displayDialog = true;
  }

  onSave() {
    this.save.emit({ item: this.currentItem, index: this.isEdit ? this.selectedIndex : -1 });
    this.displayDialog = false;
    this.currentItem = {};
  }

  onDelete(item: any, index: number) {
    this.confirmationService.confirm({
      message: `هل أنت متأكد من حذف هذا ${this.itemLabel}؟`,
      header: 'تأكيد الحذف',
      icon: 'pi pi-exclamation-triangle',
      acceptLabel: 'نعم',
      rejectLabel: 'لا',
      acceptButtonStyleClass: 'p-button-danger p-button-text',
      rejectButtonStyleClass: 'p-button-text',
      accept: () => {
        this.delete.emit({ item, index });
      }
    });
  }

  hideDialog() {
    this.displayDialog = false;
  }
}
