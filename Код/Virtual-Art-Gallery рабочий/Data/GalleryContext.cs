using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Virtual_Art_Gallery.Models;

namespace Virtual_Art_Gallery.Data
{
    public class GalleryContext : IdentityDbContext
    {
        public GalleryContext(DbContextOptions<GalleryContext> options) : base(options) { }
        public DbSet<ArtworkModel> Artworks { get; set; }
        public DbSet<ExhibitionModel> Exhibitions { get; set; }
        public DbSet<PriceListModel> Prices { get; set; }
        public DbSet<CategoryModel> Categories { get; set; }
        public DbSet<LikeModel> Likes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ArtworkModel>()
                .HasOne(a => a.Creator)
                .WithMany() 
                .HasForeignKey(a => a.CreatorId)
                .OnDelete(DeleteBehavior.Restrict); 
        }


    }
}
