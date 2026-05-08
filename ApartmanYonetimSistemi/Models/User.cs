namespace ApartmanYonetimSistemi.Models;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty; 
    public string LastName { get; set; } = string.Empty;  
    public string PasswordHash { get; set; } = string.Empty;
    public string Salt { get; set; } = string.Empty;
    public string Role { get; set; } = "Resident";
    public bool MustChangePassword { get; set; } = true;
}