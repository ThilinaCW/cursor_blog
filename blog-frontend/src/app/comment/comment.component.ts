import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Comment, CommentService } from '../services/comment.service';

@Component({
  selector: 'app-comment',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './comment.component.html',
  styleUrls: ['./comment.component.scss']
})
export class CommentComponent {
  @Input() comment!: Comment;
  @Output() commentUpdated = new EventEmitter<Comment>();
  @Output() commentDeleted = new EventEmitter<string>();

  isEditing = false;
  editContent = '';

  constructor(private commentService: CommentService) {}

  startEdit(): void {
    this.isEditing = true;
    this.editContent = this.comment.content;
  }

  cancelEdit(): void {
    this.isEditing = false;
    this.editContent = '';
  }

  saveEdit(): void {
    if (this.editContent.trim()) {
      this.commentService.updateComment(this.comment.id, { content: this.editContent.trim() })
        .subscribe({
          next: (updatedComment) => {
            this.comment = updatedComment;
            this.isEditing = false;
            this.editContent = '';
            this.commentUpdated.emit(updatedComment);
          },
          error: (error) => {
            console.error('Error updating comment:', error);
            alert('Failed to update comment. Please try again.');
          }
        });
    }
  }

  deleteComment(): void {
    if (confirm('Are you sure you want to delete this comment?')) {
      this.commentService.deleteComment(this.comment.id)
        .subscribe({
          next: () => {
            this.commentDeleted.emit(this.comment.id);
          },
          error: (error) => {
            console.error('Error deleting comment:', error);
            alert('Failed to delete comment. Please try again.');
          }
        });
    }
  }
} 