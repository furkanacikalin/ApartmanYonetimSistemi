using Microsoft.EntityFrameworkCore;
using ApartmanYonetimSistemi.Models;

namespace ApartmanYonetimSistemi.Data
{
    public class RequestContext : DbContext
    {
        public RequestContext(DbContextOptions<RequestContext> options) : base(options)
        {
        }

        public DbSet<RequestTicket> RequestTickets { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // PostgreSQL büyük-küçük harf çakışmalarını önlemek için şemayı küçük harfe zorluyoruz
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                var tableName = entity.GetTableName();
                if (!string.IsNullOrEmpty(tableName))
                {
                    entity.SetTableName(tableName.ToLowerInvariant());
                }

                foreach (var property in entity.GetProperties())
                {
                    // .Name kullanarak CS0618 uyarısını engelliyoruz
                    property.SetColumnName(property.Name.ToLowerInvariant());
                }
            }
        }
    }
}