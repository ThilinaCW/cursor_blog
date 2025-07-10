import { Component, OnInit } from '@angular/core';
import { BlogService } from '../blog.service';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './sidebar.component.html',
  styleUrl: './sidebar.component.scss'
})
export class SidebarComponent implements OnInit {
  categories: string[] = [];
  loading = true;

  constructor(private blogService: BlogService) {}

  ngOnInit() {
    this.loadCategories();
  }

  loadCategories() {
    this.blogService.getCategories().subscribe({
      next: categories => {
        this.categories = categories || [];
        this.loading = false;
      },
      error: () => {
        this.categories = [];
        this.loading = false;
      }
    });
  }
} 