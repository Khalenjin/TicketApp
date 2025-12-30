import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Ticket, TicketDetail, BuyTicket } from '../models/models';

@Injectable({
  providedIn: 'root'
})
export class TicketService {
  // launchSettings.json'daki https portunu kullanıyoruz
  private apiUrl = 'http://localhost:5289/api';

  constructor(private http: HttpClient) { }

  getTickets(): Observable<Ticket[]> {
    return this.http.get<Ticket[]>(`${this.apiUrl}/tickets`);
  }

  getTicketDetails(id: number): Observable<TicketDetail> {
    return this.http.get<TicketDetail>(`${this.apiUrl}/tickets/${id}/seats`);
  }

  buyTickets(data: BuyTicket): Observable<any> {
    return this.http.post(`${this.apiUrl}/purchases`, data);
  }
}