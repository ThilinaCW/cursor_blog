import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { BlogService, BlogPost } from '../blog.service';
import { CommonModule } from '@angular/common';

const DEFAULT_IMAGE = 'https://images.unsplash.com/photo-1506744038136-46273834b3fb?auto=format&fit=crop&w=800&q=80';

@Component({
  selector: 'app-edit-post',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule],
  templateUrl: './edit-post.component.html',
  styleUrl: './edit-post.component.scss'
})
export class EditPostComponent implements OnInit {
  postForm: FormGroup;
  loading = true;
  error = '';
  postId!: string;

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private blogService: BlogService,
    private router: Router
  ) {
    this.postForm = this.fb.group({
      title: ['', Validators.required],
      content: ['', Validators.required],
      author: ['', Validators.required],
      imageUrl: [''],
      isFeatured: [false],
      categories: ['']
    });
  }

  ngOnInit() {
    const id = this.route.snapshot.paramMap.get('id');
    if (!id) {
      this.error = 'Invalid post ID.';
      this.loading = false;
      return;
    }
    
    this.postId = id;
    this.blogService.getPost(this.postId).subscribe({
      next: (post: BlogPost) => {
        this.postForm.patchValue({
          title: post.title,
          content: post.content,
          author: post.author,
          imageUrl: post.imageUrl,
          isFeatured: post.isFeatured,
          categories: post.categories ? post.categories.join(', ') : ''
        });
        this.loading = false;
      },
      error: () => {
        this.error = 'Failed to load post.';
        this.loading = false;
      }
    });
  }

  submit() {
    if (this.postForm.invalid) return;
    this.loading = true;
    const value = this.postForm.value;
    if (!value.imageUrl) {
      value.imageUrl = DEFAULT_IMAGE;
    }
    // Convert categories string to array
    if (value.categories) {
      value.categories = value.categories.split(',').map((cat: string) => cat.trim()).filter((cat: string) => cat.length > 0);
    } else {
      value.categories = [];
    }
    this.blogService.updatePost(this.postId, value).subscribe({
      next: () => this.router.navigate(['/post', this.postId]),
      error: () => {
        this.error = 'Failed to update post.';
        this.loading = false;
      }
    });
  }
}
