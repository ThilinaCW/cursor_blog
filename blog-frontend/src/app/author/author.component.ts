import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { BlogService, BlogPost } from '../blog.service';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-author',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './author.component.html',
  styleUrl: './author.component.scss'
})
export class AuthorComponent implements OnInit {
  posts: BlogPost[] = [];
  author: string = '';
  loading = true;
  error = '';

  constructor(
    private blogService: BlogService,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit() {
    this.route.params.subscribe(params => {
      this.author = params['author'];
      this.fetchPostsByAuthor();
    });
  }

  fetchPostsByAuthor() {
    this.loading = true;
    this.blogService.getPostsByAuthor(this.author).subscribe({
      next: posts => {
        this.posts = posts || [];
        this.loading = false;
      },
      error: err => {
        this.error = 'Failed to load posts by this author.';
        this.posts = [];
        this.loading = false;
      }
    });
  }

  viewPost(id: string) {
    this.router.navigate(['/post', id]);
  }
} 