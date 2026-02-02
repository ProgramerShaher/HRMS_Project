import { Injectable, signal, effect, inject } from '@angular/core';
import { DOCUMENT } from '@angular/common';

@Injectable({
  providedIn: 'root'
})
export class ThemeService {
  private document = inject(DOCUMENT);
  
  // Signal to hold the current theme state (true = dark, false = light)
  isDark = signal<boolean>(this.getInitialTheme());

  constructor() {
    // Effect to update the DOM whenever isDark changes
    effect(() => {
      const dark = this.isDark();
      const html = this.document.documentElement;
      
      if (dark) {
        html.classList.add('dark');
        localStorage.setItem('theme', 'dark');
      } else {
        html.classList.remove('dark');
        localStorage.setItem('theme', 'light');
      }
    });
  }

  toggleTheme() {
    this.isDark.update(d => !d);
    console.log('Theme toggled. New state:', this.isDark() ? 'Dark' : 'Light');
  }

  private getInitialTheme(): boolean {
    // Check localStorage first
    const savedTheme = localStorage.getItem('theme');
    if (savedTheme) {
        return savedTheme === 'dark';
    }
    
    // Fallback to system preference, default to False (Light) if simpler
    return false; 
  }
}
