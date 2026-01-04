import { Component, signal } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../service/authservice';
import { Register } from '../../models/register';

@Component({
  selector: 'app-register-page',
  standalone: false,
  templateUrl: './register-page.html',
  styleUrls: ['./register-page.css']
})
export class RegisterPage {
  model = signal<Register>({
    userName: '',
    password: '',
    comparePassword: '',
    fullName: null,
    email: null
  });

  constructor(private auth: AuthService, private router: Router) { }

  onSubmit() {
    const data = this.model();

    // Client-side validation
    if (data.password !== data.comparePassword) {
      alert('Passwords do not match!');
      return;
    }

    // Remove comparePassword before sending to API
    const payload: Register = {
      userName: data.userName,
      password: data.password,
      fullName: data.fullName,
      email: data.email
    };

    this.auth.register(payload).subscribe(
      res => {
        alert('Registration successful!');
        this.router.navigate(['/login']); // redirect to login
      },
      err => {
        console.error(err);
        alert('Registration failed. Check console for details.');
      }
    );
  }
}
