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

        [Required]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        
        [Required]
        public string Type { get; set; } = "Dilek";

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public bool IsResolved { get; set; } = false; 

        public string? AdminNote { get; set; } 
    }
}