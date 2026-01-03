import { Component } from '@angular/core';
import { AuthService } from './service/authservice'; // adjust path

@Component({
  selector: 'app-root',
  standalone:false,
  templateUrl: './app.html',
  styleUrls: ['./app.css']
})
export class App {
  title = 'Halishahar Islamia Madrasah';

  navbarOpen = false;
  dropdowns: { [key: string]: boolean } = {};

  constructor(public auth: AuthService) { }

  // Auth helpers
  loggedIn(): boolean {
    return this.auth.isLoggedIn();
  }

  name(): string {
    return this.auth.getUserName();
  }

  isAdmin(): boolean {
    return this.auth.hasRole('Admin');
  }

  isTeacher(): boolean {
    return this.auth.hasRole('Teacher');
  }

  isStudent(): boolean {
    return this.auth.hasRole('Student');
  }

  // UI helpers
  toggleNavbar() {
    this.navbarOpen = !this.navbarOpen;
  }

  toggleDropdown(name: string) {
    this.dropdowns[name] = !this.dropdowns[name];
  }
}
