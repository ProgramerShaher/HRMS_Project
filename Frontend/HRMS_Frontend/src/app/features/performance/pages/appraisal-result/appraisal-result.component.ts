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
    <div class="result-page" *ngIf="appraisal(); else loading">
        <!-- Back Button -->
        <div class="top-bar">
            <button pButton icon="pi pi-arrow-right" label="العودة للقائمة"
                    class="p-button-text" routerLink="/performance/appraisals"></button>
        </div>

        <!-- Hero Score Card -->
        <div class="hero-card" [class]="'grade-' + gradeClass()">
            <div class="hero-left">
                <div class="employee-avatar">{{ initials() }}</div>
                <div class="employee-info">
                    <h2>{{ appraisal()!.employeeName }}</h2>
                    <p>{{ appraisal()!.cycleName }}</p>
                    <p class="evaluator">المُقيّم: {{ appraisal()!.evaluatorName }}</p>
                </div>
            </div>
            <div class="hero-score">
                <div class="score-ring" [style]="ringStyle()">
                    <div class="score-inner">
                        <div class="score-num">{{ appraisal()!.finalScore | number:'1.1-1' }}</div>
                        <div class="score-max">/ 100</div>
                    </div>
                </div>
                <div class="grade-badge">{{ gradeLabel() }}</div>
            </div>
        </div>

        <!-- KPI Details Grid -->
        <div class="section-title">
            <i class="pi pi-chart-bar"></i>
            <span>تفاصيل مؤشرات الأداء (KPIs)</span>
        </div>

        <div class="kpi-grid">
            <div class="kpi-card" *ngFor="let d of appraisal()!.details">
                <div class="kpi-card-header">
                    <div class="kpi-title-row">
                        <span class="kpi-dot" [style.background]="getCategoryColor(d.kpiCategory)"></span>
                        <strong>{{ d.kpiName }}</strong>
                    </div>
                    <span class="kpi-weight">وزن {{ d.weight }}%</span>
                </div>

                <!-- Triple Score Bars -->
                <div class="score-bars">
                    <div class="score-bar-row">
                        <span class="bar-label">التقييم الذاتي</span>
                        <div class="bar-track">
                            <div class="bar-fill employee" [style.width.%]="d.employeeScore"></div>
                        </div>
                        <span class="bar-value">{{ d.employeeScore }}%</span>
                    </div>
                    <div class="score-bar-row">
                        <span class="bar-label">تقييم المدير</span>
                        <div class="bar-track">
                            <div class="bar-fill manager" [style.width.%]="d.managerScore"></div>
                        </div>
                        <span class="bar-value">{{ d.managerScore }}%</span>
                    </div>
                    <div class="score-bar-row final-row">
                        <span class="bar-label">النهائي</span>
                        <div class="bar-track">
                            <div class="bar-fill final" [style.width.%]="d.finalScore"></div>
                        </div>
                        <span class="bar-value final-val">{{ d.finalScore }}%</span>
                    </div>
                </div>

                <!-- Actual vs Target (if available) -->
                <div class="actual-target" *ngIf="d.actualValue !== null && d.actualValue !== undefined">
                    <span class="actual-chip">فعلي: {{ d.actualValue }}</span>
                    <span class="target-chip">مستهدف: {{ d.targetValue }}</span>
                </div>

                <div class="kpi-comment" *ngIf="d.comments">
                    <i class="pi pi-comment"></i>
                    {{ d.comments }}
                </div>
            </div>
        </div>

        <!-- Radar Chart (Canvas) -->
        <div class="radar-section" *ngIf="appraisal()!.details?.length">
            <div class="section-title">
                <i class="pi pi-chart-pie"></i>
                <span>نقاط القوة والضعف</span>
            </div>
            <div class="radar-container">
                <svg [attr.viewBox]="'0 0 400 400'" class="radar-svg" *ngIf="radarPoints().length">
                    <!-- Background circles -->
                    <circle cx="200" cy="200" r="160" fill="none" stroke="#e2e8f0" stroke-width="1"/>
                    <circle cx="200" cy="200" r="120" fill="none" stroke="#e2e8f0" stroke-width="1"/>
                    <circle cx="200" cy="200" r="80" fill="none" stroke="#e2e8f0" stroke-width="1"/>
                    <circle cx="200" cy="200" r="40" fill="none" stroke="#e2e8f0" stroke-width="1"/>

                    <!-- Axis lines -->
                    <line *ngFor="let pt of radarPoints()" [attr.x1]="200" [attr.y1]="200"
                          [attr.x2]="pt.x100" [attr.y2]="pt.y100"
                          stroke="#e2e8f0" stroke-width="1"/>

                    <!-- Employee polygon -->
                    <polygon [attr.points]="getPolygonPoints('employee')"
                             fill="rgba(99,102,241,0.2)" stroke="#6366f1" stroke-width="2"/>

                    <!-- Manager polygon -->
                    <polygon [attr.points]="getPolygonPoints('manager')"
                             fill="rgba(16,185,129,0.15)" stroke="#10b981" stroke-width="2" stroke-dasharray="4"/>

                    <!-- Final polygon -->
                    <polygon [attr.points]="getPolygonPoints('final')"
                             fill="rgba(245,158,11,0.25)" stroke="#f59e0b" stroke-width="2.5"/>

                    <!-- Labels -->
                    <text *ngFor="let pt of radarPoints()"
                          [attr.x]="pt.labelX" [attr.y]="pt.labelY"
                          text-anchor="middle" dominant-baseline="middle"
                          font-size="10" fill="#475569">{{ pt.label }}</text>
                </svg>

                <div class="radar-legend">
                    <div class="legend-item"><span class="lg-dot blue"></span> التقييم الذاتي</div>
                    <div class="legend-item"><span class="lg-dot green"></span> تقييم المدير</div>
                    <div class="legend-item"><span class="lg-dot amber"></span> الدرجة النهائية</div>
                </div>
            </div>
        </div>

        <!-- Comments Section -->
        <div class="comments-section">
            <div class="comment-card" *ngIf="appraisal()!.employeeComment">
                <div class="comment-header"><i class="pi pi-user"></i> تعليق الموظف</div>
                <p>{{ appraisal()!.employeeComment }}</p>
            </div>
            <div class="comment-card manager" *ngIf="appraisal()!.comments">
                <div class="comment-header"><i class="pi pi-briefcase"></i> تعليق المدير / الموارد البشرية</div>
                <p>{{ appraisal()!.comments }}</p>
            </div>
        </div>
    </div>

    <ng-template #loading>
        <div class="loading-screen">
            <i class="pi pi-spin pi-spinner" style="font-size: 2rem; color: #6366f1;"></i>
            <p>جاري تحميل بيانات التقييم…</p>
        </div>
    </ng-template>
    `,
    styles: [`
        :host { display: block; direction: rtl; }
        .result-page { padding: 1.5rem; max-width: 1100px; margin: 0 auto; }
        .top-bar { margin-bottom: 1rem; }

        /* Hero */
        .hero-card {
            border-radius: 20px; padding: 2rem;
            display: flex; justify-content: space-between; align-items: center;
            margin-bottom: 2rem; color: white;
            background: linear-gradient(135deg, #4f46e5, #7c3aed);
        }
        .hero-card.grade-excellent { background: linear-gradient(135deg, #059669, #10b981); }
        .hero-card.grade-verygood  { background: linear-gradient(135deg, #0891b2, #06b6d4); }
        .hero-card.grade-good      { background: linear-gradient(135deg, #4f46e5, #7c3aed); }
        .hero-card.grade-fair      { background: linear-gradient(135deg, #d97706, #f59e0b); }
        .hero-card.grade-poor      { background: linear-gradient(135deg, #dc2626, #ef4444); }

        .hero-left { display: flex; align-items: center; gap: 1.25rem; }
        .employee-avatar {
            width: 64px; height: 64px; border-radius: 50%;
            background: rgba(255,255,255,0.2); display: grid; place-items: center;
            font-size: 1.4rem; font-weight: 700; color: white;
        }
        h2 { margin: 0; font-size: 1.4rem; }
        .employee-info p { margin: 2px 0 0; opacity: 0.85; font-size: 0.875rem; }
        .evaluator { opacity: 0.7 !important; }

        .hero-score { display: flex; flex-direction: column; align-items: center; gap: 0.75rem; }
        .score-ring {
            width: 110px; height: 110px; border-radius: 50%;
            background: conic-gradient(rgba(255,255,255,0.9) var(--pct), rgba(255,255,255,0.2) 0);
            display: grid; place-items: center;
        }
        .score-inner {
            width: 88px; height: 88px; border-radius: 50%;
            background: rgba(0,0,0,0.2); display: flex; flex-direction: column;
            align-items: center; justify-content: center; color: white;
        }
        .score-num { font-size: 1.6rem; font-weight: 800; line-height: 1; }
        .score-max { font-size: 0.75rem; opacity: 0.7; }
        .grade-badge {
            background: rgba(255,255,255,0.2); border-radius: 20px;
            padding: 4px 16px; font-weight: 600; font-size: 0.9rem;
        }

        /* Section Title */
        .section-title {
            display: flex; align-items: center; gap: 0.5rem;
            font-size: 1.05rem; font-weight: 700; color: #1e293b;
            margin-bottom: 1rem; margin-top: 1.5rem;
        }
        .section-title i { color: #6366f1; }

        /* KPI Grid */
        .kpi-grid { display: grid; grid-template-columns: repeat(auto-fill, minmax(320px, 1fr)); gap: 1rem; }
        .kpi-card {
            background: #fff; border-radius: 14px; padding: 1.25rem;
            box-shadow: 0 1px 6px rgba(0,0,0,0.07);
            border: 1px solid #f1f5f9; transition: box-shadow 0.2s;
        }
        .kpi-card:hover { box-shadow: 0 4px 20px rgba(99,102,241,0.12); }
        .kpi-card-header { display: flex; justify-content: space-between; align-items: flex-start; margin-bottom: 1rem; }
        .kpi-title-row { display: flex; align-items: center; gap: 0.5rem; font-size: 0.95rem; }
        .kpi-dot { width: 8px; height: 8px; border-radius: 50%; flex-shrink: 0; }
        .kpi-weight { font-size: 0.78rem; color: #6366f1; background: #ede9fe; border-radius: 20px; padding: 2px 10px; }

        .score-bars { display: flex; flex-direction: column; gap: 0.6rem; }
        .score-bar-row { display: flex; align-items: center; gap: 0.5rem; }
        .bar-label { font-size: 0.75rem; color: #64748b; width: 75px; flex-shrink: 0; }
        .bar-track { flex: 1; height: 8px; background: #f1f5f9; border-radius: 4px; overflow: hidden; }
        .bar-fill { height: 100%; border-radius: 4px; transition: width 0.5s ease; }
        .bar-fill.employee { background: #6366f1; }
        .bar-fill.manager  { background: #10b981; }
        .bar-fill.final    { background: linear-gradient(90deg, #f59e0b, #ef4444); }
        .bar-value { font-size: 0.78rem; width: 32px; text-align: left; color: #64748b; }
        .final-row .bar-label { font-weight: 700; color: #1e293b; }
        .final-val { font-weight: 700; color: #1e293b; }

        .actual-target { display: flex; gap: 0.5rem; margin-top: 0.75rem; }
        .actual-chip { background: #dbeafe; color: #1d4ed8; border-radius: 20px; padding: 2px 8px; font-size: 0.78rem; }
        .target-chip { background: #fef9c3; color: #a16207; border-radius: 20px; padding: 2px 8px; font-size: 0.78rem; }

        .kpi-comment { margin-top: 0.75rem; font-size: 0.82rem; color: #64748b; display: flex; gap: 0.4rem; font-style: italic; }

        /* Radar */
        .radar-section { margin-top: 1.5rem; }
        .radar-container { background: #fff; border-radius: 16px; padding: 1.5rem; box-shadow: 0 1px 6px rgba(0,0,0,0.06); display: flex; align-items: center; gap: 2rem; }
        .radar-svg { width: 300px; height: 300px; flex-shrink: 0; }
        .radar-legend { display: flex; flex-direction: column; gap: 0.75rem; }
        .legend-item { display: flex; align-items: center; gap: 0.5rem; font-size: 0.875rem; color: #475569; }
        .lg-dot { width: 12px; height: 12px; border-radius: 50%; }
        .lg-dot.blue { background: #6366f1; }
        .lg-dot.green { background: #10b981; }
        .lg-dot.amber { background: #f59e0b; }

        /* Comments */
        .comments-section { display: grid; grid-template-columns: 1fr 1fr; gap: 1rem; margin-top: 1.5rem; }
        .comment-card { background: #fff; border-radius: 14px; padding: 1.25rem; box-shadow: 0 1px 6px rgba(0,0,0,0.06); border-right: 4px solid #6366f1; }
        .comment-card.manager { border-color: #10b981; }
        .comment-header { font-weight: 700; color: #1e293b; margin-bottom: 0.5rem; display: flex; gap: 0.4rem; align-items: center; }
        .comment-card p { margin: 0; color: #475569; font-size: 0.9rem; line-height: 1.6; }

        .loading-screen { display: flex; flex-direction: column; align-items: center; justify-content: center; height: 60vh; gap: 1rem; color: #64748b; }
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
