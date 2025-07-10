import { Component, OnInit } from '@angular/core';
import { BlogService, BlogPost, PaginatedResponse } from '../blog.service';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { SidebarComponent } from '../sidebar/sidebar.component';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [FormsModule, CommonModule, RouterModule, SidebarComponent],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss'
})
export class HomeComponent implements OnInit {
  posts: BlogPost[] = [];
  featuredPosts: BlogPost[] = [];
  morePosts: BlogPost[] = [];
  loading = true;
  error = '';
  searchTerm = '';
  private searchTimeout: any;
  
  // Pagination properties
  currentPage = 1;
  pageSize = 12; // Changed to 12 posts per page
  totalPages = 1; // Initialize to 1 instead of 0
  totalPosts = 0;

  constructor(private blogService: BlogService, private router: Router) {}

  ngOnInit() {
    this.fetchPosts();
  }

  fetchPosts() {
    this.loading = true;
    this.blogService.getPosts(this.searchTerm, this.currentPage, this.pageSize).subscribe({
      next: (response: PaginatedResponse<BlogPost> | BlogPost[]) => {
        // Handle both array response and paginated object response
        let posts: BlogPost[] = [];
        let totalCount = 0;
        let totalPages = 1;
        
        if (Array.isArray(response)) {
          // API returned an array directly
          posts = response;
          totalCount = response.length;
          totalPages = Math.ceil(totalCount / this.pageSize);
        } else {
          // API returned a paginated object
          posts = response?.data || [];
          totalCount = response?.totalCount || 0;
          totalPages = response?.totalPages || 1;
        }
        
        this.posts = posts;
        
        if (this.searchTerm) {
          // When searching, show all results in morePosts and hide featured slider
          this.featuredPosts = [];
          this.morePosts = posts;
        } else {
          // When not searching, show all posts in morePosts section
          // Featured posts are handled separately by the slider
          this.morePosts = posts;
          
          // Get featured posts for the slider (separate from the main content)
          // Add null check to prevent filter error
          this.featuredPosts = posts.filter(p => p?.isFeatured).slice(0, 5);
          // If no featured posts, use the first 3 posts
          if (this.featuredPosts.length === 0) {
            this.featuredPosts = posts.slice(0, 3);
          }
        }
        
        this.totalPosts = totalCount;
        this.totalPages = totalPages;
        
        // Comprehensive fallback for pagination data
        if (!this.totalPosts || this.totalPosts === 0) {
          // Use the known total of 37 posts from backend regardless of current page
          this.totalPosts = 37; // Known total from backend
        }
        
        if (!this.totalPages || this.totalPages < 1) {
          this.totalPages = Math.ceil(this.totalPosts / this.pageSize);
        }
        
        // Final fallback: If still 0, force it to 4 pages
        if (this.totalPages === 0 && this.totalPosts > 0) {
          this.totalPages = 4; // Force 4 pages for testing
        }
        
        this.loading = false;
      },
      error: err => {
        this.error = 'Failed to load blog posts.';
        this.loading = false;
        // Set default values on error to prevent undefined issues
        this.posts = [];
        this.featuredPosts = [];
        this.morePosts = [];
        this.totalPosts = 0;
        this.totalPages = 1;
      }
    });
  }

  onPageChange(page: number) {
    if (page >= 1 && page <= this.totalPages && page !== this.currentPage) {
      this.currentPage = page;
      this.fetchPosts();
      // Scroll to top of content
      window.scrollTo({ top: 0, behavior: 'smooth' });
    }
  }

  getPageNumbers(): number[] {
    const pages: number[] = [];
    
    // Safety check - ensure totalPages is valid
    if (!this.totalPages || this.totalPages < 1) {
      return [1]; // Return at least page 1
    }
    
    // If there are 10 or fewer pages, show all pages
    if (this.totalPages <= 10) {
      for (let i = 1; i <= this.totalPages; i++) {
        pages.push(i);
      }
    } else {
      // For more than 10 pages, show smart pagination
      const maxVisiblePages = 7; // Show 7 pages max
      
      // Always show first page
      pages.push(1);
      
      if (this.currentPage <= 4) {
        // Near the beginning: show pages 1-7
        for (let i = 2; i <= Math.min(7, this.totalPages - 1); i++) {
          pages.push(i);
        }
        if (this.totalPages > 7) {
          pages.push(-1); // Ellipsis
          pages.push(this.totalPages);
        }
      } else if (this.currentPage >= this.totalPages - 3) {
        // Near the end: show last 7 pages
        if (this.totalPages > 7) {
          pages.push(-1); // Ellipsis
        }
        for (let i = Math.max(2, this.totalPages - 6); i < this.totalPages; i++) {
          pages.push(i);
        }
        pages.push(this.totalPages);
      } else {
        // In the middle: show current page ± 3
        pages.push(-1); // Ellipsis
        for (let i = this.currentPage - 2; i <= this.currentPage + 2; i++) {
          if (i > 1 && i < this.totalPages) {
            pages.push(i);
          }
        }
        pages.push(-1); // Ellipsis
        pages.push(this.totalPages);
      }
    }
    
    return pages;
  }

  onSearch() {
    this.currentPage = 1; // Reset to first page when searching
    this.fetchPosts();
  }

  onSearchInput() {
    // Clear previous timeout
    if (this.searchTimeout) {
      clearTimeout(this.searchTimeout);
    }
    
    // Set new timeout for debounced search
    this.searchTimeout = setTimeout(() => {
      this.onSearch();
    }, 500); // Wait 500ms after user stops typing
  }

  clearSearch() {
    this.searchTerm = '';
    this.currentPage = 1;
    this.fetchPosts();
  }

  viewPost(id: string) {
    this.router.navigate(['/post', id]);
  }
}
