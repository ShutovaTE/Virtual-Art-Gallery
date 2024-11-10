using Microsoft.EntityFrameworkCore;
using Virtual_Art_Gallery.Models;

namespace Virtual_Art_Gallery.Data
{
    public class GalleryContext : DbContext
    {
        public GalleryContext(DbContextOptions<GalleryContext> options) : base(options) { }
        public DbSet<ArtworkModel> Artworks { get; set; }
        public DbSet<ExhibitionModel> Exhibitions { get; set; }
        public DbSet<PriceListModel> Prices { get; set; }
        public DbSet<ItemModel> Items { get; set; }
        public DbSet<CategoryModel> Categories { get; set; }

    }
}
