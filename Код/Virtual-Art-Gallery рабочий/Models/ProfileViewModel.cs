namespace Virtual_Art_Gallery.Models
{
    public class ProfileViewModel
    {
        public string Username { get; set; }
        public string Email { get; set; }

        public List<ArtworkModel> Artworks { get; set; }
    }

}
