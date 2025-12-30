export interface Ticket {
  id: number;
  playName: string;
  date: string;
  price: number;
}

export interface Seat {
  id: number;
  rowNumber: number;
  seatNumber: number;
  isReserved: boolean;
  selected?: boolean; // Frontend'de seçim yapmak için ekledik
}

export interface Hall {
  name: string;
  rowCount: number;
  seatsPerRow: number;
}

export interface TicketDetail {
  ticketId: number;
  playName: string;
  date: string;
  price: number;
  hall: Hall;
  seats: Seat[];
}

export interface BuyTicket {
  ticketId: number;
  seatIds: number[];
  userId: number;
}

export interface LoginUser {
    email: string;
    password: string;
}