import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';
import { tap } from 'rxjs/operators';

export interface User {
  id: string;
  username: string;
  email: string;
  firstName: string;
  lastName: string;
  createdDate: string;
  lastLoginDate?: string;
  isActive: boolean;
  role: string | number;
  bio?: string;
  profileImageUrl?: string;
}

export interface CreateUserDto {
  username: string;
  email: string;
  password: string;
  firstName: string;
  lastName: string;
  bio?: string;
}

export interface LoginDto {
  username: string;
  password: string;
}

export interface LoginResponseDto {
  token: string;
  user: User;
  expiresAt: string;
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = 'http://localhost:5062/api/users';
  private currentUserSubject = new BehaviorSubject<User | null>(null);
  public currentUser$ = this.currentUserSubject.asObservable();

  constructor(private http: HttpClient) {
    // Check if user is already logged in from localStorage (browser only)
    if (typeof window !== 'undefined') {
      const token = localStorage.getItem('authToken');
      const user = localStorage.getItem('currentUser');
      if (token && user) {
        this.currentUserSubject.next(JSON.parse(user));
      }
    }
  }

  register(userData: CreateUserDto): Observable<User> {
    return this.http.post<User>(`${this.apiUrl}/register`, userData);
  }

  login(loginData: LoginDto): Observable<LoginResponseDto> {
    return this.http.post<LoginResponseDto>(`${this.apiUrl}/login`, loginData)
      .pipe(
        tap(response => {
          console.log('Login response:', response);
          console.log('JWT Token received:', response.token);
          if (typeof window !== 'undefined') {
            // Store token and user data
            localStorage.setItem('authToken', response.token);
            localStorage.setItem('currentUser', JSON.stringify(response.user));
          }
          this.currentUserSubject.next(response.user);
        })
      );
  }

  logout(): void {
    if (typeof window !== 'undefined') {
      localStorage.removeItem('authToken');
      localStorage.removeItem('currentUser');
    }
    this.currentUserSubject.next(null);
  }

  getCurrentUser(): User | null {
    return this.currentUserSubject.value;
  }

  isLoggedIn(): boolean {
    return this.currentUserSubject.value !== null;
  }

  getToken(): string | null {
    if (typeof window !== 'undefined') {
      const token = localStorage.getItem('authToken');
      console.log('getToken() called, localStorage authToken:', token);
      return token;
    }
    return null;
  }

  checkUsernameAvailability(username: string): Observable<{ exists: boolean }> {
    return this.http.get<{ exists: boolean }>(`${this.apiUrl}/check-username/${username}`);
  }

  checkEmailAvailability(email: string): Observable<{ exists: boolean }> {
    return this.http.get<{ exists: boolean }>(`${this.apiUrl}/check-email/${email}`);
  }

  isAdmin(): boolean {
    const user = this.getCurrentUser();
    if (!user?.role) return false;
    
    // Check for string 'Admin' or number 1 (Admin enum value)
    return user.role === 'Admin' || user.role === 1;
  }
} 