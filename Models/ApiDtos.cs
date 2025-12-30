namespace TicketApp.Models
{
    // Bilet listesi için özet model
    public class TicketDto
    {
        public int Id { get; set; }
        public string PlayName { get; set; } = null!;
        public DateTime Date { get; set; }
        public decimal Price { get; set; }
    }

    // Detay sayfası: Bilet + Salon + Koltuklar
    public class TicketDetailDto
    {
        public int TicketId { get; set; }
        public string PlayName { get; set; } = null!;
        public DateTime Date { get; set; }
        public decimal Price { get; set; }
        public HallDto Hall { get; set; } = null!;
        public List<SeatDto> Seats { get; set; } = new();
    }

    public class HallDto
    {
        public string Name { get; set; } = null!;
        public int RowCount { get; set; }
        public int SeatsPerRow { get; set; }
    }

    public class SeatDto
    {
        public int Id { get; set; }
        public int RowNumber { get; set; }
        public int SeatNumber { get; set; }
        public bool IsReserved { get; set; }
    }

    // Satın alma işlemi için Angular'dan gelecek veri
    public class BuyTicketDto
    {
        public int TicketId { get; set; }
        public List<int> SeatIds { get; set; } = new();
        public int UserId { get; set; } // Şimdilik User ID'yi direkt alıyoruz
    }
}