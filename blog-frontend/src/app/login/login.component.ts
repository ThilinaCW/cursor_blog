import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule, RouterModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
export class LoginComponent {
  loginForm: FormGroup;
  loading = false;
  error = '';

  constructor(private fb: FormBuilder, private router: Router) {
    this.loginForm = this.fb.group({
      username: ['', Validators.required],
      password: ['', Validators.required]
    });
  }

  submit() {
    if (this.loginForm.invalid) return;
    
    this.loading = true;
    this.error = '';
    
    const { username, password } = this.loginForm.value;
    
    // For now, we'll simulate a login process
    // In a real app, this would call an authentication service
    setTimeout(() => {
      if (username === 'admin' && password === 'password') {
        // Successful login
        this.router.navigate(['/']);
      } else {
        this.error = 'Invalid username or password';
      }
      this.loading = false;
    }, 1000);
  }
} 