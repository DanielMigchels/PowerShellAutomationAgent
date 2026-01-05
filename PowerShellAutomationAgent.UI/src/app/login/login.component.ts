import { Component } from '@angular/core';
import { FormGroup, FormControl, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthenticationService } from '../../services/authentication/authentication.service';
import { LoginRequestModel } from '../../services/authentication/models/login-request-model';
import { NgIf } from '@angular/common';

@Component({
  selector: 'app-login',
  imports: [ReactiveFormsModule, NgIf],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {

  rejected = false;

  formGroup = new FormGroup({
    password: new FormControl('', [Validators.required]),
  });
  
  constructor(private authenticationService: AuthenticationService, private router: Router) { }

  login() {
    this.rejected = false;

    if (!this.formGroup.valid) {
      this.formGroup.markAllAsTouched();
      return;
    }

    this.authenticationService.login(this.formGroup.value as LoginRequestModel).subscribe({
      next: response => {
        if (response.success) {
          this.authenticationService.setJwt(response.jwt);
          this.router.navigate(['/']);
        }
        else {
          this.rejected = true;
        }
      },
      error: error => {
        this.rejected = true;
      }
    }
    );
  }
}
