import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService, CreateUserDto } from '../auth.service';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-signup',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './signup.component.html',
  styleUrl: './signup.component.scss'
})
export class SignupComponent implements OnInit {
  signupForm!: FormGroup;
  loading = false;
  error = '';
  success = '';
  usernameAvailable = true;
  emailAvailable = true;
  checkingUsername = false;
  checkingEmail = false;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit() {
    this.initForm();
  }

  initForm() {
    this.signupForm = this.fb.group({
      username: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(50)]],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      confirmPassword: ['', [Validators.required]],
      firstName: ['', [Validators.required, Validators.maxLength(50)]],
      lastName: ['', [Validators.required, Validators.maxLength(50)]],
      bio: ['', [Validators.maxLength(500)]]
    }, { validators: this.passwordMatchValidator });
  }

  passwordMatchValidator(form: FormGroup) {
    const password = form.get('password');
    const confirmPassword = form.get('confirmPassword');
    
    if (password && confirmPassword && password.value !== confirmPassword.value) {
      confirmPassword.setErrors({ passwordMismatch: true });
      return { passwordMismatch: true };
    }
    
    return null;
  }

  onUsernameChange() {
    const username = this.signupForm.get('username')?.value;
    if (username && username.length >= 3) {
      this.checkingUsername = true;
      this.authService.checkUsernameAvailability(username).subscribe({
        next: (response) => {
          this.usernameAvailable = !response.exists;
          this.checkingUsername = false;
        },
        error: () => {
          this.checkingUsername = false;
        }
      });
    }
  }

  onEmailChange() {
    const email = this.signupForm.get('email')?.value;
    if (email && this.signupForm.get('email')?.valid) {
      this.checkingEmail = true;
      this.authService.checkEmailAvailability(email).subscribe({
        next: (response) => {
          this.emailAvailable = !response.exists;
          this.checkingEmail = false;
        },
        error: () => {
          this.checkingEmail = false;
        }
      });
    }
  }

  onSubmit() {
    if (this.signupForm.valid && this.usernameAvailable && this.emailAvailable) {
      this.loading = true;
      this.error = '';
      this.success = '';

      const userData: CreateUserDto = {
        username: this.signupForm.value.username,
        email: this.signupForm.value.email,
        password: this.signupForm.value.password,
        firstName: this.signupForm.value.firstName,
        lastName: this.signupForm.value.lastName,
        bio: this.signupForm.value.bio || undefined
      };

      this.authService.register(userData).subscribe({
        next: (user) => {
          this.success = 'Account created successfully! Redirecting to login...';
          this.loading = false;
          setTimeout(() => {
            this.router.navigate(['/login']);
          }, 2000);
        },
        error: (err) => {
          this.loading = false;
          if (err.error?.message) {
            this.error = err.error.message;
          } else {
            this.error = 'An error occurred while creating your account. Please try again.';
          }
        }
      });
    } else {
      this.markFormGroupTouched();
    }
  }

  markFormGroupTouched() {
    Object.keys(this.signupForm.controls).forEach(key => {
      const control = this.signupForm.get(key);
      control?.markAsTouched();
    });
  }

  getFieldError(fieldName: string): string {
    const field = this.signupForm.get(fieldName);
    if (field?.errors && field.touched) {
      if (field.errors['required']) return `${fieldName.charAt(0).toUpperCase() + fieldName.slice(1)} is required.`;
      if (field.errors['email']) return 'Please enter a valid email address.';
      if (field.errors['minlength']) return `${fieldName.charAt(0).toUpperCase() + fieldName.slice(1)} must be at least ${field.errors['minlength'].requiredLength} characters.`;
      if (field.errors['maxlength']) return `${fieldName.charAt(0).toUpperCase() + fieldName.slice(1)} must be no more than ${field.errors['maxlength'].requiredLength} characters.`;
      if (field.errors['passwordMismatch']) return 'Passwords do not match.';
    }
    return '';
  }
} 