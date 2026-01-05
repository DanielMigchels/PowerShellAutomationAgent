import { Component } from '@angular/core';
import { Router, RouterOutlet } from '@angular/router';
import { AuthenticationService } from '../../../services/authentication/authentication.service';

@Component({
  selector: 'app-base-layout',
  imports: [RouterOutlet],
  templateUrl: './base-layout.component.html',
  styleUrl: './base-layout.component.css'
})
export class BaseLayoutComponent {
  
  constructor(private authenticationService: AuthenticationService, private router: Router) { }

  navigateHome() {
    this.router.navigate(['/']);
  }

  navigateProjects() {
    this.router.navigate(['/projects']);
  }

  navigateJobs() {
    this.router.navigate(['/jobs']);
  }

  logout() {
    this.authenticationService.logout();
  }
}
