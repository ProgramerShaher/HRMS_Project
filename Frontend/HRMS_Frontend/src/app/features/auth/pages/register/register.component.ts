import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { PasswordModule } from 'primeng/password';
import { FloatLabelModule } from 'primeng/floatlabel';
import { ToastModule } from 'primeng/toast';
import { MessageService } from 'primeng/api';
import { AuthService } from '../../../../core/auth/services/auth.service';
import { RegisterRequest } from '../../../../core/auth/models/auth.dto';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [
    CommonModule, 
    FormsModule, 
    RouterModule, 
    ButtonModule, 
    InputTextModule, 
    PasswordModule, 
    FloatLabelModule, 
    ToastModule
  ],
  providers: [MessageService],
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss']
})
export class RegisterComponent {
  authService = inject(AuthService);
  router = inject(Router);
  messageService = inject(MessageService);

  registerRequest: RegisterRequest = {
    userName: '',
    email: '',
    password: '',
    confirmPassword: '',
    fullNameAr: '',
    fullNameEn: '',
    phoneNumber: ''
  };

  loading: boolean = false;

  register() {
    if (this.registerRequest.password !== this.registerRequest.confirmPassword) {
      this.messageService.add({severity:'error', summary:'خطأ', detail:'كلمات المرور غير متطابقة'});
      return;
    }

    this.loading = true;
    this.authService.register(this.registerRequest).subscribe({
      next: (res) => {
        this.loading = false;
        this.messageService.add({severity:'success', summary:'تم بنجاح', detail:'تم إنشاء الحساب بنجاح'});
        setTimeout(() => this.router.navigate(['/dashboard']), 1000);
      },
      error: (err) => {
        this.loading = false;
        this.messageService.add({severity:'error', summary:'خطأ', detail:'فشل إنشاء الحساب'});
        console.error(err);
      }
    });
  }
}
