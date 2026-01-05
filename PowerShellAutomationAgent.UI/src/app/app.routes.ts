import { Routes } from '@angular/router';
import { ProjectsComponent } from './projects/projects.component';
import { BaseLayoutComponent } from './layout/base-layout/base-layout.component';
import { DashboardComponent } from './dashboard/dashboard.component';
import { JobsComponent } from './jobs/jobs.component';
import { ScriptComponent } from './script/script.component';
import { CreateProjectComponent } from './projects/create-project/create-project.component';
import { EditProjectComponent } from './projects/edit-project/edit-project.component';
import { AuthLayoutComponent } from './layout/auth-layout/auth-layout.component';
import { LoginComponent } from './login/login.component';
import { authenticationGuard } from './guards/authentication.guard';
import { ViewJobComponent } from './jobs/view-job/view-job.component';

export const routes: Routes = [
  {
    path: '',
    component: BaseLayoutComponent,
    canActivate: [authenticationGuard],
    children: [
      { path: '', component: DashboardComponent },
      { path: 'projects', component: ProjectsComponent },
      { path: 'projects/create', component: CreateProjectComponent },
      { path: 'projects/edit/:id', component: EditProjectComponent },
      { path: 'jobs', component: JobsComponent },
      { path: 'jobs/view/:id', component: ViewJobComponent },
      { path: 'projects/script/:id', component: ScriptComponent },
    ]
  },
  {
    path: 'login',
    component: AuthLayoutComponent,
    children: [
      { path: '', component: LoginComponent },
    ]
  },
  { path: '**', redirectTo: 'login', pathMatch: 'full' },
];
