import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { JobComponent } from '../../components/job/job.component';
import { JobResponseModel } from '../../services/jobs/models/job-response-model';
import { DashboardService } from '../../services/dashboard/dashboard.service';

@Component({
  selector: 'app-dashboard',
  imports: [JobComponent],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css'
})
export class DashboardComponent {
  
  jobs: JobResponseModel[] | undefined;
  
  runningJobs = 0;
  failedJobs = 0;
  successfulJobs = 0;

  constructor(private router: Router, private dashboardService: DashboardService) { }

  ngOnInit(): void {
    this.dashboardService.getDashboard().subscribe({
      next: response => {
        this.jobs = response.recentJobs;
        this.runningJobs = response.runningJobsCount;
        this.successfulJobs = response.successfulJobsCount;
        this.failedJobs = response.failedJobsCount;
      },
      error: err => console.error('Error fetching dashboard data', err)
    });
  }
  
  navigateJobs() {
    this.router.navigate(['/jobs']);
  }
}
