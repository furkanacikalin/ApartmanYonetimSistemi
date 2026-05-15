using Microsoft.EntityFrameworkCore;
using ApartmanYonetimSistemi.Models;

namespace ApartmanYonetimSistemi.Data;

public class UserContext : DbContext
{
    public UserContext(DbContextOptions<UserContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; } 
}