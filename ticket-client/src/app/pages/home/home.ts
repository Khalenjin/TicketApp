import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { TicketService } from '../../services/ticket';
import { Ticket } from '../../models/models';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <div class="container mt-4">
      <h2 class="mb-4">Vizyondaki Oyunlar</h2>
      <div class="row">
        <div class="col-md-4 mb-3" *ngFor="let ticket of tickets">
          <div class="card shadow-sm">
            <div class="card-body">
              <h5 class="card-title">{{ ticket.playName }}</h5>
              <p class="card-text text-muted">{{ ticket.date | date:'dd.MM.yyyy HH:mm' }}</p>
              <div class="d-flex justify-content-between align-items-center">
                <span class="fw-bold">{{ ticket.price }} TL</span>
                <a [routerLink]="['/ticket', ticket.id]" class="btn btn-primary btn-sm">Bilet Al</a>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  `
})
export class HomeComponent implements OnInit {
  tickets: Ticket[] = [];

  constructor(private ticketService: TicketService) {}

  ngOnInit(): void {
    this.ticketService.getTickets().subscribe(data => {
      this.tickets = data;
    });
  }
}