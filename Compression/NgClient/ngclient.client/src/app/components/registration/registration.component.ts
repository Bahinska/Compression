import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { User } from 'src/app/models/user.model';

@Component({
  selector: 'app-registration',
  templateUrl: './registration.component.html',
  styleUrls: ['./registration.component.css']
})
export class RegistrationComponent {
  registrationForm: FormGroup;
  submitted = false;

  constructor(private fb: FormBuilder, private http: HttpClient) {
    this.registrationForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      sensorId: ['', Validators.required]
    });
  }

  get f() { return this.registrationForm.controls; }

  onSubmit(): void {
    this.submitted = true;
    if (this.registrationForm.valid) {
      const formData: User = {
        email: this.registrationForm.get('email')!.value,
        password: this.registrationForm.get('password')!.value,
        sensorId: this.registrationForm.get('sensorId')!.value
      };
      
      const headers = new HttpHeaders({ 'Content-Type': 'application/json' });


      this.http.post('https://localhost:7246/api/admin/register', formData, {headers}).subscribe(response => {
        console.log('Registration successful', response);
      }, error => {
        console.error('Registration error', error);
      });
    }
  }
}