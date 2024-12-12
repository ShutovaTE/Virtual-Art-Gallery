namespace Virtual_Art_Gallery.Models
{
    public class AdminStatisticsViewModel
    {
        public ArtworkModel MostViewedArtwork { get; set; }
        public IEnumerable<dynamic> CategoryUsage { get; set; }
        public IEnumerable<ArtworkModel> TopArtworks { get; set; }
        public List<ArtworkModel> TopLikedArtworks { get; set; }
    }

}
