using System.ComponentModel.DataAnnotations;

namespace Virtual_Art_Gallery.Models
{
    public class ItemModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        public string? ImagePath { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        [Required]
        public decimal Price { get; set; }

        public int PriceListId { get; set; }

        
    }
}
