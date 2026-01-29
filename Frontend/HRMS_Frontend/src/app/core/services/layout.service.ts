import { Injectable, signal } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class LayoutService {
  // Signal State for Sidebar Visibility
  sidebarVisible = signal<boolean>(true);
  isSidebarCollapsed = signal<boolean>(false);
  
  // Toggle Logic
  toggleSidebar() {
    this.isSidebarCollapsed.update(v => !v);
  }

  setSidebarState(state: boolean) {
    this.sidebarVisible.set(state);
  }

  // Direction State
  direction = signal<'ltr' | 'rtl'>('rtl');

  // Toggle Direction
  toggleDirection() {
    this.direction.update(d => d === 'ltr' ? 'rtl' : 'ltr');
    // Ideally update document body dir attribute too for global libraries
    document.documentElement.dir = this.direction();
  }

}
