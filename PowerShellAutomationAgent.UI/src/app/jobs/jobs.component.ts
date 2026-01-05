import { Component, OnInit } from '@angular/core';
import { LoaderComponent } from '../../components/loader/loader.component';
import { JobResponseModel } from '../../services/jobs/models/job-response-model';
import { JobService } from '../../services/jobs/job.service';
import { PaginatedList } from '../../services/pagination/paginated-list';
import { NgIf } from '@angular/common';
import { NgIconComponent } from '@ng-icons/core';
import { JobComponent } from '../../components/job/job.component';

@Component({
  selector: 'app-jobs',
  imports: [NgIf, JobComponent],
  templateUrl: './jobs.component.html',
  styleUrl: './jobs.component.css'
})
export class JobsComponent implements OnInit {

  jobs: PaginatedList<JobResponseModel> | undefined;

  constructor(private jobService: JobService  ) { }

  ngOnInit(): void {
    this.fetchData();
  }

  pageSize = 10;
  page = 0;

  fetchData() {
    this.jobService.getJobs(this.pageSize, this.page).subscribe({
      next: x => this.jobs = x
    });
  }
  
  nextPage() {
    this.page++;
    this.fetchData();
  }

  previousPage() {
    this.page--;
    this.fetchData();
  }
}
