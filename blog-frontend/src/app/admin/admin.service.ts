import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AuthService } from '../auth/auth.service';

export interface AdminDashboard {
  pendingUsers: number;
  pendingPosts: number;
  totalUsers: number;
  totalPosts: number;
}

export interface User {
  id: string;
  username: string;
  email: string;
  firstName: string;
  lastName: string;
  bio: string;
  profileImageUrl: string;
  role: string | number;
  isActive: boolean;
  isApproved: boolean;
  createdDate: string;
  lastLoginDate?: string;
  approvedDate?: string;
  approvedBy?: string;
}

export interface BlogPost {
  id: string;
  title: string;
  content: string;
  author: string;
  createdDate: string;
  imageUrl?: string;
  isFeatured?: boolean;
  categories?: string[];
  updatedDate?: string;
  isActive?: boolean;
  isApproved?: boolean;
  approvedDate?: string;
  approvedBy?: string;
  rejectionReason?: string;
}

export interface RejectRequest {
  reason: string;
}

@Injectable({
  providedIn: 'root'
})
export class AdminService {
  private apiUrl = 'http://localhost:5062/api/admin';

  constructor(private http: HttpClient, private authService: AuthService) { }

  private getHeaders(): HttpHeaders {
    const token = this.authService.getToken();
    console.log('JWT Token:', token);
    
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${token}`
    });
    
    console.log('Request Headers:', headers);
    return headers;
  }

  getDashboard(): Observable<AdminDashboard> {
    return this.http.get<AdminDashboard>(`${this.apiUrl}/dashboard`, { headers: this.getHeaders() });
  }

  getPendingUsers(): Observable<User[]> {
    return this.http.get<User[]>(`${this.apiUrl}/users/pending`, { headers: this.getHeaders() });
  }

  approveUser(userId: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/users/${userId}/approve`, {}, { headers: this.getHeaders() });
  }

  rejectUser(userId: string, reason: string): Observable<any> {
    const rejectRequest: RejectRequest = { reason };
    return this.http.post(`${this.apiUrl}/users/${userId}/reject`, rejectRequest, { headers: this.getHeaders() });
  }

  getPendingPosts(): Observable<BlogPost[]> {
    return this.http.get<BlogPost[]>(`${this.apiUrl}/posts/pending`, { headers: this.getHeaders() });
  }

  approvePost(postId: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/posts/${postId}/approve`, {}, { headers: this.getHeaders() });
  }

  rejectPost(postId: string, reason: string): Observable<any> {
    const rejectRequest: RejectRequest = { reason };
    return this.http.post(`${this.apiUrl}/posts/${postId}/reject`, rejectRequest, { headers: this.getHeaders() });
  }

  deletePost(postId: string): Observable<any> {
    return this.http.delete(`${this.apiUrl}/posts/${postId}`, { headers: this.getHeaders() });
  }
} 