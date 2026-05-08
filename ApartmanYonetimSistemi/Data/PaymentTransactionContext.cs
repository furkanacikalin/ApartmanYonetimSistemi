using Microsoft.EntityFrameworkCore;
using ApartmanYonetimSistemi.Models;

namespace ApartmanYonetimSistemi.Data;

public class PaymentTransactionContext : DbContext
{
    public PaymentTransactionContext(DbContextOptions<PaymentTransactionContext> options) : base(options)
    {
    }

    public DbSet<PaymentTransaction> PaymentTransactions { get; set; }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
       
        configurationBuilder.Properties<decimal>().HaveConversion<double>();
    }
}