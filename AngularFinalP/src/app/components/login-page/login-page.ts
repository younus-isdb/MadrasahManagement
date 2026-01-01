import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../service/authservice';
import { Login } from '../../models/login';

@Component({
  selector: 'app-login-page',
  standalone: false,
  templateUrl: './login-page.html',
  styleUrls: ['./login-page.css']
})
export class LoginPage {
  userName: string = '';
  password: string = '';

  constructor(private auth: AuthService, private router: Router) { }

  FormSubmit() {
    const loginData: Login = {
      userName: this.userName,
      password: this.password
    };

    this.auth.login(loginData).subscribe(
      res => {
        alert('Login successful');

        // Optional: redirect based on role
        const user = this.auth.getUser();
        if (user && user.roles) {
          if (user.roles.includes('Admin')) this.router.navigate(['/admin-dashboard']);
          else if (user.roles.includes('Teacher')) this.router.navigate(['/teacher-dashboard']);
          else this.router.navigate(['/student-dashboard']);
        } else {
          this.router.navigate(['/']); // fallback
        }
      },
      err => {
        console.error(err);
        alert('Login failed: ' + (err.error || err.message));
      }
    );
  }
}
