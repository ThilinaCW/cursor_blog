import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { BlogService, BlogPost } from '../blog.service';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-category',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './category.component.html',
  styleUrl: './category.component.scss'
})
export class CategoryComponent implements OnInit {
  posts: BlogPost[] = [];
  category: string = '';
  loading = true;
  error = '';

  constructor(
    private blogService: BlogService,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit() {
    this.route.params.subscribe(params => {
      this.category = params['category'];
      this.fetchPostsByCategory();
    });
  }

  fetchPostsByCategory() {
    this.loading = true;
    this.blogService.getPostsByCategory(this.category).subscribe({
      next: posts => {
        this.posts = posts || [];
        this.loading = false;
      },
      error: err => {
        this.error = 'Failed to load posts for this category.';
        this.posts = [];
        this.loading = false;
      }
    });
  }

  viewPost(id: string) {
    this.router.navigate(['/post', id]);
  }
} 