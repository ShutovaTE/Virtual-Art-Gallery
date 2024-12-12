using Microsoft.AspNetCore.Identity;
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

        [Required(ErrorMessage = "Выберите категорию")]
        public int CategoryId { get; set; }

        public CategoryModel? Category { get; set; }

        public string? ImagePath { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        [Required]
        public DateTime DateCreated { get; set; }
        public int? ExhibitionId { get; set; }
        public virtual ExhibitionModel? Exhibition { get; set; }

        [Required]
        public string CreatorId { get; set; } 
        public IdentityUser? Creator { get; set; }

        public ArtworkStatus Status { get; set; } = ArtworkStatus.Draft;


    }
}