using Microsoft.AspNetCore.Identity;

namespace Virtual_Art_Gallery.Models
{
    public class GalleryIndexViewModel
    {
        public List<ArtworkModel> Artworks { get; set; }
        public List<IdentityUser> ArtistProfiles { get; set; }
        public List<ExhibitionModel> Exhibitions { get; set; }
    }

}
