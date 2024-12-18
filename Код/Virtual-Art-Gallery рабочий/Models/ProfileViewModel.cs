namespace Virtual_Art_Gallery.Models
{
    public class ProfileViewModel
    {
        public string UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }

        public List<ArtworkModel> Artworks { get; set; }
        public List<ExhibitionModel> Exhibitions { get; set; }
        public List<PriceListModel> Prices { get; set; }
    }

}
