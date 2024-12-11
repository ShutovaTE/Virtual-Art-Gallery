using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Virtual_Art_Gallery.Data;
using Virtual_Art_Gallery.Models;
using Microsoft.EntityFrameworkCore;
            using System.Linq;
using Microsoft.AspNet.Identity;

namespace Virtual_Art_Gallery.Controllers
{
    public class GalleryController : Controller
    {
        private readonly GalleryContext _context;

        public GalleryController(GalleryContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var artworks = await _context.Artworks
                .Where(a => a.Status == ArtworkStatus.Approved)
                .Include(a => a.Category)  
                .Include(a => a.Creator)
                .OrderBy(a => Guid.NewGuid()) 
                .Take(4)
                .ToListAsync();

            var artistProfiles = await _context.Users
                .Where(u => _context.Artworks.Any(a => a.CreatorId == u.Id && a.Status == ArtworkStatus.Approved))
                .OrderBy(u => Guid.NewGuid())  
                .Take(4)
                .ToListAsync();

            if (!artworks.Any())
            {
                ViewBag.NoArtworksMessage = "Нет публикаций.";
            }

            if (!artistProfiles.Any())
            {
                ViewBag.NoProfilesMessage = "Нет художников.";
            }

            var model = new GalleryIndexViewModel
            {
                Artworks = artworks,
                ArtistProfiles = artistProfiles
            };

            return View(model);
        }
    }

}
