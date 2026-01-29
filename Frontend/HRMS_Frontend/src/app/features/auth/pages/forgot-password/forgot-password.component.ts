import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { FloatLabelModule } from 'primeng/floatlabel';
import { ToastModule } from 'primeng/toast';
import { MessageService } from 'primeng/api';

@Component({
  selector: 'app-forgot-password',
  standalone: true,
  imports: [
    CommonModule, 
    FormsModule, 
    RouterModule, 
    ButtonModule, 
    InputTextModule, 
    FloatLabelModule, 
    ToastModule
  ],
  providers: [MessageService],
  templateUrl: './forgot-password.component.html',
  styleUrls: ['./forgot-password.component.scss']
})
export class ForgotPasswordComponent {
  messageService = inject(MessageService);
  email: string = '';
  loading: boolean = false;

  submit() {
    if (!this.email) {
      this.messageService.add({severity:'warn', summary:'تنبيه', detail:'يرجى إدخال البريد الإلكتروني'});
      return;
    }

    this.loading = true;
    // Simulate API call
    setTimeout(() => {
        this.loading = false;
        this.messageService.add({severity:'success', summary:'تم الإرسال', detail:'تم إرسال رابط استعادة كلمة المرور إلى بريدك الإلكتروني.'});
    }, 1500);
  }
}
