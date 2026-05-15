using Microsoft.EntityFrameworkCore;
using ApartmanYonetimSistemi.Data;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using ApartmanYonetimSistemi.Services;
using ApartmanYonetimSistemi.Models;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContextFactory<UserContext>(options =>
    options.UseSqlite("Data Source=users.db"));

builder.Services.AddDbContextFactory<ApartmentContext>(options =>
    options.UseSqlite("Data Source=apartments.db"));

builder.Services.AddDbContextFactory<FlatContext>(options =>
    options.UseSqlite("Data Source=flats.db"));

builder.Services.AddDbContextFactory<AnnouncementContext>(options =>
    options.UseSqlite("Data Source=announcements.db"));

builder.Services.AddDbContextFactory<RequestContext>(options =>
    options.UseSqlite("Data Source=requests.db"));

builder.Services.AddDbContextFactory<PaymentContext>(options =>
    options.UseSqlite("Data Source=payments.db"));

builder.Services.AddDbContextFactory<PaymentTransactionContext>(options =>
    options.UseSqlite("Data Source=transactions.db"));


builder.Services.AddBlazoredLocalStorage();

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

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddScoped<SecurityService>();
builder.Services.AddScoped<PaymentService>();
builder.Services.AddScoped<GeminiService>();

var app = builder.Build();


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

        
        using (var uCtx = userFactory.CreateDbContext()) uCtx.Database.EnsureCreated();
        using (var aCtx = aptFactory.CreateDbContext()) aCtx.Database.EnsureCreated();
        using (var fCtx = flatFactory.CreateDbContext()) fCtx.Database.EnsureCreated();
        using (var nCtx = annFactory.CreateDbContext()) nCtx.Database.EnsureCreated();
        using (var rCtx = reqFactory.CreateDbContext()) rCtx.Database.EnsureCreated();
        using (var pCtx = payFactory.CreateDbContext()) pCtx.Database.EnsureCreated();
        using (var tCtx = transFactory.CreateDbContext()) tCtx.Database.EnsureCreated();

        
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
        logger.LogError(ex, "Veritabaný oluþturulurken bir hata oluþtu.");
    }
}


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