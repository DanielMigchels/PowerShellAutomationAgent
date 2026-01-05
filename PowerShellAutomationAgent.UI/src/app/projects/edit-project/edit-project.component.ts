import { Component, OnInit } from '@angular/core';
import { FormGroup, FormControl, Validators, ReactiveFormsModule } from '@angular/forms';
import { ProjectService } from '../../../services/projects/project.service';
import { EditProjectRequestModel } from '../../../services/projects/models/edit-project-request-model';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-edit-project',
  imports: [ReactiveFormsModule],
  templateUrl: './edit-project.component.html',
  styleUrl: './edit-project.component.css'
})
export class EditProjectComponent implements OnInit {

  formGroup = new FormGroup({
    name: new FormControl('', [Validators.required]),
    script: new FormControl(''),
  });

  projectId: string = "";

  constructor(private route: ActivatedRoute, private projectService: ProjectService, private router: Router) { }
  
  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {
      this.projectId = params.get('id') ?? '';
      this.getProject();
    });
  }

  getProject() {
    this.projectService.getProject(this.projectId).subscribe({
      next: project => {
        this.formGroup.patchValue(project);
      },
      error: () => {
        this.router.navigate([`/projects`]);
      }
    });
  }

  updateProject() {
    if (!this.formGroup.valid) {
      this.formGroup.markAllAsTouched();
      return;
    }

    this.projectService.editProject(this.projectId, this.formGroup.value as EditProjectRequestModel).subscribe({
      next: x => {
        this.formGroup.reset();
        this.router.navigate([`/projects`]);
      },
      error: () => {

      }
    });
  }
}
