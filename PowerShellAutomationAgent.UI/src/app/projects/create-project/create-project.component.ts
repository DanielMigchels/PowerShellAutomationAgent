import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { ProjectService } from '../../../services/projects/project.service';
import { FormGroup, FormControl, Validators, ReactiveFormsModule } from '@angular/forms';
import { AddProjectRequestModel } from '../../../services/projects/models/add-project-request-model';

@Component({
  selector: 'app-create-project',
  imports: [ReactiveFormsModule],
  templateUrl: './create-project.component.html',
  styleUrl: './create-project.component.css'
})
export class CreateProjectComponent {
  
  formGroup = new FormGroup({
    name: new FormControl('', [Validators.required]),
  });

  constructor(private projectService: ProjectService, private router: Router) { }
  
  createProject() {
    
    if (!this.formGroup.valid) {
      this.formGroup.markAllAsTouched();
      return;
    }

    this.projectService.addProject(this.formGroup.value as AddProjectRequestModel).subscribe({
      next: x => {
        this.formGroup.reset();
        this.router.navigate([`/projects`]);
      },
      error: () => {

      }
    });
  }
}
