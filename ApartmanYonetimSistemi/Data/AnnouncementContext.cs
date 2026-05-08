using Microsoft.EntityFrameworkCore;
using ApartmanYonetimSistemi.Models;

namespace ApartmanYonetimSistemi.Data;

public class AnnouncementContext : DbContext
{
    public AnnouncementContext(DbContextOptions<AnnouncementContext> options) : base(options) { }

    public DbSet<Announcement> Announcements { get; set; }
}