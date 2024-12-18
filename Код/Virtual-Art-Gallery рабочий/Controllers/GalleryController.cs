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

        public async Task<IActionResult> Index(string searchQuery, int? categoryId)
        {
            // Получение данных для списка категорий
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

            // Фильтрация по категории
            if (categoryId.HasValue)
            {
                artworkQuery = artworkQuery.Where(a => a.CategoryId == categoryId.Value);
            }

            // Фильтрация по ключевым словам
            if (!string.IsNullOrEmpty(searchQuery))
            {
                string lowerSearchQuery = searchQuery.ToLower();

                // Поиск по названиям и авторам
                artworkQuery = artworkQuery.Where(a =>
                    a.Title.ToLower().Contains(lowerSearchQuery) ||
                    a.Creator.UserName.ToLower().Contains(lowerSearchQuery));

                // Поиск по выставкам и авторам
                exhibitionQuery = exhibitionQuery.Where(e =>
                    e.Title.ToLower().Contains(lowerSearchQuery) ||
                    e.Creator.UserName.ToLower().Contains(lowerSearchQuery));

                // Поиск по профилям художников
                artistQuery = artistQuery.Where(u => u.UserName.ToLower().Contains(lowerSearchQuery));
            }

            // Получение данных
            var artworks = await artworkQuery.OrderBy(a => Guid.NewGuid()).Take(4).ToListAsync();
            var artistProfiles = await artistQuery.OrderBy(u => Guid.NewGuid()).Take(4).ToListAsync();
            var exhibitions = await exhibitionQuery.OrderBy(e => Guid.NewGuid()).Take(4).ToListAsync();

            // Сообщения при отсутствии данных
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

            // Модель для представления
            var model = new GalleryIndexViewModel
            {
                Artworks = artworks,
                ArtistProfiles = artistProfiles,
                Exhibitions = exhibitions,
                Categories = categories
            };

            return View(model);
        }

    }

}
