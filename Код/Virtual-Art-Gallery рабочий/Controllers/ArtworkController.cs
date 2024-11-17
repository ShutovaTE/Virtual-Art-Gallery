using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Virtual_Art_Gallery.Data;
using SixLabors.ImageSharp;
using Virtual_Art_Gallery.Models;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Virtual_Art_Gallery.Controllers
{
    public class ArtworkController : Controller
    {
        private readonly GalleryContext _context;

        public ArtworkController(GalleryContext context)
        {
            _context = context;
        }

        // GET: Artwork
        public async Task<IActionResult> Index()
        {
            var artworks = await _context.Artworks.Include(a => a.Category).ToListAsync();
            return View(artworks);
        }

        // GET: Artwork/Details/5
        public async Task<IActionResult> Details(int? id)
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

        // GET: Artwork/Create
        public IActionResult Create()
        {
            ViewData["CategoryList"] = new SelectList(_context.Categories, "Id", "Name");
            return View(new ArtworkModel()); // Инициализируем пустую модель
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Description,CategoryId,ImagePath")] ArtworkModel artwork, IFormFile imageFile)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Проверяем, был ли загружен файл
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        Console.WriteLine($"Получен файл: {imageFile.FileName}, размер: {imageFile.Length} байт");

                        // Проверка расширения файла (только изображения)
                        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                        var fileExtension = Path.GetExtension(imageFile.FileName).ToLower();
                        if (!allowedExtensions.Contains(fileExtension))
                        {
                            ModelState.AddModelError("ImageFile", "Недопустимый формат файла.");
                            ViewData["CategoryList"] = new SelectList(_context.Categories, "Id", "Name", artwork.CategoryId);
                            return View(artwork);
                        }

                        // Генерация уникального имени файла
                        var fileName = Path.GetFileNameWithoutExtension(imageFile.FileName) + "_" + Guid.NewGuid() + fileExtension;
                        var imagesFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");

                        // Создаем папку, если она не существует
                        if (!Directory.Exists(imagesFolder))
                        {
                            Directory.CreateDirectory(imagesFolder);
                            Console.WriteLine("Папка для изображений создана.");
                        }

                        var filePath = Path.Combine(imagesFolder, fileName);
                        Console.WriteLine($"Путь сохранения файла: {filePath}");

                        // Сохраняем файл на диск
                        try
                        {
                            using (var fileStream = new FileStream(filePath, FileMode.Create))
                            {
                                await imageFile.CopyToAsync(fileStream);
                            }
                            Console.WriteLine("Файл успешно сохранен.");
                        }
                        catch (Exception fileEx)
                        {
                            Console.WriteLine($"Ошибка при сохранении файла: {fileEx.Message}");
                            ModelState.AddModelError("ImageFile", "Ошибка при сохранении изображения.");
                            ViewData["CategoryList"] = new SelectList(_context.Categories, "Id", "Name", artwork.CategoryId);
                            return View(artwork);
                        }

                        // Обработка изображения с помощью ImageSharp
                        try
                        {
                            using (var img = Image.Load(filePath))
                            {
                                artwork.Width = img.Width;
                                artwork.Height = img.Height;
                            }
                            artwork.ImagePath = "/images/" + fileName;
                            Console.WriteLine($"Размер изображения: {artwork.Width}x{artwork.Height}");
                        }
                        catch (Exception imageEx)
                        {
                            Console.WriteLine($"Ошибка при обработке изображения: {imageEx.Message}");
                            ModelState.AddModelError("ImageFile", "Ошибка при обработке изображения.");
                            ViewData["CategoryList"] = new SelectList(_context.Categories, "Id", "Name", artwork.CategoryId);
                            return View(artwork);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("ImageFile", "Изображение не загружено.");
                    }

                    _context.Add(artwork);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
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
            if (artwork == null)
            {
                return NotFound();
            }

            ViewData["CategoryList"] = new SelectList(_context.Categories, "Id", "Name");
            return View(artwork);
        }

        // POST: Artwork/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,CategoryId,ImagePath,Width,Height")] ArtworkModel artwork, IFormFile imageFile)
        {
            if (id != artwork.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (imageFile != null)
                    {
                        // Генерация уникального имени для файла
                        var fileName = Path.GetFileNameWithoutExtension(imageFile.FileName) + Guid.NewGuid() + Path.GetExtension(imageFile.FileName);
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(fileStream);
                        }

                        // Сохраняем путь изображения в базе данных
                        artwork.ImagePath = "/images/" + fileName;

                        // Получаем размеры изображения
                        using (var img = Image.Load(filePath))
                        {
                            artwork.Width = img.Width;
                            artwork.Height = img.Height;
                        }
                    }

                    _context.Update(artwork);
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

            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", artwork.CategoryId);
            return View(artwork);
        }

        // GET: Artwork/Delete/5
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
            if (artwork != null)
            {
                _context.Artworks.Remove(artwork);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ArtworkModelExists(int id)
        {
            return _context.Artworks.Any(e => e.Id == id);
        }

    }
}
