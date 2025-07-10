import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { BlogService } from '../blog.service';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';

const DEFAULT_IMAGE = 'https://images.unsplash.com/photo-1506744038136-46273834b3fb?auto=format&fit=crop&w=800&q=80';

@Component({
  selector: 'app-create-post',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule],
  templateUrl: './create-post.component.html',
  styleUrl: './create-post.component.scss'
})
export class CreatePostComponent {
  postForm: FormGroup;
  loading = false;
  error = '';

  constructor(private fb: FormBuilder, private blogService: BlogService, private router: Router) {
    this.postForm = this.fb.group({
      title: ['', Validators.required],
      content: ['', Validators.required],
      author: ['', Validators.required],
      imageUrl: [''],
      isFeatured: [false],
      categories: ['']
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
    this.blogService.createPost(value).subscribe({
      next: () => this.router.navigate(['/']),
      error: () => {
        this.error = 'Failed to create post.';
        this.loading = false;
      }
    });
  }
}
