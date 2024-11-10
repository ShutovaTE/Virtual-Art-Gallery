using System.ComponentModel.DataAnnotations;

namespace Virtual_Art_Gallery.Models
{
    public class StatisticModel
    {
        [Key]
        public int Id { get; set; }
        public DateTime VisitData { get; set; }
    }
}
