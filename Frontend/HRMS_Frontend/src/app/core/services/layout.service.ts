import { Injectable, signal } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class LayoutService {
  // Signal State for Sidebar Visibility
  sidebarVisible = signal<boolean>(true);
  
  // Toggle Logic
  toggleSidebar() {
    this.sidebarVisible.update(v => !v);
  }

  // Direction State
  direction = signal<'ltr' | 'rtl'>('rtl');

  // Toggle Direction
  toggleDirection() {
    this.direction.update(d => d === 'ltr' ? 'rtl' : 'ltr');
    // Ideally update document body dir attribute too for global libraries
    document.documentElement.dir = this.direction();
  }

  // Set Explicit Side State
  setSidebarState(state: boolean) {
    this.sidebarVisible.set(state);
  }
}
