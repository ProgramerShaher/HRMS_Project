import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { TabsModule } from 'primeng/tabs';
import { DividerModule } from 'primeng/divider';
import { AvatarModule } from 'primeng/avatar';
import { TagModule } from 'primeng/tag';
import { SkeletonModule } from 'primeng/skeleton';
import { CardModule } from 'primeng/card';
import { EmployeeService } from '../../services/employee.service';
import { EmployeeProfile } from '../../models/employee-profile.model';

@Component({
  selector: 'app-employee-profile',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    ButtonModule,
    TabsModule,
    DividerModule,
    AvatarModule,
    TagModule,
    SkeletonModule,
    CardModule
  ],
  templateUrl: './employee-profile.component.html',
  styleUrls: ['./employee-profile.component.scss']
})
export class EmployeeProfileComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private employeeService = inject(EmployeeService);

  employeeId = signal<number>(0);
  employee = signal<EmployeeProfile | null>(null);
  loading = signal<boolean>(true);

  ngOnInit() {
    this.route.paramMap.subscribe(params => {
      const id = params.get('id');
      if (id) {
        this.employeeId.set(+id);
        this.loadProfile(+id);
      }
    });
  }

  loadProfile(id: number) {
    this.loading.set(true);
    // Try get full profile, assuming endpoint exists or falling back/mocking for now if needed.
    // Based on user input, we use getById which returns the profile structure we defined.
    this.employeeService.getById(id).subscribe({
      next: (data) => {
        this.employee.set(data);
        this.loading.set(false);
      },
      error: (err) => {
        console.error(err);
        this.loading.set(false);
      }
    });
  }

  getInitials(name: string): string {
    if (!name) return 'EMP';
    const parts = name.split(' ');
    if (parts.length === 1) return parts[0].substring(0, 2).toUpperCase();
    return (parts[0][0] + parts[1][0]).toUpperCase();
  }
}
