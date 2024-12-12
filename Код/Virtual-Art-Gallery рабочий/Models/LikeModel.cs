
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

    namespace Virtual_Art_Gallery.Models
    {
        public class LikeModel
        {
            [Key]
            public int Id { get; set; }

            [Required]
            public int ArtworkId { get; set; }
            public virtual ArtworkModel Artwork { get; set; }

            [Required]
            public string UserId { get; set; }
            public virtual IdentityUser User { get; set; }
    }
    }


