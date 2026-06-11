using Microsoft.EntityFrameworkCore;
using ApartmanYonetimSistemi.Data;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using ApartmanYonetimSistemi.Services;
using ApartmanYonetimSistemi.Models;

var builder = WebApplication.CreateBuilder(args);

// ==========================================
// POSTGRESQL BAÐLANTI AYARLARI
// Kurulumda belirlediðin þifreyi buraya yaz:
// ==========================================
string dbPassword = "1111";

builder.Services.AddDbContextFactory<UserContext>(options =>
    options.UseNpgsql($"Host=localhost;Database=Apartman_Users;Username=postgres;Password={dbPassword}"));

builder.Services.AddDbContextFactory<ApartmentContext>(options =>
    options.UseNpgsql($"Host=localhost;Database=Apartman_Apartments;Username=postgres;Password={dbPassword}"));

builder.Services.AddDbContextFactory<FlatContext>(options =>
    options.UseNpgsql($"Host=localhost;Database=Apartman_Flats;Username=postgres;Password={dbPassword}"));

builder.Services.AddDbContextFactory<AnnouncementContext>(options =>
    options.UseNpgsql($"Host=localhost;Database=Apartman_Announcements;Username=postgres;Password={dbPassword}"));

builder.Services.AddDbContextFactory<RequestContext>(options =>
    options.UseNpgsql($"Host=localhost;Database=Apartman_Requests;Username=postgres;Password={dbPassword}"));

builder.Services.AddDbContextFactory<PaymentContext>(options =>
    options.UseNpgsql($"Host=localhost;Database=Apartman_Payments;Username=postgres;Password={dbPassword}"));

builder.Services.AddDbContextFactory<PaymentTransactionContext>(options =>
    options.UseNpgsql($"Host=localhost;Database=Apartman_Transactions;Username=postgres;Password={dbPassword}"));

builder.Services.AddDbContextFactory<SurveyContext>(options =>
    options.UseNpgsql($"Host=localhost;Database=Apartman_Surveys;Username=postgres;Password={dbPassword}"));

// YENÝ: Bütçe ve Harcama Yönetimi için DbContext eklendi
builder.Services.AddDbContextFactory<BudgetContext>(options =>
    options.UseNpgsql($"Host=localhost;Database=Apartman_Budgets;Username=postgres;Password={dbPassword}"));

// Blazored LocalStorage Kaydý
builder.Services.AddBlazoredLocalStorage();

// HTTP Client Entegrasyonu (GeminiService / Groq API'nin hatasýz çalýþmasý için ÞART)
builder.Services.AddHttpClient();

// Kimlik Doðrulama ve Yetkilendirme Ayarlarý
builder.Services.AddAuthentication("Cookies")
    .AddCookie("Cookies", options =>
    {
        options.Cookie.Name = "ApartmanApp_Auth";
        options.LoginPath = "/login";
        options.AccessDeniedPath = "/access-denied";
    });

builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();

// Blazor Server Bileþenleri Kaydý
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Özel Servislerimizin Ömür Döngüsü Yönetimi
builder.Services.AddSingleton<SecurityService>(); // Stateless olduðu için Singleton yaptýk
builder.Services.AddScoped<PaymentService>();
builder.Services.AddScoped<GeminiService>(); // Ýçerideki IHttpClientFactory artýk sorunsuz çalýþacak

var app = builder.Build();

// ==========================================
// VERÝTABANI ÝLK KURULUM VE SEED DATA AKIÞI
// ==========================================
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var userFactory = services.GetRequiredService<IDbContextFactory<UserContext>>();
        var aptFactory = services.GetRequiredService<IDbContextFactory<ApartmentContext>>();
        var flatFactory = services.GetRequiredService<IDbContextFactory<FlatContext>>();
        var annFactory = services.GetRequiredService<IDbContextFactory<AnnouncementContext>>();
        var reqFactory = services.GetRequiredService<IDbContextFactory<RequestContext>>();
        var payFactory = services.GetRequiredService<IDbContextFactory<PaymentContext>>();
        var transFactory = services.GetRequiredService<IDbContextFactory<PaymentTransactionContext>>();
        var surveyFactory = services.GetRequiredService<IDbContextFactory<SurveyContext>>();
        var budgetFactory = services.GetRequiredService<IDbContextFactory<BudgetContext>>(); // YENÝ EKLENDÝ

        // PostgreSQL veritabanlarý ve tablolarý yoksa otomatik oluþturuluyor
        using (var uCtx = userFactory.CreateDbContext()) uCtx.Database.EnsureCreated();
        using (var aCtx = aptFactory.CreateDbContext()) aCtx.Database.EnsureCreated();
        using (var fCtx = flatFactory.CreateDbContext()) fCtx.Database.EnsureCreated();
        using (var nCtx = annFactory.CreateDbContext()) nCtx.Database.EnsureCreated();
        using (var rCtx = reqFactory.CreateDbContext()) rCtx.Database.EnsureCreated();
        using (var pCtx = payFactory.CreateDbContext()) pCtx.Database.EnsureCreated();
        using (var tCtx = transFactory.CreateDbContext()) tCtx.Database.EnsureCreated();
        using (var sCtx = surveyFactory.CreateDbContext()) sCtx.Database.EnsureCreated();
        using (var bCtx = budgetFactory.CreateDbContext()) bCtx.Database.EnsureCreated(); // YENÝ EKLENDÝ

        using var userCtx = userFactory.CreateDbContext();
        if (!userCtx.Users.Any())
        {
            var security = services.GetRequiredService<SecurityService>();
            var (hash, salt) = security.HashPassword("123456");

            userCtx.Users.Add(new User
            {
                Username = "admin",
                FirstName = "Sistem",
                LastName = "Yöneticisi",
                PasswordHash = hash,
                Salt = salt,
                Role = "Admin",
                MustChangePassword = true
            });
            userCtx.SaveChanges();
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Veritabaný oluþturulurken veya ilk veri (Seed) yazýlýrken bir hata oluþtu.");
    }
}

// HTTP Pipeline Yapýlandýrmasý
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapRazorComponents<ApartmanYonetimSistemi.Components.App>()
    .AddInteractiveServerRenderMode();

app.Run();