import { Directive, HostListener, ElementRef, inject } from '@angular/core';

@Directive({
  selector: '[appEnterNext]',
  standalone: true
})
export class EnterNextDirective {
  private el = inject(ElementRef);

  @HostListener('keydown', ['$event'])
  onKeyDown(event: KeyboardEvent) {
    if (event.key === 'Enter') {
      const target = event.target as HTMLElement;
      
      // If it's a button, let the default behavior happen (e.g. submit)
      if (target.tagName.toLowerCase() === 'button' || target.getAttribute('role') === 'button') {
        return;
      }

      event.preventDefault(); // Prevent form submission or newline

      // Find all focusable elements within the host element
      const focusableSelectors = 'input:not([disabled]):not([readonly]), select:not([disabled]), textarea:not([disabled]), button:not([disabled]), [tabindex]:not([tabindex="-1"])';
      const focusableElements = Array.from(this.el.nativeElement.querySelectorAll(focusableSelectors)) as HTMLElement[];
      
      const currentIndex = focusableElements.indexOf(target);
      if (currentIndex > -1 && currentIndex < focusableElements.length - 1) {
        // Focus the next element
        let nextElement = focusableElements[currentIndex + 1];
        nextElement.focus();
      } else if (currentIndex === focusableElements.length - 1) {
        // Optionally blur or wrap around. We'll just blur for now to indicate end of form
        target.blur();
      }
    }
  }

  @HostListener('focusin', ['$event'])
  onFocusIn(event: FocusEvent) {
    const target = event.target as HTMLInputElement;
    if (target && (target.tagName.toLowerCase() === 'input' || target.tagName.toLowerCase() === 'textarea')) {
      // Auto-select text on focus
      try {
        target.select();
      } catch (e) {
        // Some input types don't support selection (like date or email in some browsers), ignore gracefully
      }
    }
  }
}
