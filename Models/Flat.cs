namespace ApartmanYonetimSistemi.Models;

public class Flat
{
    public int Id { get; set; }
    public int ApartmentId { get; set; }
    public int FlatNumber { get; set; }
    public int Floor { get; set; } 
    public string Block { get; set; } = string.Empty; 
    public int? ResidentUserId { get; set; }
}