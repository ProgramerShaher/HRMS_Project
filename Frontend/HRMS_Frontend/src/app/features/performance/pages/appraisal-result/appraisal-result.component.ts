import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { PerformanceService } from '../../services/performance.service';
import { EmployeeAppraisal, AppraisalDetail } from '../../models/performance.model';
import { ButtonModule } from 'primeng/button';
import { TagModule } from 'primeng/tag';

@Component({
    selector: 'app-appraisal-result',
    standalone: true,
    imports: [CommonModule, RouterModule, ButtonModule, TagModule],
    template: `
    <div class="bg-white dark:bg-slate-900 rounded-xl p-4 border border-slate-200 dark:border-slate-800 shadow-sm flex flex-col min-h-screen gap-4 animate-in fade-in slide-in-from-bottom-5 duration-700 mx-auto max-w-6xl" dir="rtl" *ngIf="appraisal(); else loading">
        <!-- Back Button -->
        <div class="flex items-center">
            <button pButton icon="pi pi-arrow-right" label="العودة للقائمة"
                    class="p-button-text p-button-sm !text-[10px] !font-bold" routerLink="/performance/appraisals"></button>
        </div>

        <!-- Hero Score Card -->
        <div class="rounded-2xl p-6 flex flex-col md:flex-row justify-between items-center gap-6 text-white shadow-md relative overflow-hidden" 
             [ngClass]="{
                 'bg-gradient-to-br from-emerald-600 to-emerald-800': gradeClass() === 'excellent',
                 'bg-gradient-to-br from-cyan-600 to-cyan-800': gradeClass() === 'verygood',
                 'bg-gradient-to-br from-blue-600 to-indigo-800': gradeClass() === 'good',
                 'bg-gradient-to-br from-amber-500 to-amber-700': gradeClass() === 'fair',
                 'bg-gradient-to-br from-rose-600 to-rose-800': gradeClass() === 'poor'
             }">
            <!-- Background pattern -->
            <div class="absolute inset-0 opacity-10" style="background-image: radial-gradient(circle at 2px 2px, white 1px, transparent 0); background-size: 20px 20px;"></div>
            
            <div class="flex items-center gap-4 relative z-10">
                <div class="w-16 h-16 rounded-full bg-white/20 flex items-center justify-center text-2xl font-bold border border-white/30 backdrop-blur-sm shadow-inner">
                    {{ initials() }}
                </div>
                <div class="flex flex-col">
                    <h2 class="text-xl font-bold tracking-tight">{{ appraisal()!.employeeName }}</h2>
                    <p class="text-xs font-medium text-white/80 mt-1">{{ appraisal()!.cycleName }}</p>
                    <p class="text-[10px] text-white/60 mt-1.5 flex items-center gap-1">
                        <i class="pi pi-user text-[9px]"></i> المُقيّم: {{ appraisal()!.evaluatorName }}
                    </p>
                </div>
            </div>
            <div class="flex flex-col items-center gap-3 relative z-10">
                <div class="w-24 h-24 rounded-full flex items-center justify-center relative shadow-lg"
                     [style.background]="'conic-gradient(rgba(255,255,255,0.95) ' + ((appraisal()!.finalScore ?? 0) / 100 * 360) + 'deg, rgba(255,255,255,0.15) 0)'">
                    <div class="w-20 h-20 rounded-full bg-black/20 backdrop-blur-md flex flex-col items-center justify-center">
                        <span class="text-2xl font-black font-mono leading-none">{{ appraisal()!.finalScore | number:'1.1-1' }}</span>
                        <span class="text-[9px] font-bold text-white/70 mt-1">/ 100</span>
                    </div>
                </div>
                <div class="bg-white/20 backdrop-blur-sm border border-white/30 rounded-full px-4 py-1 font-bold text-xs shadow-inner">
                    {{ gradeLabel() }}
                </div>
            </div>
        </div>

        <!-- KPI Details Grid -->
        <div class="flex items-center gap-2 mt-2">
            <i class="pi pi-chart-bar text-blue-500 text-sm"></i>
            <h3 class="text-xs font-bold text-slate-800 dark:text-slate-200 uppercase tracking-widest">تفاصيل مؤشرات الأداء (KPIs)</h3>
        </div>

        <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
            <div *ngFor="let d of appraisal()!.details" class="bg-slate-50/50 dark:bg-slate-800/30 border border-slate-200 dark:border-slate-700 rounded-xl p-4 flex flex-col gap-4 hover:border-blue-300 dark:hover:border-blue-700 transition-colors shadow-sm">
                
                <div class="flex justify-between items-start">
                    <div class="flex items-center gap-2">
                        <span class="w-2 h-2 rounded-full shadow-sm" [style.background]="getCategoryColor(d.kpiCategory)"></span>
                        <strong class="text-xs font-bold text-slate-800 dark:text-slate-200">{{ d.kpiName }}</strong>
                    </div>
                    <span class="text-[9px] font-bold text-blue-700 bg-blue-100 dark:bg-blue-900/30 dark:text-blue-300 px-2 py-0.5 rounded-full whitespace-nowrap border border-blue-200 dark:border-blue-800/50">وزن {{ d.weight }}%</span>
                </div>

                <!-- Triple Score Bars -->
                <div class="flex flex-col gap-3 mt-1">
                    <div class="flex items-center gap-3">
                        <span class="text-[9px] font-bold text-slate-500 dark:text-slate-400 w-16">التقييم الذاتي</span>
                        <div class="flex-grow h-1.5 bg-slate-200 dark:bg-slate-700 rounded-full overflow-hidden">
                            <div class="h-full bg-blue-500 rounded-full" [style.width.%]="d.employeeScore"></div>
                        </div>
                        <span class="text-[9px] font-mono font-bold text-slate-600 dark:text-slate-300 w-8 text-left">{{ d.employeeScore }}%</span>
                    </div>
                    <div class="flex items-center gap-3">
                        <span class="text-[9px] font-bold text-slate-500 dark:text-slate-400 w-16">تقييم المدير</span>
                        <div class="flex-grow h-1.5 bg-slate-200 dark:bg-slate-700 rounded-full overflow-hidden">
                            <div class="h-full bg-emerald-500 rounded-full" [style.width.%]="d.managerScore"></div>
                        </div>
                        <span class="text-[9px] font-mono font-bold text-slate-600 dark:text-slate-300 w-8 text-left">{{ d.managerScore }}%</span>
                    </div>
                    <div class="flex items-center gap-3">
                        <span class="text-[9px] font-black text-slate-800 dark:text-slate-200 w-16">النهائي</span>
                        <div class="flex-grow h-2 bg-slate-200 dark:bg-slate-700 rounded-full overflow-hidden shadow-inner">
                            <div class="h-full bg-gradient-to-r from-amber-500 to-rose-500 rounded-full" [style.width.%]="d.finalScore"></div>
                        </div>
                        <span class="text-[10px] font-mono font-black text-slate-800 dark:text-slate-200 w-8 text-left">{{ d.finalScore }}%</span>
                    </div>
                </div>

                <!-- Actual vs Target (if available) -->
                <div class="flex gap-2 mt-1" *ngIf="d.actualValue !== null && d.actualValue !== undefined">
                    <span class="text-[9px] font-bold bg-blue-50 text-blue-700 border border-blue-100 dark:bg-blue-900/20 dark:text-blue-400 dark:border-blue-900/50 rounded-full px-2.5 py-0.5">فعلي: <span class="font-mono">{{ d.actualValue }}</span></span>
                    <span class="text-[9px] font-bold bg-amber-50 text-amber-700 border border-amber-100 dark:bg-amber-900/20 dark:text-amber-400 dark:border-amber-900/50 rounded-full px-2.5 py-0.5">مستهدف: <span class="font-mono">{{ d.targetValue }}</span></span>
                </div>

                <div class="mt-2 text-[9px] text-slate-500 dark:text-slate-400 flex gap-2 font-medium bg-white dark:bg-slate-900 p-2 rounded-lg border border-slate-100 dark:border-slate-800" *ngIf="d.comments">
                    <i class="pi pi-comment text-[9px] text-slate-400 mt-0.5"></i>
                    <span>{{ d.comments }}</span>
                </div>
            </div>
        </div>

        <div class="grid grid-cols-1 lg:grid-cols-2 gap-4 mt-2">
            <!-- Radar Chart (Canvas) -->
            <div class="bg-white dark:bg-slate-900 border border-slate-200 dark:border-slate-700 rounded-xl p-5 shadow-sm" *ngIf="appraisal()!.details?.length">
                <div class="flex items-center gap-2 mb-6">
                    <i class="pi pi-chart-pie text-purple-500 text-sm"></i>
                    <h3 class="text-xs font-bold text-slate-800 dark:text-slate-200 uppercase tracking-widest">نقاط القوة والضعف</h3>
                </div>
                <div class="flex flex-col md:flex-row items-center gap-8 justify-center">
                    <svg [attr.viewBox]="'0 0 400 400'" class="w-64 h-64 drop-shadow-sm" *ngIf="radarPoints().length">
                        <!-- Background circles -->
                        <circle cx="200" cy="200" r="160" fill="none" stroke="currentColor" class="text-slate-200 dark:text-slate-700" stroke-width="1"/>
                        <circle cx="200" cy="200" r="120" fill="none" stroke="currentColor" class="text-slate-200 dark:text-slate-700" stroke-width="1"/>
                        <circle cx="200" cy="200" r="80" fill="none" stroke="currentColor" class="text-slate-200 dark:text-slate-700" stroke-width="1"/>
                        <circle cx="200" cy="200" r="40" fill="none" stroke="currentColor" class="text-slate-200 dark:text-slate-700" stroke-width="1"/>

                        <!-- Axis lines -->
                        <line *ngFor="let pt of radarPoints()" [attr.x1]="200" [attr.y1]="200"
                              [attr.x2]="pt.x100" [attr.y2]="pt.y100"
                              stroke="currentColor" class="text-slate-200 dark:text-slate-700" stroke-width="1"/>

                        <!-- Employee polygon -->
                        <polygon [attr.points]="getPolygonPoints('employee')"
                                 fill="rgba(59,130,246,0.15)" stroke="#3b82f6" stroke-width="2"/>

                        <!-- Manager polygon -->
                        <polygon [attr.points]="getPolygonPoints('manager')"
                                 fill="rgba(16,185,129,0.15)" stroke="#10b981" stroke-width="2" stroke-dasharray="4"/>

                        <!-- Final polygon -->
                        <polygon [attr.points]="getPolygonPoints('final')"
                                 fill="rgba(245,158,11,0.2)" stroke="#f59e0b" stroke-width="2.5"/>

                        <!-- Labels -->
                        <text *ngFor="let pt of radarPoints()"
                              [attr.x]="pt.labelX" [attr.y]="pt.labelY"
                              text-anchor="middle" dominant-baseline="middle"
                              class="text-[10px] font-bold fill-slate-500 dark:fill-slate-400">{{ pt.label }}</text>
                    </svg>

                    <div class="flex flex-col gap-3">
                        <div class="flex items-center gap-2 text-[10px] font-bold text-slate-600 dark:text-slate-300">
                            <span class="w-3 h-3 rounded-full bg-blue-500"></span> التقييم الذاتي
                        </div>
                        <div class="flex items-center gap-2 text-[10px] font-bold text-slate-600 dark:text-slate-300">
                            <span class="w-3 h-3 rounded-full bg-emerald-500"></span> تقييم المدير
                        </div>
                        <div class="flex items-center gap-2 text-[10px] font-bold text-slate-600 dark:text-slate-300">
                            <span class="w-3 h-3 rounded-full bg-amber-500"></span> الدرجة النهائية
                        </div>
                    </div>
                </div>
            </div>

            <!-- Comments Section -->
            <div class="flex flex-col gap-4">
                <div class="bg-blue-50/50 dark:bg-blue-900/10 border border-blue-100 dark:border-blue-900/30 rounded-xl p-5 shadow-sm" *ngIf="appraisal()!.employeeComment">
                    <div class="flex items-center gap-2 mb-3 text-blue-700 dark:text-blue-400 font-bold text-[11px]">
                        <i class="pi pi-user text-sm"></i> تعليق الموظف
                    </div>
                    <p class="text-[10px] text-slate-700 dark:text-slate-300 font-medium leading-relaxed">{{ appraisal()!.employeeComment }}</p>
                </div>
                <div class="bg-emerald-50/50 dark:bg-emerald-900/10 border border-emerald-100 dark:border-emerald-900/30 rounded-xl p-5 shadow-sm flex-grow" *ngIf="appraisal()!.comments">
                    <div class="flex items-center gap-2 mb-3 text-emerald-700 dark:text-emerald-400 font-bold text-[11px]">
                        <i class="pi pi-briefcase text-sm"></i> تعليق المدير / الموارد البشرية
                    </div>
                    <p class="text-[10px] text-slate-700 dark:text-slate-300 font-medium leading-relaxed">{{ appraisal()!.comments }}</p>
                </div>
            </div>
        </div>
    </div>

    <ng-template #loading>
        <div class="flex flex-col items-center justify-center min-h-[60vh] gap-4 text-slate-500 dark:text-slate-400">
            <i class="pi pi-spin pi-spinner text-3xl text-blue-500"></i>
            <p class="text-xs font-bold tracking-widest">جاري تحميل بيانات التقييم…</p>
        </div>
    </ng-template>
    `,
    styles: [`
        :host { display: block; }
    `]
})
export class AppraisalResultComponent implements OnInit {
    private route = inject(ActivatedRoute);
    private svc = inject(PerformanceService);

