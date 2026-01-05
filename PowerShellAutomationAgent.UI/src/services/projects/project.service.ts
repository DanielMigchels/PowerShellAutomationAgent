import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { PaginatedList } from '../pagination/paginated-list';
import { ProjectResponseModel } from './models/project-response-model';
import { AddProjectRequestModel } from './models/add-project-request-model';
import { AddProjectResponseModel } from './models/add-project-response-model';
import { EditProjectRequestModel } from './models/edit-project-request-model';

@Injectable({
  providedIn: 'root'
})
export class ProjectService {

  private apiUrl = '/api/project';

  constructor(private http: HttpClient) { }

  getProjects(pageSize = 2147483647, page = 0): Observable<PaginatedList<ProjectResponseModel>> {
    return this.http.get<PaginatedList<ProjectResponseModel>>(`${this.apiUrl}?pageSize=${pageSize}&page=${page}`);
  }

  getProject(projectId: string) {
    return this.http.get<ProjectResponseModel>(`${this.apiUrl}/${projectId}`);
  }

  addProject(addProjectRequestModel: AddProjectRequestModel): Observable<AddProjectResponseModel> {
    return this.http.post<AddProjectResponseModel>(`${this.apiUrl}`, addProjectRequestModel);
  }

  editProject(projectId: string, editProjectRequestModel: EditProjectRequestModel) {
    return this.http.put(`${this.apiUrl}/${projectId}`, editProjectRequestModel);
  }

  deleteProject(projectId: string) {
    return this.http.delete(`${this.apiUrl}/${projectId}`);
  }
}
