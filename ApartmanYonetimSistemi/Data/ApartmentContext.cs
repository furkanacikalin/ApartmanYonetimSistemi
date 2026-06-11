using Microsoft.EntityFrameworkCore;
using ApartmanYonetimSistemi.Models;

namespace ApartmanYonetimSistemi.Data;

public class ApartmentContext : DbContext
{
    public ApartmentContext(DbContextOptions<ApartmentContext> options) : base(options)
    {
        // PostgreSQL şemalarının otomatik oluşturulmasını garanti altına almak için
    }

    public DbSet<Apartment> Apartments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            var tableName = entity.GetTableName();
            if (!string.IsNullOrEmpty(tableName))
            {
                entity.SetTableName(tableName.ToLowerInvariant());
            }

            foreach (var property in entity.GetProperties())
            {
                // GetName() yerine doğrudan .Name özelliğini küçük harfe çeviriyoruz
                property.SetColumnName(property.Name.ToLowerInvariant());
            }
        }
    }
}