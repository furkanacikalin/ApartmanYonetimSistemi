using Microsoft.EntityFrameworkCore;
using ApartmanYonetimSistemi.Data;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using ApartmanYonetimSistemi.Services;
using ApartmanYonetimSistemi.Models;
using Microsoft.EntityFrameworkCore.Infrastructure; // Tablo oluþturma iþlemleri için eklendi
using Microsoft.EntityFrameworkCore.Storage; // Tablo oluþturma iþlemleri için eklendi

var builder = WebApplication.CreateBuilder(args);

// ==========================================
// CANLI VERÝTABANI BAÐLANTISI (NEON.TECH)
// ==========================================
// Baðlantý cümlesini güvenli bir þekilde appsettings.json'dan çekiyoruz
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Artýk tüm veriler tek bir Neon veritabanýnda, kendi tablolarýnda tutulacak
builder.Services.AddDbContextFactory<UserContext>(options => options.UseNpgsql(connectionString));
builder.Services.AddDbContextFactory<ApartmentContext>(options => options.UseNpgsql(connectionString));
builder.Services.AddDbContextFactory<FlatContext>(options => options.UseNpgsql(connectionString));
builder.Services.AddDbContextFactory<AnnouncementContext>(options => options.UseNpgsql(connectionString));
builder.Services.AddDbContextFactory<RequestContext>(options => options.UseNpgsql(connectionString));
builder.Services.AddDbContextFactory<PaymentContext>(options => options.UseNpgsql(connectionString));
builder.Services.AddDbContextFactory<PaymentTransactionContext>(options => options.UseNpgsql(connectionString));
builder.Services.AddDbContextFactory<SurveyContext>(options => options.UseNpgsql(connectionString));
builder.Services.AddDbContextFactory<BudgetContext>(options => options.UseNpgsql(connectionString));

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
        var budgetFactory = services.GetRequiredService<IDbContextFactory<BudgetContext>>();

        // Bütün contextleri bir diziye alýyoruz ki tek veritabanýnda tablolarý atlamadan açsýn
        var contexts = new DbContext[]
        {
            userFactory.CreateDbContext(),
            aptFactory.CreateDbContext(),
            flatFactory.CreateDbContext(),
            annFactory.CreateDbContext(),
            reqFactory.CreateDbContext(),
            payFactory.CreateDbContext(),
            transFactory.CreateDbContext(),
            surveyFactory.CreateDbContext(),
            budgetFactory.CreateDbContext()
        };

        foreach (var ctx in contexts)
        {
            try
            {
                // 1. Eðer Neon'da veritabaný hiç yoksa ana veritabanýný oluþturur
                ctx.Database.EnsureCreated();

                // 2. Veritabaný var ama tablolar eksikse, o context'e ait tablolarý zorla oluþturur
                var creator = ctx.Database.GetService<IRelationalDatabaseCreator>();
                creator.CreateTables();
            }
            catch
            {
                // Eðer tablo zaten Neon'da baþarýyla oluþturulmuþsa CreateTables() hata fýrlatýr.
                // Biz bu hatayý görmezden geliyoruz çünkü zaten istediðimiz þey tablonun var olmasý.
            }
            finally
            {
                ctx.Dispose(); // Hafýza sýzýntýsýný önlemek için iþi biteni kapatýyoruz
            }
        }

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