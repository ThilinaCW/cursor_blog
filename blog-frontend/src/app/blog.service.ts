import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

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
}

export interface PaginatedResponse<T> {
  data: T[];
  totalCount: number;
  totalPages: number;
  currentPage: number;
  pageSize: number;
  hasMore: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class BlogService {
  private apiUrl = 'http://localhost:5062/api/blogposts';

  constructor(private http: HttpClient) { }

  getPosts(search?: string, page: number = 1, pageSize: number = 6): Observable<PaginatedResponse<BlogPost> | BlogPost[]> {
    let url = this.apiUrl;
    const params = new URLSearchParams();
    
    if (search) {
      params.append('search', search);
    }
    params.append('page', page.toString());
    params.append('pageSize', pageSize.toString());
    
    if (params.toString()) {
      url += `?${params.toString()}`;
    }
    
    return this.http.get<PaginatedResponse<BlogPost> | BlogPost[]>(url);
  }

  getFeaturedPosts(): Observable<BlogPost[]> {
    return this.http.get<BlogPost[]>(`${this.apiUrl}/featured`);
  }

  getPostsByCategory(category: string): Observable<BlogPost[]> {
    return this.http.get<BlogPost[]>(`${this.apiUrl}/categories/${encodeURIComponent(category)}`);
  }

  getPostsByAuthor(author: string): Observable<BlogPost[]> {
    return this.http.get<BlogPost[]>(`${this.apiUrl}/authors/${encodeURIComponent(author)}`);
  }

  getCategories(): Observable<string[]> {
    return this.http.get<string[]>(`${this.apiUrl}/categories`);
  }

  getAuthors(): Observable<string[]> {
    return this.http.get<string[]>(`${this.apiUrl}/authors`);
  }

  getPost(id: string): Observable<BlogPost> {
    return this.http.get<BlogPost>(`${this.apiUrl}/${id}`);
  }

  createPost(post: Omit<BlogPost, 'id' | 'createdDate'>): Observable<BlogPost> {
    return this.http.post<BlogPost>(this.apiUrl, post);
  }

  updatePost(id: string, post: Omit<BlogPost, 'id' | 'createdDate'>): Observable<BlogPost> {
    return this.http.put<BlogPost>(`${this.apiUrl}/${id}`, post);
  }
}
