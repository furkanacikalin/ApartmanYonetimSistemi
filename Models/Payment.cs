using System.ComponentModel.DataAnnotations;

namespace ApartmanYonetimSistemi.Models;

public class Payment
{
    public int Id { get; set; }

    [Required]
    public int FlatId { get; set; } 

    [Required]
    public int ApartmentId { get; set; } 

    [Required]
    [Range(0.01, 1000000, ErrorMessage = "Tutar 0'dan büyük olmalıdır.")]
    public decimal Amount { get; set; }

    [Required]
    public string Description { get; set; } = string.Empty; 

    [Required]
    public string Category { get; set; } = "Aidat"; 

    public DateTime DueDate { get; set; } 
    public DateTime CreatedDate { get; set; } = DateTime.Now; 

    public bool IsPaid { get; set; } = false; 
    public DateTime? PaidDate { get; set; } 
}