using System.ComponentModel.DataAnnotations;

namespace Virtual_Art_Gallery.Models
{
    public class ArtworkModel
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Введите название публикации")]
        public string Title { get; set; }

        public string? Description { get; set; }

        [Required]
        public int CategoryId { get; set; }
        public CategoryModel Category { get; set; }

        //public bool IsPublished { get; set; }

        //public int Rating { get; set; }

        //public DateTime DateCreated { get; set; } = DateTime.Now;

        public string ImagePath { get; set; }

        public int Width { get; set; }
        public int Height { get; set; }
    }
}
