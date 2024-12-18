using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Virtual_Art_Gallery.Models
{
    public class PriceListModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        public string? ImagePath { get; set; }

        [Required]
        [Range(0, 9999999.99, ErrorMessage = "Цена должна быть положительной и не превышать 9 999 999.99.")]
        public decimal Price { get; set; }

        [Required]
        public string CreatorId { get; set; }

        [ForeignKey("CreatorId")]
        public IdentityUser? Creator { get; set; }

    }
}
