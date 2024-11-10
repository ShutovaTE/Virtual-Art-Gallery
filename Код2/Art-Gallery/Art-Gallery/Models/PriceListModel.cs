using System.ComponentModel.DataAnnotations;

namespace Art_Gallery.Models
{
    public class PriceListModel
    {
        [Key]
        public int Id { get; set; }

        //[Required]
        //public List<ItemModel> Items { get; set; }
    }
}
