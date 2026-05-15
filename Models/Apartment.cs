namespace ApartmanYonetimSistemi.Models;

public class Apartment
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty; 
    public string Address { get; set; } = string.Empty; 

    
    public int AdminUserId { get; set; }
}