import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { NgIconComponent } from '@ng-icons/core';
import { PaginatedList } from '../../services/pagination/paginated-list';
import { ProjectResponseModel } from '../../services/projects/models/project-response-model';
import { ProjectService } from '../../services/projects/project.service';
import { DatePipe, NgIf } from '@angular/common';
import { JobService } from '../../services/jobs/job.service';

@Component({
  selector: 'app-projects',
  imports: [NgIconComponent, NgIf, DatePipe],
  templateUrl: './projects.component.html',
  styleUrl: './projects.component.css'
})
export class ProjectsComponent implements OnInit {
  
  projects: PaginatedList<ProjectResponseModel> | undefined;

  constructor(private projectService: ProjectService, private jobService: JobService, private router: Router) { }
  
  ngOnInit(): void {
    this.fetchData();
  }

  pageSize = 10;
  page = 0;

  fetchData() {
    this.projects = undefined;
    this.projectService.getProjects(this.pageSize, this.page).subscribe({
      next: x => this.projects = x
    });
  }

  createProject() {
    this.router.navigate(['/projects/create']);
  }

  openScript(id: string) {
    this.router.navigate(['/projects/script', id]);
  }

  editProject(id: string) {
    this.router.navigate(['/projects/edit', id]);
  }

  startScript(id: string) {
    this.jobService.startJob(id).subscribe({
      next: () => this.router.navigate(['jobs']),
      error: err => {}
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
