import { Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { tap } from 'rxjs/operators';
import { Observable } from 'rxjs';

export const LoggedIn = signal<boolean>(!!localStorage.getItem('token'));
export const UserName = signal<string | null>(
  localStorage.getItem('user') ? JSON.parse(localStorage.getItem('user')!).userName : null
);
export const UserRoles = signal<string[]>(
  localStorage.getItem('user') ? JSON.parse(localStorage.getItem('user')!).roles : []
);

interface LoginResponse {
  token: string;
  userName: string;
  fullName: string;
  roles: string[];
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private apiUrl = 'https://localhost:7113/api/Token';

  constructor(private http: HttpClient) { }

  // ---------------- Auth API ----------------
  register(user: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/register`, user);
  }

  login(user: any): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${this.apiUrl}/login`, user)
      .pipe(
        tap(res => {
          localStorage.setItem('token', res.token);
          localStorage.setItem('user', JSON.stringify(res));

          LoggedIn.set(true);
          UserName.set(res.userName);
          UserRoles.set(res.roles);
        })
      );
  }

  logout() {
    localStorage.removeItem('token');
    localStorage.removeItem('user');

    LoggedIn.set(false);
    UserName.set(null);
    UserRoles.set([]);
  }

  getToken(): string | null {
    return localStorage.getItem('token');
  }

  isLoggedIn(): boolean {
    return LoggedIn();
  }

  // ---------------- New Methods for Layout ----------------

  getUserName(): string {
    return UserName() || '';
  }

  hasRole(role: string): boolean {
    return UserRoles().includes(role);
  }

  getUser(): any {
    const user = localStorage.getItem('user');
    return user ? JSON.parse(user) : null;
  }
}
