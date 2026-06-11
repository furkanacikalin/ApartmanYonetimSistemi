using ApartmanYonetimSistemi.Models;
using Microsoft.EntityFrameworkCore;

namespace ApartmanYonetimSistemi.Data;

public class SurveyContext : DbContext
{
    public SurveyContext(DbContextOptions<SurveyContext> options) : base(options)
    {
    }

    public DbSet<Survey> Surveys { get; set; }
    public DbSet<SurveyOption> SurveyOptions { get; set; }
    public DbSet<SurveyVote> SurveyVotes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // PostgreSQL küçük harf uyumluluğu için tablo ve şema isimleri senkronizasyonu
        modelBuilder.Entity<Survey>().ToTable("surveys");
        modelBuilder.Entity<SurveyOption>().ToTable("surveyoptions");
        modelBuilder.Entity<SurveyVote>().ToTable("surveyvotes");

        // İlişkiler ve Silme Kuralları (Cascade Delete)
        modelBuilder.Entity<SurveyOption>()
            .HasOne<Survey>()
            .WithMany(s => s.Options)
            .HasForeignKey(o => o.SurveyId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<SurveyVote>()
            .HasOne<SurveyOption>()
            .WithMany(o => o.Votes)
            .HasForeignKey(v => v.SurveyOptionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}