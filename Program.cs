using Microsoft.EntityFrameworkCore;
using TicketApp.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using TicketApp.Data.Abstract;
using TicketApp.Data.Concrete.EfCore;

var builder = WebApplication.CreateBuilder(args);

// 1. CORS Ayarı (En Kritik Kısım)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp",
        b => b.SetIsOriginAllowed(origin => true) // Angular (localhost:4200) erişebilsin
              .AllowAnyMethod()                   // GET, POST, PUT, DELETE hepsi serbest
              .AllowAnyHeader()
              .AllowCredentials());               // Cookie/Auth bilgilerine izin ver
});

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<TicketContext>(options =>
{
    options.UseSqlite(builder.Configuration["ConnectionStrings:Sql_connection"]);
});

builder.Services.AddScoped<IUserRepository, EfCoreUserRepository>();
builder.Services.AddScoped<ITicketRepository, EfCoreTicketRepository>();
builder.Services.AddScoped<ITicketPurchaseRepository, EfCoreTicketPurchaseRepository>();

// 2. Auth Ayarları
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        // ÖNEMLİ: API kullanıyoruz, kullanıcı giriş yapmazsa Login sayfasına yönlendirme (Redirect) yapma.
        // Bunun yerine 401 hatası dön, Angular anlasın.
        options.Events.OnRedirectToLogin = context =>
        {
            context.Response.StatusCode = 401;
            return Task.CompletedTask;
        };
        // Eski MVC ayarı (Bunu iptal ediyoruz çünkü login sayfası Angular'da)
        // options.LoginPath = "/Users/Login"; 
    });

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseStaticFiles();

// 3. Middleware Sıralaması (Çok Önemli)
app.UseRouting();

app.UseCors("AllowAngularApp"); // Routing'den SONRA, Auth'tan ÖNCE olmalı.

app.UseAuthentication();
app.UseAuthorization();

// 4. Controller Map'leme
app.MapControllers(); // API Controller'ları için

// 5. MVC Varsayılan Rotası (Home'a çekiyoruz)
// UsersController artık API olduğu için burayı Home yapıyoruz.
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

// 6. Angular Fallback
// Eğer wwwroot klasöründe Angular build dosyaların yoksa bu satır bir şey yapmaz, 
// ama varsa Angular route'larını destekler.
// app.MapFallbackToFile("index.html"); 

SeedData.SeedDatabase(app);

app.Run();