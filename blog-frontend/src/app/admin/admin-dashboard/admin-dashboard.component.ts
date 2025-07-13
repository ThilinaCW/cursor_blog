import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AdminService, AdminDashboard, User, BlogPost } from '../admin.service';
import { Router, RouterModule } from '@angular/router';

@Component({
  selector: 'app-admin-dashboard',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './admin-dashboard.component.html',
  styleUrl: './admin-dashboard.component.scss'
})
export class AdminDashboardComponent implements OnInit {
  dashboard: AdminDashboard | null = null;
  pendingUsers: User[] = [];
  pendingPosts: BlogPost[] = [];
  loading = true;
  error = '';
  activeTab = 'dashboard';
  rejectionReason = '';

  constructor(private adminService: AdminService, private router: Router) {}

  ngOnInit() {
    setTimeout(() => {
      this.loadDashboard(); // Always load dashboard stats
      const tab = (window.location.search.match(/tab=([^&]+)/) || [])[1];
      if (tab) {
        this.activeTab = tab;
        if (tab === 'users') {
          this.loadPendingUsers();
        } else if (tab === 'posts') {
          this.loadPendingPosts();
        }
      }
    }, 100);
  }

  loadDashboard() {
    this.loading = true;
    this.adminService.getDashboard().subscribe({
      next: (data) => {
        this.dashboard = data;
        this.loading = false;
      },
      error: (err) => {
        console.error('Error loading dashboard:', err);
        this.error = 'Failed to load dashboard data.';
        this.loading = false;
      }
    });
  }

  loadPendingUsers() {
    this.loading = true;
    this.adminService.getPendingUsers().subscribe({
      next: (users) => {
        this.pendingUsers = users;
        this.loading = false;
      },
      error: (err) => {
        console.error('Error loading pending users:', err);
        this.error = 'Failed to load pending users.';
        this.loading = false;
      }
    });
  }

  loadPendingPosts() {
    this.loading = true;
    this.adminService.getPendingPosts().subscribe({
      next: (posts) => {
        this.pendingPosts = posts;
        this.loading = false;
      },
      error: (err) => {
        console.error('Error loading pending posts:', err);
        this.error = 'Failed to load pending posts.';
        this.loading = false;
      }
    });
  }

  approveUser(userId: string) {
    this.adminService.approveUser(userId).subscribe({
      next: () => {
        this.pendingUsers = this.pendingUsers.filter(u => u.id !== userId);
        this.loadDashboard(); // Refresh dashboard stats
      },
      error: (err) => {
        this.error = 'Failed to approve user.';
      }
    });
  }

  rejectUser(userId: string) {
    if (!this.rejectionReason.trim()) {
      this.error = 'Please provide a rejection reason.';
      return;
    }

    this.adminService.rejectUser(userId, this.rejectionReason).subscribe({
      next: () => {
        this.pendingUsers = this.pendingUsers.filter(u => u.id !== userId);
        this.rejectionReason = '';
        this.loadDashboard(); // Refresh dashboard stats
      },
      error: (err) => {
        this.error = 'Failed to reject user.';
      }
    });
  }

  approvePost(postId: string) {
    this.adminService.approvePost(postId).subscribe({
      next: () => {
        this.pendingPosts = this.pendingPosts.filter(p => p.id !== postId);
        this.loadDashboard(); // Refresh dashboard stats
      },
      error: (err) => {
        this.error = 'Failed to approve post.';
      }
    });
  }

  rejectPost(postId: string) {
    if (!this.rejectionReason.trim()) {
      this.error = 'Please provide a rejection reason.';
      return;
    }

    this.adminService.rejectPost(postId, this.rejectionReason).subscribe({
      next: () => {
        this.pendingPosts = this.pendingPosts.filter(p => p.id !== postId);
        this.rejectionReason = '';
        this.loadDashboard(); // Refresh dashboard stats
      },
      error: (err) => {
        this.error = 'Failed to reject post.';
      }
    });
  }

  deletePost(postId: string) {
    if (!confirm('Are you sure you want to delete this post?')) return;
    this.adminService.deletePost(postId).subscribe({
      next: () => {
        this.pendingPosts = this.pendingPosts.filter(p => p.id !== postId);
        this.loadDashboard();
      },
      error: () => {
        this.error = 'Failed to delete post.';
      }
    });
  }

  setActiveTab(tab: string) {
    this.activeTab = tab;
    this.error = '';
    
    if (tab === 'users') {
      this.loadPendingUsers();
    } else if (tab === 'posts') {
      this.loadPendingPosts();
    }
  }

  getFullName(user: User): string {
    return `${user.firstName} ${user.lastName}`.trim() || user.username;
  }

  formatDate(dateString: string): string {
    return new Date(dateString).toLocaleDateString();
  }

  viewPost(postId: string) {
    this.router.navigate(['/admin/posts', postId], { queryParams: { backTo: '/admin?tab=posts' } });
  }
} 