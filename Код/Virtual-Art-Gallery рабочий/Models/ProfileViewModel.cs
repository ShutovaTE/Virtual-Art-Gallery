namespace Virtual_Art_Gallery.Models
{
    public class ProfileViewModel
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Contact { get; set; }
        public string AboutMe { get; set; }
        public List<ArtworkModel> Artworks { get; set; }
    }

}
