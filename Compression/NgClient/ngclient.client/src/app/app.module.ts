import { HttpClientModule } from '@angular/common/http';
import { CUSTOM_ELEMENTS_SCHEMA, NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppComponent } from './app.component';
import { VideoStreamComponent } from './components/video-stream/video-stream.component';
import { LoginComponent } from './components/login/login.component';
import { AppRoutingModule } from './app-routing.module';
import { ReactiveFormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MaterialModule } from './material.module';
import { AuthService } from './services/auth-service.service';
import { SelectionComponent } from './components/selection/selection.component';
import { RegistrationComponent } from './components/registration/registration.component';
import { UserListComponent } from './components/user-list/user-list.component';
import { MatTableModule } from '@angular/material/table';
import { EditUserDialogComponent } from './components/edit-user-dialog/edit-user-dialog.component';
import { MatDialogModule } from '@angular/material/dialog';
import {MatIconModule} from '@angular/material/icon'

@NgModule({
  declarations: [
    AppComponent,
    VideoStreamComponent,
    LoginComponent,
    SelectionComponent,
    RegistrationComponent,
    UserListComponent,
    EditUserDialogComponent
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    AppRoutingModule,
    ReactiveFormsModule,
    BrowserAnimationsModule,
    MaterialModule,
    MatTableModule,
    MatDialogModule,
    MatIconModule
  ],
  providers: [AuthService],
  bootstrap: [AppComponent],
  schemas: [CUSTOM_ELEMENTS_SCHEMA]
})
export class AppModule { }
