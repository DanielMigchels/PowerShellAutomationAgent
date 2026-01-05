import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';
import { LoginRequestModel } from './models/login-request-model';
import { LoginResponseModel } from './models/login-response-model';

@Injectable({
  providedIn: 'root'
})
export class AuthenticationService {
  
  private apiUrl = '/api/authentication';

  constructor(private http: HttpClient, private router: Router) { }

  login(loginRequestModel: LoginRequestModel): Observable<LoginResponseModel> {
    return this.http.post<LoginResponseModel>(`${this.apiUrl}/login`, loginRequestModel);
  }

  setJwt(jwt: string) {
    localStorage.setItem('jwt', jwt);
  }

  getJwt(): String | null {
    return localStorage.getItem('jwt');
  }

  isLoggedIn(): boolean {
    return this.getJwt() !== null;
  }

  logout() {
    localStorage.removeItem('jwt');
    this.router.navigate(['/login']);
  }
}
