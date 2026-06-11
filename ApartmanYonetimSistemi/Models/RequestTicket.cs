using System;
using System.ComponentModel.DataAnnotations;

namespace ApartmanYonetimSistemi.Models
{
    public class RequestTicket
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ResidentUserId { get; set; }

        [Required]
        public int ApartmentId { get; set; }

        [Required(ErrorMessage = "Lütfen bir başlık giriniz.")]
        [StringLength(100, ErrorMessage = "Başlık 100 karakterden uzun olamaz.")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Lütfen talebinizi açıklayınız.")]
        public string Description { get; set; } = string.Empty;

        [Required]
        public string Type { get; set; } = "Dilek"; // Dilek, Şikayet, Arıza vb.

        // PostgreSQL uyumluluğu için yerel zamanı UTC standardına çekiyoruz
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public bool IsResolved { get; set; } = false;

        public string? AdminNote { get; set; }

        // --- YAPAY ZEKA ENTEGRASYONU İÇİN EKLENEN ALANLAR ---

        /// <summary>
        /// Yapay zeka tarafından atanan öncelik puanı (1: Düşük, 5: Çok Acil)
        /// </summary>
        [Range(0, 5)]
        public int PriorityScore { get; set; } = 0;

        /// <summary>
        /// Yapay zekanın öncelik puanını belirleme gerekçesi
        /// </summary>
        public string? PriorityReason { get; set; }
    }
}