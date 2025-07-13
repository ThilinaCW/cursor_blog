import { Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { BlogDetailComponent } from './blog-detail/blog-detail.component';
import { CreatePostComponent } from './create-post/create-post.component';
import { EditPostComponent } from './edit-post/edit-post.component';
import { CategoryComponent } from './category/category.component';
import { AuthorComponent } from './author/author.component';
import { AboutComponent } from './about/about.component';
import { LoginComponent } from './auth/login/login.component';
import { SignupComponent } from './auth/signup/signup.component';
import { AdminDashboardComponent } from './admin/admin-dashboard/admin-dashboard.component';
import { AdminRedirectGuard } from './auth/admin-redirect.guard';
import { AuthGuard } from './auth/auth.guard';
import { MyPostsComponent } from './my-posts.component';

export const routes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'post/:id', component: BlogDetailComponent },
  { path: 'create', component: CreatePostComponent, canActivate: [AuthGuard] },
  { path: 'edit/:id', component: EditPostComponent },
  { path: 'category/:category', component: CategoryComponent },
  { path: 'author/:author', component: AuthorComponent },
  { path: 'about', component: AboutComponent },
  { path: 'login', component: LoginComponent },
  { path: 'signup', component: SignupComponent },
  { path: 'admin', component: AdminDashboardComponent },
  { path: 'admin/posts/:id', component: BlogDetailComponent },
  { path: 'my-posts', component: MyPostsComponent, canActivate: [AuthGuard] }
];
