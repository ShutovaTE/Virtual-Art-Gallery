namespace Virtual_Art_Gallery.Models
{
    public class ArtistModel : UserModel
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        //public List<ArtworkModel> Artworkmodels { get; set; }

        //        public ArtworkModel CreateArtwork(string title, string image, string description)
        //        {
        //            var newArtwork = new ArtworkModel
        //            {
        //                Title = title,
        //                Image = image,
        //                Description = description,
        //                DateCreated = DateTime.Now,
        //                Published = false
        //            };
        //            Artworkmodels.Add(newArtwork);
        //            return newArtwork;
        //        }

        //        public void EditArtwork(ArtworkModel artworkmodel, string title, string description, string image)
        //        {
        //            artworkmodel.Title = title;
        //            artworkmodel.Description = description;
        //            artworkmodel.ImageData = image;
        //        }

        //        public void DeleteArtwork(ArtworkModel artworkmodel)
        //        {
        //            Artworkmodels.Remove(artworkmodel);
        //        }
        //    }
    }
}
