namespace Virtual_Art_Gallery.Models
{
    public class VisitorModel : UserModel
    {
        public List<PublicationModel> ViewPublicationModels(List<PublicationModel> publicationmodels)
        {
            return publicationmodels;
        }

        public int ViewRating(PublicationModel publicationmodel)
        {
            return publicationmodel.Rating;
        }
    }
}