﻿using System.ComponentModel.DataAnnotations;

namespace Virtual_Art_Gallery.Models
{
    public class ExhibitionModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public bool IsClosed { get; set; }

        public virtual ICollection<ArtworkModel> Artworks { get; set; } = new List<ArtworkModel>();
    }
}

