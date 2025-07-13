import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { BlogService, BlogPost } from '../blog.service';
import { CommonModule } from '@angular/common';
import { QuillModule } from 'ngx-quill';
import { FormsModule } from '@angular/forms';
import { isPlatformBrowser } from '@angular/common';
import { Inject, PLATFORM_ID } from '@angular/core';

const DEFAULT_IMAGE = 'https://images.unsplash.com/photo-1506744038136-46273834b3fb?auto=format&fit=crop&w=800&q=80';

@Component({
  selector: 'app-edit-post',
  standalone: true,
  imports: [FormsModule, CommonModule, QuillModule],
  templateUrl: './edit-post.component.html',
  styleUrl: './edit-post.component.scss'
})
export class EditPostComponent implements OnInit {
  loading = true;
  error = '';
  postId!: string;

  // Direct properties for ngModel
  title = '';
  content = '';
  author = '';
  imageUrl = '';
  isFeatured = false;
  categories = '';
  userId: string = '';

  quillInstance: any | null = null;
  contentLoaded = false;

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
    private route: ActivatedRoute,
    private blogService: BlogService,
    private router: Router,
    @Inject(PLATFORM_ID) private platformId: Object
  ) {
  }

  async ngOnInit() {
    const id = this.route.snapshot.paramMap.get('id');
    if (!id) {
      this.error = 'Invalid post ID.';
      this.loading = false;
      return;
    }
    this.postId = id;
    this.blogService.getPost(this.postId).subscribe({
      next: (post: BlogPost) => {
        this.title = post.title || '';
        this.content = post.content || '';
        this.author = post.author || '';
        this.imageUrl = post.imageUrl || '';
        this.isFeatured = post.isFeatured ?? false;
        this.categories = post.categories ? post.categories.join(', ') : '';
        this.userId = (post as any).userId || '';
        this.loading = false;
        this.contentLoaded = true;
        this.trySetQuillContent();
      },
      error: () => {
        this.error = 'Failed to load post.';
        this.loading = false;
      }
    });
  }

  onEditorCreated(editor: any) {
    this.quillInstance = editor;
    this.trySetQuillContent();
  }

  trySetQuillContent() {
    debugger
    if (this.quillInstance && this.contentLoaded) {
      window.setTimeout(() => {
        this.quillInstance.clipboard.dangerouslyPasteHTML(this.content);     
      }, 500);
     
    }
  }

  submit() {
    this.loading = true;
    const value: any = {
      title: this.title,
      content: this.content,
      author: this.author,
      imageUrl: this.imageUrl || DEFAULT_IMAGE,
      isFeatured: this.isFeatured,
      categories: this.categories
        ? this.categories.split(',').map((cat: string) => cat.trim()).filter((cat: string) => cat.length > 0)
        : [],
      userId: this.userId
    };
    this.blogService.updatePost(this.postId, value).subscribe({
      next: () => this.router.navigate(['/post', this.postId]),
      error: () => {
        this.error = 'Failed to update post.';
        this.loading = false;
      }
    });
  }
}
