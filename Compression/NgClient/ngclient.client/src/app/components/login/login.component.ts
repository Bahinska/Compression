import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from 'src/app/services/auth-service.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent {
  loginForm: FormGroup;
  submitted = false;

  constructor(
    private formBuilder: FormBuilder,
    private router: Router,
     private authService: AuthService 
  ) {
    this.loginForm = this.formBuilder.group({
      username: ['', Validators.required],
      password: ['', Validators.required]
    });

    // Redirect to stream if already logged in
    if (this.authService.isAuthenticated()) {
      this.router.navigate(['/stream']);
    }
  }

  get f() { return this.loginForm.controls; }

  onSubmit() {
    this.submitted = true;
    if (this.loginForm.invalid) {
      return;
    }

    try {
      this.authService.getJwtToken(this.loginForm.value.username, this.loginForm.value.password).then((token)=>{
        localStorage.setItem('jwtToken', token);
        this.router.navigate(['/stream']);
      });
    } catch(error: any) {
      console.error("Login failed", error);
    }
  }
}