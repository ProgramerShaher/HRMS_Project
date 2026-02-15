import { Directive, Input, ElementRef, Renderer2, inject, ViewContainerRef, TemplateRef, effect } from '@angular/core';
import { PermissionService } from '../../core/auth/services/permission.service';

/**
 * Attribute Directive لإخفاء/إظهار العناصر بناءً على الصلاحيات
 * يستخدم display: none للإخفاء، مما يسمح باستخدامه مع *ngIf
 */
@Directive({
    selector: '[hasPermission]',
    standalone: true
})
export class HasPermissionDirective {
    @Input() hasPermission?: string;

    private permissionService = inject(PermissionService);
    private el = inject(ElementRef);
    private renderer = inject(Renderer2);

    constructor() {
        effect(() => {
            this.updateView();
        });
    }

    private updateView() {
        // إذا لم يتم تحديد صلاحية، أو كان المستخدم لديه الصلاحية، اظهر العنصر
        if (!this.hasPermission || this.permissionService.hasPermission(this.hasPermission)) {
            this.renderer.removeStyle(this.el.nativeElement, 'display');
        } else {
            // إخفاء العنصر
            this.renderer.setStyle(this.el.nativeElement, 'display', 'none');
        }
    }
}

/**
 * Structural Directive لإخفاء/إظهار العناصر بناءً على وجود أي صلاحية من قائمة
 */
@Directive({
    selector: '[hasAnyPermission]',
    standalone: true
})
export class HasAnyPermissionDirective {
    @Input() hasAnyPermission!: string[];

    private permissionService = inject(PermissionService);
    private templateRef = inject(TemplateRef<any>);
    private viewContainer = inject(ViewContainerRef);

    constructor() {
        effect(() => {
            this.updateView();
        });
    }

    private updateView() {
        this.viewContainer.clear();
        if (this.permissionService.hasAnyPermission(this.hasAnyPermission)) {
            this.viewContainer.createEmbeddedView(this.templateRef);
        }
    }
}

/**
 * Structural Directive لإخفاء/إظهار العناصر بناءً على الدور
 */
@Directive({
    selector: '[hasRole]',
    standalone: true
})
export class HasRoleDirective {
    @Input() hasRole!: string;

    private permissionService = inject(PermissionService);
    private templateRef = inject(TemplateRef<any>);
    private viewContainer = inject(ViewContainerRef);

    constructor() {
        effect(() => {
            this.updateView();
        });
    }

    private updateView() {
        this.viewContainer.clear();
        if (this.permissionService.hasRole(this.hasRole)) {
            this.viewContainer.createEmbeddedView(this.templateRef);
        }
    }
}

/**
 * Structural Directive لإخفاء/إظهار العناصر بناءً على وجود أي دور من قائمة
 */
@Directive({
    selector: '[hasAnyRole]',
    standalone: true
})
export class HasAnyRoleDirective {
    @Input() hasAnyRole!: string[];

    private permissionService = inject(PermissionService);
    private templateRef = inject(TemplateRef<any>);
    private viewContainer = inject(ViewContainerRef);

    constructor() {
        effect(() => {
            this.updateView();
        });
    }

    private updateView() {
        this.viewContainer.clear();
        if (this.permissionService.hasAnyRole(this.hasAnyRole)) {
            this.viewContainer.createEmbeddedView(this.templateRef);
        }
    }
}
