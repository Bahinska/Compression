import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from './components/login/login.component';
import { VideoStreamComponent } from './components/video-stream/video-stream.component';
import { authGuard } from './guards/auth.guard';
import { SelectionComponent } from './components/selection/selection.component';

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
  { path: '', redirectTo: '/selection', pathMatch: 'full' }, // Redirect to stream if authenticated
  { path: '**', redirectTo: '/login' }
];


@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
