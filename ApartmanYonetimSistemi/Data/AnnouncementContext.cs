using Microsoft.EntityFrameworkCore;
using ApartmanYonetimSistemi.Models;

namespace ApartmanYonetimSistemi.Data;

public class AnnouncementContext : DbContext
{
    public AnnouncementContext(DbContextOptions<AnnouncementContext> options) : base(options) { }

    public DbSet<Announcement> Announcements { get; set; }

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
                // .GetName() yerine doğrudan .Name kullanarak CS0618 uyarısını gideriyoruz
                property.SetColumnName(property.Name.ToLowerInvariant());
            }
        }
    }
}