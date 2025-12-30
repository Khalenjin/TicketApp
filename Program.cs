using Microsoft.EntityFrameworkCore;
using TicketApp.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using TicketApp.Data.Abstract;
using TicketApp.Data.Concrete.EfCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<TicketContext>(options =>
{
    options.UseSqlite(builder.Configuration["ConnectionStrings:Sql_connection"]);
});

builder.Services.AddScoped<IUserRepository, EfCoreUserRepository>();
builder.Services.AddScoped<ITicketRepository, EfCoreTicketRepository>();
builder.Services.AddScoped<ITicketPurchaseRepository, EfCoreTicketPurchaseRepository>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Users/Login";
    });

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// 1) API + MVC controller map
app.MapControllers();

// 2) (Geçici) MVC route kalsın (Razor sayfalar durduğu için)
// İstersen şimdilik default Login kalsın.
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Users}/{action=Login}/{id?}"
);

// 3) Angular fallback (wwwroot/index.html varsa, /shows gibi route’lar Angular'a düşer)
app.MapFallbackToFile("index.html");

SeedData.SeedDatabase(app);

app.Run();
