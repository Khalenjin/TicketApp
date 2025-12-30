using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using TicketApp.Data.Abstract;
using TicketApp.Models;
using Microsoft.EntityFrameworkCore;
using TicketApp.Entity;

namespace TicketApp.Controllers
{
    // API Route tanımlaması
    [Route("api/[controller]")]
    [ApiController] 
    public class UsersController : ControllerBase // Controller yerine ControllerBase daha hafiftir
    {
        private readonly IUserRepository _userRepository;
        private readonly ITicketRepository _ticketRepository;
        private readonly ITicketPurchaseRepository _purchaseRepository;

        public UsersController(IUserRepository userRepository, ITicketRepository ticketRepository, ITicketPurchaseRepository purchaseRepository)
        {
            _userRepository = userRepository;
            _ticketRepository = ticketRepository;
            _purchaseRepository = purchaseRepository;
        }

        // GET: api/users/check-auth
        // Kullanıcının giriş yapıp yapmadığını kontrol eden endpoint
        [HttpGet("check-auth")]
        public IActionResult CheckAuth()
        {
            if (User.Identity!.IsAuthenticated)
            {
                return Ok(new { isAuthenticated = true, username = User.Identity.Name });
            }
            return Ok(new { isAuthenticated = false });
        }

        // POST: api/users/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            // [FromBody] ile JSON veriyi karşılıyoruz
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userRepository.Users
                .FirstOrDefaultAsync(x => x.Email == model.Email || x.UserName == model.Email);

            if (user == null || user.Password != model.Password)
            {
                return Unauthorized(new { message = "E-posta veya şifre hatalı." });
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            // Cookie'yi tarayıcıya set et
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal);

            // Angular'a başarılı döndük
            return Ok(new { message = "Giriş başarılı", userId = user.Id });
        }

        // POST: api/users/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var exist = await _userRepository.GetUserByEmailAsync(model.Email!);

            if (exist != null)
            {
                return BadRequest(new { message = "Bu e-posta zaten kullanımda." });
            }

            var user = new User
            {
                UserName = model.UserName!,
                Email = model.Email!,
                Password = model.Password!
            };

            await _userRepository.CreateUser(user);

            return Ok(new { message = "Kayıt başarılı" });
        }

        // GET: api/users/logout
        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok(new { message = "Çıkış yapıldı" });
        }

        // GET: api/users/profile
        [HttpGet("profile")]
        public async Task<IActionResult> Profile()
        {
            if (!User.Identity!.IsAuthenticated)
                return Unauthorized(new { message = "Giriş yapmalısınız." });

            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var user = await _userRepository.Users
                .FirstOrDefaultAsync(x => x.Id == userId);

            if (user == null)
                return NotFound(new { message = "Kullanıcı bulunamadı." });

            var purchases = await _purchaseRepository.TicketPurchases
                .Where(x => x.UserId == userId)
                .Include(x => x.Ticket)
                .Include(x => x.Seat)
                .ToListAsync();

            // ViewModel yerine direkt anonim obje veya DTO dönüyoruz
            var response = new 
            {
                User = new { user.UserName, user.Email }, // Hassas verileri (şifre gibi) gizle
                Purchases = purchases
            };

            return Ok(response);
        }
    }
}