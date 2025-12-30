import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { LoginUser } from '../models/models';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  // Backend adresin (HTTP olanı kullanıyoruz şimdilik)
  private apiUrl = 'http://localhost:5289/api/users'; // Controller adını kontrol et (UsersController ise)

  constructor(private http: HttpClient) { }

  login(user: LoginUser): Observable<any> {
    // EN ÖNEMLİ KISIM: { withCredentials: true }
    // Bu ayar olmadan tarayıcı backend'den gelen Cookie'yi kaydetmez!
    return this.http.post(`${this.apiUrl}/login`, user, { 
      withCredentials: true 
    });
  }

  logout(): Observable<any> {
    return this.http.get(`${this.apiUrl}/logout`, { 
      withCredentials: true 
    });
  }
}