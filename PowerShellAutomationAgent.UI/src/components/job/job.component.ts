import { Component, Input } from '@angular/core';
import { NgIconComponent } from '@ng-icons/core';
import { JobResponseModel, JobStates } from '../../services/jobs/models/job-response-model';
import { LoaderComponent } from '../loader/loader.component';
import { DatePipe, NgIf } from '@angular/common';
import { JobService } from '../../services/jobs/job.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-job',
  imports: [NgIconComponent, LoaderComponent, NgIf, DatePipe],
  templateUrl: './job.component.html',
  styleUrl: './job.component.css'
})
export class JobComponent {

  JobStates = JobStates;
  @Input() job: JobResponseModel | undefined;

  get runtime(): string {
  if (!this.job || !this.job.startedOnUtc || !this.job.finishedOnUtc) return '';

  const started = new Date(this.job.startedOnUtc);
  const finished = new Date(this.job.finishedOnUtc);

  const runtimeMs = finished.getTime() - started.getTime();
  if (runtimeMs < 0) return '';

  // Convert ms to seconds (or format nicely if needed)
  const runtimeSeconds = Math.floor(runtimeMs / 1000);
  return `${runtimeSeconds} seconds`;
}

  constructor(private router: Router, private jobService: JobService) { }

  downloadArtifacts(jobId: number): void {
    this.jobService.downloadArtifacts(jobId).subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = `${jobId}-artifacts.zip`;
        a.click();
        window.URL.revokeObjectURL(url);
      },
      error: (error) => {
        console.error('Error downloading artifacts:', error);
      }
    });
  }

  openJob(jobId: number): void {
    this.router.navigate(['/jobs/view', jobId]);
  }
}
