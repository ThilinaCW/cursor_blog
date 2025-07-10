import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface Comment {
  id: string;
  content: string;
  blogPostId: string;
  userId: string;
  userName: string;
  userFirstName: string;
  userLastName: string;
  createdDate: string;
  updatedDate?: string;
  isActive: boolean;
  canEdit: boolean;
}

export interface CreateCommentRequest {
  content: string;
  blogPostId: string;
}

export interface UpdateCommentRequest {
  content: string;
}

@Injectable({
  providedIn: 'root'
})
export class CommentService {
  private apiUrl = 'http://localhost:5062/api/comments';

  constructor(private http: HttpClient) { }

  getCommentsByBlogPostId(blogPostId: string): Observable<Comment[]> {
    return this.http.get<Comment[]>(`${this.apiUrl}/post/${blogPostId}`);
  }

  getCommentById(id: string): Observable<Comment> {
    return this.http.get<Comment>(`${this.apiUrl}/${id}`);
  }

  createComment(comment: CreateCommentRequest): Observable<Comment> {
    const token = localStorage.getItem('authToken');
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    return this.http.post<Comment>(this.apiUrl, comment, { headers });
  }

  updateComment(id: string, comment: UpdateCommentRequest): Observable<Comment> {
    const token = localStorage.getItem('authToken');
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    return this.http.put<Comment>(`${this.apiUrl}/${id}`, comment, { headers });
  }

  deleteComment(id: string): Observable<void> {
    const token = localStorage.getItem('authToken');
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    return this.http.delete<void>(`${this.apiUrl}/${id}`, { headers });
  }
} 