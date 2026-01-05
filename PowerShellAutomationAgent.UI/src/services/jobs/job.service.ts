import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { PaginatedList } from '../pagination/paginated-list';
import { JobResponseModel } from './models/job-response-model';

@Injectable({
  providedIn: 'root'
})
export class JobService {

  private apiUrl = '/api/job';

  constructor(private http: HttpClient) { }

  getJobs(pageSize = 2147483647, page = 0): Observable<PaginatedList<JobResponseModel>> {
    return this.http.get<PaginatedList<JobResponseModel>>(`${this.apiUrl}?pageSize=${pageSize}&page=${page}`);
  }

  getJob(jobId: string) {
    return this.http.get<JobResponseModel>(`${this.apiUrl}/${jobId}`);
  }

  startJob(projectId: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/${projectId}/start`, null);
  }

  downloadArtifacts(jobId: number) {
    return this.http.get(`${this.apiUrl}/${jobId}/artifacts`, { responseType: 'blob' });
  }
}