    appraisal = signal<EmployeeAppraisal | null>(null);

    ngOnInit() {
        const id = +(this.route.snapshot.paramMap.get('id') ?? 0);
        if (id) {
            this.svc.getAppraisalById(id).subscribe(res => {
                if (res.succeeded) this.appraisal.set(res.data);
            });
        }
    }

    initials(): string {
        const name = this.appraisal()?.employeeName ?? '';
        return name.split(' ').slice(0, 2).map(p => p[0]).join('');
    }

    gradeClass(): string {
        const g = this.appraisal()?.grade?.toLowerCase() ?? '';
        if (g === 'excellent')  return 'excellent';
        if (g === 'very good')  return 'verygood';
        if (g === 'good')       return 'good';
        if (g === 'fair')       return 'fair';
        return 'poor';
    }

    gradeLabel(): string {
        const m: Record<string, string> = {
            'excellent': 'ممتاز', 'very good': 'جيد جداً',
            'good': 'جيد', 'fair': 'مقبول', 'poor': 'ضعيف'
        };
        return m[this.appraisal()?.grade?.toLowerCase() ?? ''] ?? this.appraisal()?.grade ?? '—';
    }

    ringStyle(): string {
        const pct = (this.appraisal()?.finalScore ?? 0) / 100 * 360;
        return `--pct: ${pct}deg`;
    }

