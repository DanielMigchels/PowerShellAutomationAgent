import { Injectable } from '@angular/core';
import { DashboardResponseModel } from './models/dashboard-response-model';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class DashboardService {

  private apiUrl = '/api/dashboard';

  constructor(private http: HttpClient) { }

  getDashboard(): Observable<DashboardResponseModel> {
    return this.http.get<DashboardResponseModel>(`${this.apiUrl}`);
  }
}
