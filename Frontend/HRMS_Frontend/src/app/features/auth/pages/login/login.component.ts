import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { PasswordModule } from 'primeng/password';
import { CheckboxModule } from 'primeng/checkbox';
import { ToastModule } from 'primeng/toast';
import { MessageService } from 'primeng/api';
import { AuthService } from '../../../../core/auth/services/auth.service';
import { LoginRequest } from '../../../../core/auth/models/auth.dto';
import { FloatLabelModule } from 'primeng/floatlabel';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    CommonModule, 
    FormsModule, 
    RouterModule,
    ButtonModule, 
    InputTextModule, 
    PasswordModule, 
    CheckboxModule, 
    ToastModule,
    FloatLabelModule
  ],
  providers: [MessageService],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent {
  authService = inject(AuthService);
  router = inject(Router);
  messageService = inject(MessageService);

  loginRequest: LoginRequest = {
    userName: '',
    password: '',
    rememberMe: false
  };

  loading = signal(false);

  login() {
    if (!this.loginRequest.userName || !this.loginRequest.password) {
      this.messageService.add({severity:'warn', summary:'تنبيه', detail:'يرجى إدخال اسم المستخدم وكلمة المرور'});
      return;
    }

    this.loading.set(true);
    this.authService.login(this.loginRequest).subscribe({
      next: (response) => {
        this.loading.set(false);
        this.router.navigate(['/dashboard']);
        this.messageService.add({severity:'success', summary:'مرحباً', detail: `أهلاً بك ${response.fullName || response.userName}`});
      },
      error: (err) => {
        this.loading.set(false);
        this.messageService.add({severity:'error', summary:'خطأ', detail: 'فشل تسجيل الدخول. تأكد من البيانات.'});
        console.error(err);
      }
    });
  }
}
