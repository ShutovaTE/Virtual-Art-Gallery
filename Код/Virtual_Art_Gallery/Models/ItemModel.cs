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
        public decimal Price { get; set; }
    }
}
