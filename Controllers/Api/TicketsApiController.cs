using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TicketApp.Data.Abstract;
using TicketApp.Data.Concrete.EfCore;
using TicketApp.Models;

namespace TicketApp.Controllers.Api
{
    [ApiController]
    [Route("api/tickets")]
    public class TicketsApiController : ControllerBase
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly TicketContext _context;

        public TicketsApiController(ITicketRepository ticketRepository, TicketContext context)
        {
            _ticketRepository = ticketRepository;
            _context = context;
        }

        // Biletleri Listele: GET api/tickets
        [HttpGet]
        public async Task<IActionResult> GetTickets()
        {
            var tickets = await _ticketRepository.Tickets
                .OrderBy(t => t.Date)
                .Select(t => new TicketDto
                {
                    Id = t.Id,
                    PlayName = t.PlayName,
                    Date = t.Date,
                    Price = t.Price
                })
                .ToListAsync();

            return Ok(tickets);
        }

        // Koltukları Getir: GET api/tickets/5/seats
        [HttpGet("{id}/seats")]
        public async Task<IActionResult> GetTicketDetails(int id)
        {
            var ticket = await _context.Tickets
                .Include(t => t.Hall)
                .ThenInclude(h => h.Seats)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (ticket == null) return NotFound("Bilet bulunamadı.");

            var dto = new TicketDetailDto
            {
                TicketId = ticket.Id,
                PlayName = ticket.PlayName,
                Date = ticket.Date,
                Price = ticket.Price,
                Hall = new HallDto
                {
                    Name = ticket.Hall.Name,
                    RowCount = ticket.Hall.RowCount,
                    SeatsPerRow = ticket.Hall.SeatsPerRow
                },
                Seats = ticket.Hall.Seats
                    .OrderBy(s => s.RowNumber)
                    .ThenBy(s => s.SeatNumber)
                    .Select(s => new SeatDto
                    {
                        Id = s.Id,
                        RowNumber = s.RowNumber,
                        SeatNumber = s.SeatNumber,
                        IsReserved = s.IsReserved
                    })
                    .ToList()
            };

            return Ok(dto);
        }
    }
}