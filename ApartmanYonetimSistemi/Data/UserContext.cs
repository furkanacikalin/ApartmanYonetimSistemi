using Microsoft.EntityFrameworkCore;
using ApartmanYonetimSistemi.Models;

namespace ApartmanYonetimSistemi.Data;

public class UserContext : DbContext
{
    public UserContext(DbContextOptions<UserContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // PostgreSQL büyük-küçük harf (Case-Sensitive) uyumsuzluklarını tamamen engelliyoruz
        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            var tableName = entity.GetTableName();
            if (!string.IsNullOrEmpty(tableName))
            {
                entity.SetTableName(tableName.ToLowerInvariant());
            }

            foreach (var property in entity.GetProperties())
            {
                // .Name kullanarak eski metot uyarılarını önlüyoruz
                property.SetColumnName(property.Name.ToLowerInvariant());
            }
        }
    }
}