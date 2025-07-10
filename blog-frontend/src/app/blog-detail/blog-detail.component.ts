import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { BlogService, BlogPost } from '../blog.service';
import { AuthService, User } from '../auth/auth.service';
import { CommentService, Comment } from '../services/comment.service';
import { RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CommentComponent } from '../comment/comment.component';

@Component({
  selector: 'app-blog-detail',
  standalone: true,
  imports: [RouterModule, CommonModule, FormsModule, CommentComponent],
  templateUrl: './blog-detail.component.html',
  styleUrl: './blog-detail.component.scss'
})
export class BlogDetailComponent implements OnInit {
  post?: BlogPost;
  loading = true;
  error = '';
  currentUser: User | null = null;
  canEdit = false;
  
  // Comment properties
  comments: Comment[] = [];
  loadingComments = false;
  newComment = '';
  submittingComment = false;

  backTo: string | null = null;

  constructor(
    private route: ActivatedRoute, 
    private blogService: BlogService, 
    private router: Router,
    private authService: AuthService,
    private commentService: CommentService
  ) {}

  ngOnInit() {
    // Subscribe to current user
    this.authService.currentUser$.subscribe(user => {
      this.currentUser = user;
      this.checkEditPermission();
    });

    const id = this.route.snapshot.paramMap.get('id');
    this.backTo = this.route.snapshot.queryParamMap.get('backTo');
    if (!id) {
      this.error = 'Invalid blog post ID.';
      this.loading = false;
      return;
    }

    this.blogService.getPost(id).subscribe({
      next: post => {
        this.post = post;
        this.loading = false;
        this.checkEditPermission();
        this.loadComments();
      },
      error: err => {
        this.error = 'Blog post not found.';
        this.loading = false;
      }
    });
  }

  checkEditPermission() {
    if (this.post && this.currentUser) {
      // Check if current user is the author of the post
      this.canEdit = this.currentUser.username === this.post.author;
    } else {
      this.canEdit = false;
    }
  }

  loadComments() {
    if (!this.post) return;
    
    this.loadingComments = true;
    this.commentService.getCommentsByBlogPostId(this.post.id).subscribe({
      next: (comments) => {
        this.comments = comments;
        this.loadingComments = false;
      },
      error: (error) => {
        console.error('Error loading comments:', error);
        this.loadingComments = false;
      }
    });
  }

  submitComment() {
    if (!this.post || !this.newComment.trim() || this.submittingComment) return;

    this.submittingComment = true;
    const commentData = {
      content: this.newComment.trim(),
      blogPostId: this.post.id
    };

    this.commentService.createComment(commentData).subscribe({
      next: (newComment) => {
        this.comments.unshift(newComment); // Add to beginning of array
        this.newComment = '';
        this.submittingComment = false;
      },
      error: (error) => {
        console.error('Error creating comment:', error);
        alert('Failed to create comment. Please try again.');
        this.submittingComment = false;
      }
    });
  }

  onCommentUpdated(updatedComment: Comment) {
    const index = this.comments.findIndex(c => c.id === updatedComment.id);
    if (index !== -1) {
      this.comments[index] = updatedComment;
    }
  }

  onCommentDeleted(commentId: string) {
    this.comments = this.comments.filter(c => c.id !== commentId);
  }

  goBack() {
    if (this.backTo) {
      // Parse path and query params
      const [path, queryString] = this.backTo.split('?');
      const queryParams: any = {};
      if (queryString) {
        queryString.split('&').forEach(pair => {
          const [key, value] = pair.split('=');
          queryParams[key] = value;
        });
      }
      this.router.navigate([path], { queryParams });
    } else {
      this.router.navigate(['/']);
    }
  }
}
