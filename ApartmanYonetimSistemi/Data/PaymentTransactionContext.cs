using Microsoft.EntityFrameworkCore;
using ApartmanYonetimSistemi.Models;

namespace ApartmanYonetimSistemi.Data;

public class PaymentTransactionContext : DbContext
{
    public PaymentTransactionContext(DbContextOptions<PaymentTransactionContext> options) : base(options)
    {
    }

    public DbSet<PaymentTransaction> PaymentTransactions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // PostgreSQL için tablo ve sütun isimlerini küçük harfe zorlayarak Case-Sensitive hatalarını engelliyoruz
        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            var tableName = entity.GetTableName();
            if (!string.IsNullOrEmpty(tableName))
            {
                entity.SetTableName(tableName.ToLowerInvariant());
            }

            foreach (var property in entity.GetProperties())
            {
                // .Name kullanarak uyarıları engelliyor ve sütun isimlerini eşitliyoruz
                property.SetColumnName(property.Name.ToLowerInvariant());
            }
        }
    }
}