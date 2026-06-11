using System;
using System.ComponentModel.DataAnnotations;

namespace ApartmanYonetimSistemi.Models;

public class PaymentTransaction
{
    [Key]
    public int Id { get; set; }

    public int PaymentId { get; set; }
    public int UserId { get; set; }

    public decimal Amount { get; set; }
    public string Currency { get; set; } = "TRY";

    public string Status { get; set; } = "Pending";

    public string? IyzicoPaymentId { get; set; }

    public string? ErrorMessage { get; set; }

    // PostgreSQL zaman dilimi hatasını engellemek için UTC'ye geçiyoruz
    public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
}