namespace Virtual_Art_Gallery.Models
{
    public class AdminModel : UserModel
    {
        public List<CategoryModel> CategoryModels { get; set; } = new List<CategoryModel>();

        public void ManageCategory(CategoryModel categorymodel, string action)
        {
            if (action == "add")
                CategoryModels.Add(categorymodel);
            else if (action == "delete")
                CategoryModels.Remove(categorymodel);
        }

        public void ConfirmPublishArtwork(ArtworkModel artworkmodel)
        {
            artworkmodel.IsPublished = true;
        }

        public void DeleteArtistProfile(ArtistModel artistmodel)
        {
        }
    }
}