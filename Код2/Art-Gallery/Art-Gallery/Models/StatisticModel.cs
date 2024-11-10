using System.ComponentModel.DataAnnotations;

namespace Art_Gallery.Models
{
    public class StatisticModel
    {
        [Key]
        public int Id { get; set; }
        public DateTime VisitData { get; set; }
    }
}
