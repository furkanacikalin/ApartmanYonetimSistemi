using System;

namespace ApartmanYonetimSistemi.Models;

public class Announcement
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;

    // PostgreSQL uyumluluğu için varsayılan zamanı UTC'ye çekiyoruz
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedDate { get; set; }
    public int ApartmentId { get; set; }
    public string? AttachmentUrl { get; set; }
}