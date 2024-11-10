using System.ComponentModel.DataAnnotations;

namespace Art_Gallery.Models
{
    public class ExhibitionModel
    {
        [Key]
        public string Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public bool IsClosed { get; set; }

        //public List<ArtworkModel> Artwork { get; set; }
    }
}