﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp;
using Virtual_Art_Gallery.Data;
using Virtual_Art_Gallery.Models;

namespace Virtual_Art_Gallery.Controllers
{
    public class ArtworkController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly GalleryContext _context;

        public ArtworkController(UserManager<IdentityUser> userManager, GalleryContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        // GET: Artwork
        public async Task<IActionResult> Index()
        {
            IQueryable<ArtworkModel> artworks;

            if (User.IsInRole("Administrator"))
            {
                artworks = _context.Artworks.Include(a => a.Category)
                    .Include(a => a.Creator); ;
            }
            else
            {
                artworks = _context.Artworks.Include(a => a.Category).Include(a => a.Creator).Where(a => a.Status == ArtworkStatus.Approved);
            }

            return View(await artworks.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var artwork = await _context.Artworks
                .Include(a => a.Category)
                .Include(a => a.Creator)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (artwork == null)
            {
                return NotFound();
            }

            artwork.ViewCount++;
            await _context.SaveChangesAsync();

            var userId = _userManager.GetUserId(User);
            var userLiked = await _context.Likes.AnyAsync(l => l.ArtworkId == artwork.Id && l.UserId == userId);

            artwork.LikeCount = await _context.Likes.CountAsync(l => l.ArtworkId == artwork.Id);

            ViewData["UserLiked"] = userLiked;

            return View(artwork);
        }


        // GET: Artwork/Create
        [Authorize]
        public IActionResult Create(int? exhibitionId)
        {
            ViewData["ExhibitionId"] = exhibitionId;
            ViewData["CategoryList"] = new SelectList(_context.Categories, "Id", "Name");
            return View(new ArtworkModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
          [Bind("Title,Description,CategoryId,CreatorId")] ArtworkModel artwork,
          IFormFile imageFile,
          int? exhibitionId)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    if (!User.Identity.IsAuthenticated)
                    {
                        return Unauthorized("Вы должны быть авторизованы для создания публикации.");
                    }

                    artwork.ExhibitionId = exhibitionId;
                    artwork.DateCreated = DateTime.Now;

                    if (imageFile != null && imageFile.Length > 0)
                    {
                        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                        var fileExtension = Path.GetExtension(imageFile.FileName).ToLower();
                        if (!allowedExtensions.Contains(fileExtension))
                        {
                            ModelState.AddModelError("ImageFile", "Недопустимый формат файла.");
                            ViewData["CategoryList"] = new SelectList(_context.Categories, "Id", "Name", artwork.CategoryId);
                            return View(artwork);
                        }

                        var fileName = Path.GetFileNameWithoutExtension(imageFile.FileName) + "_" + Guid.NewGuid() + fileExtension;
                        var imagesFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                        if (!Directory.Exists(imagesFolder))
                        {
                            Directory.CreateDirectory(imagesFolder);
                        }

                        var filePath = Path.Combine(imagesFolder, fileName);
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(fileStream);
                        }

                        using (var img = Image.Load(filePath))
                        {
                            artwork.Width = img.Width;
                            artwork.Height = img.Height;
                        }

                        artwork.ImagePath = "/images/" + fileName;
                    }

                    artwork.Status = ArtworkStatus.Draft;
                    _context.Add(artwork);
                    await _context.SaveChangesAsync();
                    if (exhibitionId.HasValue)
                    {
                        return RedirectToAction("Details", "Exhibition", new { id = exhibitionId });
                    }
                    else
                    {
                        return RedirectToAction(nameof(Index));
                    }
                }

                ViewData["CategoryList"] = new SelectList(_context.Categories, "Id", "Name", artwork.CategoryId);
                return View(artwork);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Общая ошибка: {ex.Message}");
                ViewData["CategoryList"] = new SelectList(_context.Categories, "Id", "Name", artwork.CategoryId);
                return View(artwork);
            }
        }

        // GET: Artwork/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var artwork = await _context.Artworks.FindAsync(id);
            if (artwork == null || artwork.CreatorId != _userManager.GetUserId(User))
            {
                return Unauthorized("Вы не имеете права редактировать эту публикацию.");
            }

            ViewData["CategoryList"] = new SelectList(_context.Categories, "Id", "Name", artwork.CategoryId);
            return View(artwork);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,CategoryId,ImagePath,CreatorId")] ArtworkModel artwork, IFormFile imageFile)
        {
            if (id != artwork.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingArtwork = await _context.Artworks.FindAsync(id);
                    if (existingArtwork == null)
                    {
                        return NotFound();
                    }

                    if (existingArtwork.Status != ArtworkStatus.Draft)
                    {
                        return Unauthorized("Вы не можете редактировать публикацию, так как её статус не является черновиком.");
                    }

                    if (existingArtwork.CreatorId != artwork.CreatorId)
                    {
                        return Unauthorized("Вы не можете изменить создателя этой публикации.");
                    }

                    existingArtwork.Title = artwork.Title;
                    existingArtwork.Description = artwork.Description;
                    existingArtwork.CategoryId = artwork.CategoryId;

                    if (imageFile != null && imageFile.Length > 0)
                    {
                        var fileName = Path.GetFileNameWithoutExtension(imageFile.FileName) + "_" + Guid.NewGuid() + Path.GetExtension(imageFile.FileName);
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(fileStream);
                        }

                        existingArtwork.ImagePath = "/images/" + fileName;

                        using (var img = Image.Load(filePath))
                        {
                            existingArtwork.Width = img.Width;
                            existingArtwork.Height = img.Height;
                        }

                        existingArtwork.DateCreated = DateTime.Now;
                    }
                    else
                    {
                        artwork.ImagePath = existingArtwork.ImagePath;
                        artwork.Width = existingArtwork.Width;
                        artwork.Height = existingArtwork.Height;
                        artwork.DateCreated = existingArtwork.DateCreated;
                    }

                    _context.Update(existingArtwork);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ArtworkModelExists(artwork.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["CategoryList"] = new SelectList(_context.Categories, "Id", "Name", artwork.CategoryId);
            return View(artwork);
        }


        // GET: Artwork/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var artwork = await _context.Artworks.Include(a => a.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (artwork == null)
            {
                return NotFound();
            }

            return View(artwork);
        }

        // POST: Artwork/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var artwork = await _context.Artworks.FindAsync(id);
            if (artwork == null || artwork.CreatorId != _userManager.GetUserId(User))
            {
                return Unauthorized("Вы не имеете права удалять эту публикацию.");
            }

            _context.Artworks.Remove(artwork);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Profile");
        }


        private bool ArtworkModelExists(int id)
        {
            return _context.Artworks.Any(e => e.Id == id);
        }

        public async Task<IActionResult> UnassignedArtworks()
        {
            var artworks = await _context.Artworks
                .Where(a => a.ExhibitionId == null)
                .ToListAsync();

            return View(artworks);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignToExhibition(int artworkId, int exhibitionId)
        {
            var artwork = await _context.Artworks.FindAsync(artworkId);
            if (artwork == null)
            {
                return NotFound();
            }

            artwork.ExhibitionId = exhibitionId;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> UpdateStatus(int id, ArtworkStatus status)
        {
            var artwork = await _context.Artworks.FindAsync(id);
            if (artwork == null)
            {
                return NotFound();
            }

            switch (status)
            {
                case ArtworkStatus.Draft:
                    artwork.Status = ArtworkStatus.Draft;
                    break;

                case ArtworkStatus.Submitted:
                    artwork.Status = ArtworkStatus.Submitted;
                    break;

                case ArtworkStatus.Approved:
                    artwork.Status = ArtworkStatus.Approved;
                    break;

                case ArtworkStatus.Rejected:
                    artwork.Status = ArtworkStatus.Rejected;
                    break;

                default:
                    return BadRequest("Некорректный статус.");
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> AllArtworks()
        {
            var artworks = await _context.Artworks.Include(a => a.Category).ToListAsync();
            return View(artworks);
        }

        // GET: Artwork/AdminDelete/5
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> AdminDelete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var artwork = await _context.Artworks.Include(a => a.Category)
                .Include(a => a.Creator)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (artwork == null)
            {
                return NotFound();
            }

            return View(artwork);
        }

        [HttpPost, ActionName("AdminDelete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> AdminDeleteConfirmed(int id)
        {
            var artwork = await _context.Artworks.FindAsync(id);
            if (artwork == null)
            {
                return NotFound();
            }

            _context.Artworks.Remove(artwork);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<int> GetLikesCount(int artworkId)
        {
            return await _context.Likes.CountAsync(l => l.ArtworkId == artworkId);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Like(int id)
        {
            var userId = _userManager.GetUserId(User);

            var existingLike = await _context.Likes.FirstOrDefaultAsync(l => l.ArtworkId == id && l.UserId == userId);
            if (existingLike != null)
            {
                return BadRequest("Вы уже поставили лайк этому произведению.");
            }

            var like = new LikeModel
            {
                ArtworkId = id,
                UserId = userId
            };
            _context.Likes.Add(like);

            var artwork = await _context.Artworks.FindAsync(id);
            if (artwork != null)
            {
                artwork.LikeCount++;
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id });
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Unlike(int id)
        {
            var userId = _userManager.GetUserId(User);

            var existingLike = await _context.Likes.FirstOrDefaultAsync(l => l.ArtworkId == id && l.UserId == userId);
            if (existingLike == null)
            {
                return BadRequest("Вы еще не поставили лайк этому произведению.");
            }

            _context.Likes.Remove(existingLike);

            var artwork = await _context.Artworks.FindAsync(id);
            if (artwork != null && artwork.LikeCount > 0)
            {
                artwork.LikeCount--;
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id });
        }

        [Authorize]
        public async Task<IActionResult> LikedArtworks()
        {
            var userId = _userManager.GetUserId(User);

            var likedArtworks = await _context.Likes
                .Where(like => like.UserId == userId)
                .Include(like => like.Artwork)
                .ThenInclude(artwork => artwork.Category)
                .Select(like => like.Artwork)
                .ToListAsync();

            return View(likedArtworks);
        }

    }
}