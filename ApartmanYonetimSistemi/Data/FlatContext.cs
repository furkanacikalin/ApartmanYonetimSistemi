using Microsoft.EntityFrameworkCore;
using ApartmanYonetimSistemi.Models;

namespace ApartmanYonetimSistemi.Data;

public class FlatContext : DbContext
{
    public FlatContext(DbContextOptions<FlatContext> options) : base(options)
    {
    }

    public DbSet<Flat> Flats { get; set; } 
}