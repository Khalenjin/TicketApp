import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { TicketDetail, Seat } from '../../models/models';
import { TicketService } from '../../services/ticket';

@Component({
  selector: 'app-ticket-detail',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="container mt-4" *ngIf="ticket">
      <div class="row">
        <div class="col-md-8">
          <h3>{{ ticket.playName }} - Koltuk Seçimi</h3>
          <p>Salon: {{ ticket.hall.name }}</p>
          
          <div class="screen bg-secondary text-white text-center mb-4 p-2">SAHNE</div>

          <div class="seats-container d-flex flex-wrap justify-content-center gap-2" style="max-width: 600px; margin: 0 auto;">
            <button 
              *ngFor="let seat of ticket.seats"
              class="btn seat-btn"
              [ngClass]="{
                'btn-danger': seat.isReserved,
                'btn-success': seat.selected,
                'btn-outline-secondary': !seat.isReserved && !seat.selected
              }"
              [disabled]="seat.isReserved"
              (click)="toggleSeat(seat)"
              style="width: 50px; height: 50px;"
            >
              {{ seat.rowNumber }}-{{ seat.seatNumber }}
            </button>
          </div>
        </div>

        <div class="col-md-4">
          <div class="card">
            <div class="card-header">Özet</div>
            <div class="card-body">
              <p>Oyun: {{ ticket.playName }}</p>
              <p>Tarih: {{ ticket.date | date:'short' }}</p>
              <hr>
              <h6>Seçilen Koltuklar:</h6>
              <ul class="list-unstyled">
                <li *ngFor="let s of selectedSeats">
                  Sıra: {{ s.rowNumber }}, No: {{ s.seatNumber }}
                </li>
              </ul>
              <h4 class="mt-3">Toplam: {{ totalAmount }} TL</h4>
              <button 
                class="btn btn-success w-100 mt-3" 
                [disabled]="selectedSeats.length === 0"
                (click)="buy()"
              >
                Satın Al
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>
  `
})
export class TicketDetailComponent implements OnInit {
  ticket: TicketDetail | null = null;
  selectedSeats: Seat[] = [];

  constructor(
    private route: ActivatedRoute,
    private ticketService: TicketService,
    private router: Router
  ) {}

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    this.ticketService.getTicketDetails(id).subscribe(data => {
      this.ticket = data;
    });
  }

  toggleSeat(seat: Seat) {
    if (seat.isReserved) return;
    
    seat.selected = !seat.selected;
    
    if (seat.selected) {
      this.selectedSeats.push(seat);
    } else {
      this.selectedSeats = this.selectedSeats.filter(s => s.id !== seat.id);
    }
  }

  get totalAmount() {
    return (this.ticket?.price || 0) * this.selectedSeats.length;
  }

  buy() {
    if (!this.ticket || this.selectedSeats.length === 0) return;

    const buyRequest = {
      ticketId: this.ticket.ticketId,
      seatIds: this.selectedSeats.map(s => s.id),
      userId: 1 // Test amaçlı sabit ID
    };

    this.ticketService.buyTickets(buyRequest).subscribe({
      next: () => {
        alert('Satın alma başarılı!');
        this.router.navigate(['/']);
      },
      error: (err) => {
        alert('Hata oluştu: ' + err.message);
      }
    });
  }
}