namespace ApartmanYonetimSistemi.Models;

public class BudgetTransaction
{
    public int Id { get; set; }
    public int ApartmentId { get; set; }
    public string TransactionType { get; set; } = string.Empty; // "Income" (Gelir) veya "Expense" (Gider)
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime Date { get; set; } = DateTime.UtcNow;
    public string? ReceiptUrl { get; set; } // Belge/Fatura Görseli
}