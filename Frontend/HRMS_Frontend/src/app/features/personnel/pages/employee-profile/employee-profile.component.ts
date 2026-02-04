import { Component, OnInit, inject, signal, computed } from '@angular/core';
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
import { Tooltip } from "primeng/tooltip";

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
    CardModule,
    Tooltip
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
    // Use the full-profile endpoint to get all employee data in one call
    this.employeeService.getFullProfile(id).subscribe({
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

  currentCompensation = computed(() => {
    const emp = this.employee();
    if (emp?.compensation && (emp.compensation.totalSalary > 0 || emp.compensation.basicSalary > 0)) {
        return emp.compensation;
    }
    
    // Fallback to active contract or first contract
    const activeContract = emp?.contracts?.find(c => c.contractStatus === 'ACTIVE') || emp?.contracts?.[0];
    if (activeContract) {
        return {
            basicSalary: activeContract.basicSalary || 0,
            housingAllowance: activeContract.housingAllowance || 0,
            transportAllowance: activeContract.transportAllowance || 0,
            medicalAllowance: 0, 
            totalSalary: (activeContract.basicSalary || 0) + (activeContract.housingAllowance || 0) + (activeContract.transportAllowance || 0) + (activeContract.otherAllowances || 0)
        };
    }
    return null;
  });

  getInitials(name: string): string {
    if (!name) return 'EMP';
    const parts = name.split(' ');
    if (parts.length === 1) return parts[0].substring(0, 2).toUpperCase();
    return (parts[0][0] + parts[1][0]).toUpperCase();
  }

  getProfilePictureUrl(path?: string): string | null {
    if (!path) return null;
    // If path starts with http, return as is
    if (path.startsWith('http')) return path;
    
    // Build full URL from backend base
    const baseUrl = 'https://localhost:5001';
    // Remove leading slash if exists to avoid double slashes
    const cleanPath = path.startsWith('/') ? path.substring(1) : path;
    const fullUrl = `${baseUrl}/${cleanPath}`;
    
    console.log('Profile Picture Path:', path);
    console.log('Profile Picture URL:', fullUrl);
    return fullUrl;
  }

  // Generate avatar URL using UI Avatars service as fallback
  getAvatarUrl(name: string): string {
    const initials = this.getInitials(name);
    return `https://ui-avatars.com/api/?name=${encodeURIComponent(initials)}&size=200&background=3b82f6&color=fff&bold=true`;
  }

  onImageError(event: Event) {
    // Hide the image and show initials instead
    const img = event.target as HTMLImageElement;
    const parent = img.parentElement;
    if (parent) {
      parent.style.display = 'none';
    }
    console.error('Failed to load profile picture');
  }

  onImageLoadError(event: Event) {
    const img = event.target as HTMLImageElement;
    console.error('❌ Failed to load profile picture from:', img.src);
    console.error('Please check:');
    console.error('1. Backend is running');
    console.error('2. UseStaticFiles() is configured');
    console.error('3. File exists at the path');
    console.error('4. CORS is properly configured');
    
    // Hide broken image
    img.style.display = 'none';
  }
}
