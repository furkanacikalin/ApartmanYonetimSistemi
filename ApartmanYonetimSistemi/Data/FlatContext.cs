using Microsoft.EntityFrameworkCore;
using ApartmanYonetimSistemi.Models;

namespace ApartmanYonetimSistemi.Data;

public class FlatContext : DbContext
{
    public FlatContext(DbContextOptions<FlatContext> options) : base(options)
    {
    }

    public DbSet<Flat> Flats { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // PostgreSQL'in tablo ve sütun isimlerindeki büyük-küçük harf katılıklarını engelliyoruz
        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            var tableName = entity.GetTableName();
            if (!string.IsNullOrEmpty(tableName))
            {
                entity.SetTableName(tableName.ToLowerInvariant());
            }

            foreach (var property in entity.GetProperties())
            {
                // Uyarısız .Name kullanımı ile sütun isimlerini küçük harfe eşitliyoruz
                property.SetColumnName(property.Name.ToLowerInvariant());
            }
        }
    }
}