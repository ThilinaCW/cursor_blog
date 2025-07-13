import { Component, OnInit } from '@angular/core';
import { BlogService, BlogPost } from './blog.service';
import { AuthService } from './auth/auth.service';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-my-posts',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './my-posts.component.html',
  styleUrl: './my-posts.component.scss'
})
export class MyPostsComponent implements OnInit {
  posts: BlogPost[] = [];
  loading = true;
  error = '';
  currentUser: any;

  constructor(private blogService: BlogService, private authService: AuthService) {}

  ngOnInit() {
    this.currentUser = this.authService.getCurrentUser();
    if (!this.currentUser) {
      this.error = 'You must be logged in to view your posts.';
      this.loading = false;
      return;
    }
    this.blogService.getPostsByUserId(this.currentUser.id).subscribe({
      next: (posts) => {
        this.posts = posts || [];
        this.loading = false;
      },
      error: () => {
        this.error = 'Failed to load your posts.';
        this.loading = false;
      }
    });
  }

  getContentPreview(html: string, maxLength: number = 120): string {
    const div = document.createElement('div');
    div.innerHTML = html;
    const text = div.innerText || div.textContent || '';
    return text.length > maxLength ? text.slice(0, maxLength) + '...' : text;
  }
}
