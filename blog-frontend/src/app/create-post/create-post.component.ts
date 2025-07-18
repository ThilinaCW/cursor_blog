import { Component, OnInit, Inject, PLATFORM_ID } from '@angular/core';
import { Router } from '@angular/router';
import { BlogService } from '../blog.service';
import { CommonModule } from '@angular/common';
import { QuillModule } from 'ngx-quill';
import { FormsModule } from '@angular/forms';
import { isPlatformBrowser } from '@angular/common';

const DEFAULT_IMAGE = 'https://images.unsplash.com/photo-1506744038136-46273834b3fb?auto=format&fit=crop&w=800&q=80';

@Component({
  selector: 'app-create-post',
  standalone: true,
  imports: [FormsModule, CommonModule, QuillModule],
  templateUrl: './create-post.component.html',
  styleUrl: './create-post.component.scss'
})
export class CreatePostComponent implements OnInit {
  title = '';
  content = '';
  author = '';
  imageUrl = '';
  isFeatured = false;
  categories = '';
  error = '';
  loading = false;

  modules = {
    toolbar: [
      ['bold', 'italic', 'underline', 'strike'],
      ['blockquote', 'code-block'],
      [{ 'header': 1 }, { 'header': 2 }],
      [{ 'list': 'ordered'}, { 'list': 'bullet' }],
      [{ 'script': 'sub'}, { 'script': 'super' }],
      [{ 'indent': '-1'}, { 'indent': '+1' }],
      [{ 'direction': 'rtl' }],
      [{ 'size': ['small', false, 'large', 'huge'] }],
      [{ 'header': [1, 2, 3, 4, 5, 6, false] }],
      [{ 'color': [] }, { 'background': [] }],
      [{ 'font': [] }],
      [{ 'align': [] }],
      ['clean'],
      ['image']
    ],
    imageResize: {
      // You can add config options here if needed
    }
  };

  constructor(
    private blogService: BlogService,
    private router: Router,
    @Inject(PLATFORM_ID) private platformId: Object
  ) {
  }

  async ngOnInit() {
    const user = JSON.parse(localStorage.getItem('currentUser') || '{}');
    this.author = `${user.firstName || ''} ${user.lastName || ''}`.trim();
  }

  submit() {
    this.loading = true;
    const user = JSON.parse(localStorage.getItem('currentUser') || '{}');
    const userId = user.id || '';
    const value: any = {
      title: this.title,
      content: this.content,
      author: this.author,
      imageUrl: this.imageUrl || DEFAULT_IMAGE,
      isFeatured: this.isFeatured,
      categories: this.categories
        ? this.categories.split(',').map((cat: string) => cat.trim()).filter((cat: string) => cat.length > 0)
        : [],
      userId
    };
    this.blogService.createPost(value).subscribe({
      next: () => this.router.navigate(['/']),
      error: () => {
        this.error = 'Failed to create post.';
        this.loading = false;
      }
    });
  }
}
