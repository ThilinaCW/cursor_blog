import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { BlogService, BlogPost } from '../blog.service';
import { CommonModule } from '@angular/common';
import { QuillModule } from 'ngx-quill';
import { FormsModule } from '@angular/forms';
import Quill from 'quill';

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

  quillInstance: Quill | null = null;

  constructor(
    private route: ActivatedRoute,
    private blogService: BlogService,
    private router: Router
  ) {}

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
        this.title = post.title || '';
        this.content = post.content || '';
        this.author = post.author || '';
        this.imageUrl = post.imageUrl || '';
        this.isFeatured = post.isFeatured ?? false;
        this.categories = post.categories ? post.categories.join(', ') : '';
        this.userId = (post as any).userId || '';
        this.loading = false;
        if (this.quillInstance && this.content) {
          this.setQuillContent(this.content);
        }
      },
      error: () => {
        this.error = 'Failed to load post.';
        this.loading = false;
      }
    });
  }

  onEditorCreated(editor: Quill) {
    this.quillInstance = editor;
    if (this.content) {
      this.setQuillContent(this.content);
    }
  }

  setQuillContent(content: string) {
    if (this.quillInstance) {
      // If your content is HTML, use dangerouslyPasteHTML
      this.quillInstance.clipboard.dangerouslyPasteHTML(content);
      // If your content is a Delta, use setContents
      // this.quillInstance.setContents(delta);
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
