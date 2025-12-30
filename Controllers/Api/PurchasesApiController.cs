using Microsoft.AspNetCore.Mvc;
using TicketApp.Data.Concrete.EfCore;
using TicketApp.Entity;
using TicketApp.Models;

namespace TicketApp.Controllers.Api
{
    [ApiController]
    [Route("api/purchases")]
    public class PurchasesApiController : ControllerBase
    {
        private readonly TicketContext _context;

        public PurchasesApiController(TicketContext context)
        {
            _context = context;
        }

        // Satın Al: POST api/purchases
        [HttpPost]
        public async Task<IActionResult> BuyTickets([FromBody] BuyTicketDto model)
        {
            if (model.SeatIds == null || !model.SeatIds.Any())
            {
                return BadRequest("Hiç koltuk seçilmedi.");
            }

            // Seçilen koltukları kontrol et ve rezerve et
            foreach (var seatId in model.SeatIds)
            {
                var seat = await _context.Seats.FindAsync(seatId);

                // Koltuk yoksa veya zaten doluysa hata döndür veya atla
                if (seat == null) return BadRequest($"Koltuk {seatId} bulunamadı.");
                if (seat.IsReserved) return BadRequest($"Koltuk {seat.RowNumber}-{seat.SeatNumber} zaten dolu.");

                // Koltuğu rezerve et
                seat.IsReserved = true;

                // Satış kaydı oluştur
                var purchase = new TicketPurchase
                {
                    TicketId = model.TicketId,
                    SeatId = seatId,
                    UserId = model.UserId, // Angular'dan gelen User ID
                    PurchaseDate = DateTime.Now
                };

                _context.TicketPurchases.Add(purchase);
            }

            await _context.SaveChangesAsync();

            return Ok(new { message = "Satın alma başarılı!" });
        }
    }
}