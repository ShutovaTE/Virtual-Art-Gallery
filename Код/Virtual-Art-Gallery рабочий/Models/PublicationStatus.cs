using System.ComponentModel.DataAnnotations;

namespace Virtual_Art_Gallery.Models
{
    public enum ArtworkStatus
    {
        [Display(Name = "Черновик")]
        Draft,

        [Display(Name = "На модерации")]
        Submitted,

        [Display(Name = "Подтверждено")]
        Approved,

        [Display(Name = "Отклонено")]
        Rejected
    }
}
