import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common'; 
import { FormsModule } from '@angular/forms'; 
import { LoginUser } from '../../models/models';
import { AuthService } from '../../services/auth'; // Dosya yolunu kontrol et

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './login.html', // Dosya ismin login.html ise doğru
  styleUrls: ['./login.css']   // Dosya ismin login.css ise doğru
})
export class LoginComponent {
  
  loginModel: LoginUser = {
    email: '',
    password: ''
  };

  errorMessage: string = '';

  constructor(private authService: AuthService, private router: Router) {}

  login() {
    this.authService.login(this.loginModel).subscribe({
      next: (response) => {
        console.log('Giriş başarılı!', response);
        
        // DEĞİŞEN KISIM BURASI:
        // Eskisi: this.router.navigate(['/']); 
        // Yenisi: Başarılı olunca '/home' rotasına git
        this.router.navigate(['/home']); 
      },
      error: (err) => {
        console.error('Giriş hatası:', err);
        if (err.error && err.error.message) {
            this.errorMessage = err.error.message;
        } else {
            this.errorMessage = 'E-posta veya şifre hatalı.';
        }
      }
    });
  }
}