using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Virtual_Art_Gallery.Data;
using Virtual_Art_Gallery.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Virtual_Art_Gallery.Controllers
{
    public class GalleryController : Controller
    {
        private readonly GalleryContext _context;

        public GalleryController(GalleryContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? searchQuery, int? categoryId)
        {
            ViewData["CategoryList"] = new SelectList(_context.Categories, "Id", "Name", categoryId);

            var categories = await _context.Categories.ToListAsync();
            ViewBag.SearchQuery = searchQuery;
            ViewBag.CategoryId = categoryId;

            IQueryable<ArtworkModel> artworkQuery = _context.Artworks
                .Where(a => a.Status == ArtworkStatus.Approved)
                .Include(a => a.Category)
                .Include(a => a.Creator);

            IQueryable<IdentityUser> artistQuery = _context.Users
                .Where(u => _context.Artworks.Any(a => a.CreatorId == u.Id && a.Status == ArtworkStatus.Approved));

            IQueryable<ExhibitionModel> exhibitionQuery = _context.Exhibitions
                .Include(e => e.Creator)
                .Include(e => e.Artworks)
                    .ThenInclude(a => a.Creator);

            if (categoryId.HasValue)
            {
                artworkQuery = artworkQuery.Where(a => a.CategoryId == categoryId.Value);

                if (!string.IsNullOrEmpty(searchQuery))
                {
                    string lowerSearchQuery = searchQuery.ToLower();
                    artworkQuery = artworkQuery.Where(a => a.Title.ToLower().Contains(lowerSearchQuery));
                }

                artistQuery = Enumerable.Empty<IdentityUser>().AsQueryable();
                exhibitionQuery = Enumerable.Empty<ExhibitionModel>().AsQueryable();
            }
            else if (!string.IsNullOrEmpty(searchQuery))
            {
                string lowerSearchQuery = searchQuery.ToLower();

                artworkQuery = artworkQuery.Where(a =>
                    a.Title.ToLower().Contains(lowerSearchQuery));

                exhibitionQuery = exhibitionQuery.Where(e =>
                    e.Title.ToLower().Contains(lowerSearchQuery));

                artistQuery = artistQuery.Where(u => u.UserName.ToLower().Contains(lowerSearchQuery));
            }

            var artworks = await artworkQuery.OrderBy(a => Guid.NewGuid()).Take(4).ToListAsync();
            var artistProfiles = artistQuery.Any()
                ? await artistQuery.OrderBy(u => Guid.NewGuid()).Take(4).ToListAsync()
                : artistQuery.ToList();
            var exhibitions = exhibitionQuery.Any()
                ? await exhibitionQuery.OrderBy(e => Guid.NewGuid()).Take(4).ToListAsync()
                : exhibitionQuery.ToList();

            if (!artworks.Any())
            {
                ViewBag.NoArtworksMessage = "Нет публикаций.";
            }

            if (!artistProfiles.Any())
            {
                ViewBag.NoProfilesMessage = "Нет художников.";
            }

            if (!exhibitions.Any())
            {
                ViewBag.NoExhibitionsMessage = "Нет выставок.";
            }

            var model = new GalleryIndexViewModel
            {
                Artworks = artworks,
                ArtistProfiles = artistProfiles,
                Exhibitions = exhibitions,
                Categories = categories,
                SelectedCategoryId = categoryId
            };

            return View(model);
        }

    }

}
