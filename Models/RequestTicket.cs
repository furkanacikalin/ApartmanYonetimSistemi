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
        public string Type { get; set; } = "Dilek"; 

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public bool IsResolved { get; set; } = false;

        public string? AdminNote { get; set; }

        

       
        [Range(0, 5)]
        public int PriorityScore { get; set; } = 0;

        
        public string? PriorityReason { get; set; }
    }
}