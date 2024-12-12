using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Virtual_Art_Gallery.Data;
using Virtual_Art_Gallery.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Virtual_Art_Gallery.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AdminController : Controller
    {
        private readonly GalleryContext _context;

        public AdminController(GalleryContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Statistics()
        {
            var mostViewedArtwork = await _context.Artworks
                .Where(a => a.Status == ArtworkStatus.Approved) 
                .OrderByDescending(a => a.ViewCount)
                .FirstOrDefaultAsync();

            var categoryUsage = await _context.Artworks
                .Where(a => a.Status == ArtworkStatus.Approved) 
                .GroupBy(a => a.Category.Name)
                .Select(g => new { CategoryName = g.Key, Count = g.Count() })
                .OrderByDescending(c => c.Count)
                .ToListAsync();

            var topArtworks = await _context.Artworks
                .Where(a => a.Status == ArtworkStatus.Approved) 
                .OrderByDescending(a => a.ViewCount)
                .Take(5)
                .ToListAsync();

            var topLikedArtworks = await _context.Artworks
                .Where(a => a.Status == ArtworkStatus.Approved) 
                .OrderByDescending(a => a.LikeCount)
                .Take(5)
                .ToListAsync();

            var model = new AdminStatisticsViewModel
            {
                MostViewedArtwork = mostViewedArtwork,
                CategoryUsage = categoryUsage,
                TopArtworks = topArtworks,
                TopLikedArtworks = topLikedArtworks
            };

            return View(model);
        }


    }

}