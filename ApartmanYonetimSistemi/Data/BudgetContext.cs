using ApartmanYonetimSistemi.Models;
using Microsoft.EntityFrameworkCore;

namespace ApartmanYonetimSistemi.Data;

public class BudgetContext : DbContext
{
    public BudgetContext(DbContextOptions<BudgetContext> options) : base(options)
    {
    }

    public DbSet<BudgetTransaction> BudgetTransactions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // PostgreSQL uyumluluğu için
        modelBuilder.Entity<BudgetTransaction>().ToTable("budgettransactions");
    }
}