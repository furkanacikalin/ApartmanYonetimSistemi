using Microsoft.EntityFrameworkCore;
using ApartmanYonetimSistemi.Models;

namespace ApartmanYonetimSistemi.Data;

public class PaymentContext : DbContext
{
    public PaymentContext(DbContextOptions<PaymentContext> options) : base(options) { }

    public DbSet<Payment> Payments { get; set; }

    
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        
        configurationBuilder.Properties<decimal>().HaveConversion<double>();
    }
}