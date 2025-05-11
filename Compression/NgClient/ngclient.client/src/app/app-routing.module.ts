import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from './components/login/login.component';
import { VideoStreamComponent } from './components/video-stream/video-stream.component';
import { authGuard } from './guards/auth.guard';
import { SelectionComponent } from './components/selection/selection.component';
import { RegistrationComponent } from './components/registration/registration.component';
import { UserListComponent } from './components/user-list/user-list.component';

const routes: Routes = [
  { path: 'login', component: LoginComponent },
  {
    path: 'stream',
    component: VideoStreamComponent,
    canActivate: [authGuard],
  },
  {
    path: 'selection',
    component: SelectionComponent,
    canActivate: [authGuard],
  },
  {
    path: 'admin',
    component: RegistrationComponent,
    canActivate: [authGuard],
  },
  { path: 'admin/users', component: UserListComponent },
  { path: '', redirectTo: '/selection', pathMatch: 'full' },
  { path: '**', redirectTo: '/login' }
];


@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