    // ── Radar Chart ───────────────────────────────────────────────────────────
    radarPoints(): any[] {
        const details = this.appraisal()?.details ?? [];
        if (!details.length) return [];
        const n = details.length;
        const cx = 200, cy = 200, r = 160;
        return details.map((d, i) => {
            const angle = (i / n) * 2 * Math.PI - Math.PI / 2;
            return {
                label: d.kpiName.substring(0, 8),
                x100: cx + r * Math.cos(angle),
                y100: cy + r * Math.sin(angle),
                labelX: cx + (r + 22) * Math.cos(angle),
                labelY: cy + (r + 22) * Math.sin(angle),
                angle,
                employeeR: (d.employeeScore / 100) * r,
                managerR: (d.managerScore / 100) * r,
                finalR: (d.finalScore / 100) * r
            };
        });
    }

    getPolygonPoints(type: 'employee' | 'manager' | 'final'): string {
        return this.radarPoints().map(pt => {
            const rkey = type === 'employee' ? pt.employeeR : type === 'manager' ? pt.managerR : pt.finalR;
            const cx = 200, cy = 200;
            const x = cx + rkey * Math.cos(pt.angle);
            const y = cy + rkey * Math.sin(pt.angle);
            return `${x},${y}`;
        }).join(' ');
    }

    getCategoryColor(cat?: string): string {
        const m: Record<string, string> = {
            ATTENDANCE: '#6366f1', PRODUCTIVITY: '#10b981',
            BEHAVIOR: '#f59e0b', OVERTIME: '#3b82f6',
            DEVELOPMENT: '#8b5cf6', GENERAL: '#94a3b8'
        };
        return m[cat?.toUpperCase() ?? ''] ?? '#94a3b8';
    }
}
