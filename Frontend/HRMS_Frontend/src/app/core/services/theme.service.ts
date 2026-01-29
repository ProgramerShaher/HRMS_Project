import { Injectable, signal, effect, inject } from '@angular/core';
import { DOCUMENT } from '@angular/common';

@Injectable({
  providedIn: 'root'
})
export class ThemeService {
  private document = inject(DOCUMENT);
  
  // Signal to hold the current theme state
  isDark = signal<boolean>(this.getInitialTheme());

  constructor() {
    // Effect to update the DOM whenever isDark changes
    effect(() => {
      const dark = this.isDark();
      if (dark) {
        this.document.documentElement.classList.add('dark');
        localStorage.setItem('theme', 'dark');
      } else {
        this.document.documentElement.classList.remove('dark');
        localStorage.setItem('theme', 'light');
      }
    });
  }

  toggleTheme() {
    this.isDark.update(d => !d);
    console.log('Theme toggled. New state:', this.isDark() ? 'Dark' : 'Light');
  }

  private getInitialTheme(): boolean {
    const savedTheme = localStorage.getItem('theme');
    if (savedTheme) {
        return savedTheme === 'dark';
    }
    // Default to light if no preference to avoid "stuck in dark" feeling for new users
    return false; 
    // previously: return window.matchMedia('(prefers-color-scheme: dark)').matches;
  }
}
