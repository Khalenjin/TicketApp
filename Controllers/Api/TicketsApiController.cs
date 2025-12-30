using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TicketApp.Data.Abstract;
using TicketApp.Data.Concrete.EfCore;

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

        // GET /api/tickets?startDate=2025-12-01&endDate=2025-12-31
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            var query = _ticketRepository.Tickets.AsQueryable();

            if (startDate.HasValue) query = query.Where(t => t.Date >= startDate.Value);
            if (endDate.HasValue) query = query.Where(t => t.Date <= endDate.Value);

            var tickets = await query
                .OrderBy(t => t.Date)
                .Select(t => new
                {
                    t.Id,
                    t.PlayName,
                    t.Date,
                    t.Price
                })
                .ToListAsync();

            return Ok(tickets);
        }

        // GET /api/tickets/5/seats
        [HttpGet("{ticketId:int}/seats")]
        public async Task<IActionResult> GetSeats(int ticketId)
        {
            var ticket = await _context.Tickets
                .Include(t => t.Hall)
                .ThenInclude(h => h.Seats)
                .FirstOrDefaultAsync(t => t.Id == ticketId);

            if (ticket == null) return NotFound();

            var seats = ticket.Hall!.Seats!
                .OrderBy(s => s.RowNumber)
                .ThenBy(s => s.SeatNumber)
                .Select(s => new
                {
                    s.Id,
                    s.RowNumber,
                    s.SeatNumber,
                    s.IsReserved
                })
                .ToList();

            return Ok(new
            {
                TicketId = ticket.Id,
                PlayName = ticket.PlayName,
                Date = ticket.Date,
                Price = ticket.Price,
                Hall = new
                {
                    ticket.Hall.Id,
                    ticket.Hall.Name,
                    ticket.Hall.RowCount,
                    ticket.Hall.SeatsPerRow
                },
                Seats = seats
            });
        }
    }
}
