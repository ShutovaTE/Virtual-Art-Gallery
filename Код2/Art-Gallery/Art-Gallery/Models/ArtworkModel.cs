using System.ComponentModel.DataAnnotations;

namespace Art_Gallery.Models
{
    public class ArtworkModel
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Введите название публикации")]
        public string Title { get; set; }

        public string? Description { get; set; }

        public bool IsPublished { get; set; }

        public int rating { get; set; }

        private DateTime DateCreated { get; set; }

        public byte ImageData { get; set; }
        private int Width;
        private int Height;
    }
}
