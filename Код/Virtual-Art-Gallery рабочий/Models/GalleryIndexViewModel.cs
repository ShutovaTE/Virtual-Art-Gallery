using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Virtual_Art_Gallery.Models
{
    public class GalleryIndexViewModel
    {
        public List<ArtworkModel> Artworks { get; set; }
        public List<IdentityUser> ArtistProfiles { get; set; }
        public List<ExhibitionModel> Exhibitions { get; set; }

        public int? SelectedCategoryId { get; set; }
        [ForeignKey(nameof(SelectedCategoryId))]
        public List<CategoryModel> Categories { get; set; }
    }

}
