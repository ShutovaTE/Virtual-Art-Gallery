﻿using Microsoft.AspNetCore.Mvc;
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
            return View(new ArtworkModel()); 
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Description,CategoryId")] ArtworkModel artwork, IFormFile imageFile)
        {
            {
                try
                {
                    if (ModelState.IsValid)
                    {
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

            ViewData["CategoryList"] = new SelectList(_context.Categories, "Id", "Name", artwork.CategoryId);
            return View(artwork);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,CategoryId,ImagePath")] ArtworkModel artwork, IFormFile imageFile)
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

                    // Обновляем основные поля
                    existingArtwork.Title = artwork.Title;
                    existingArtwork.Description = artwork.Description;
                    existingArtwork.CategoryId = artwork.CategoryId;

                    if (imageFile != null && imageFile.Length > 0)
                    {
                        // Генерация имени для нового файла
                        var fileName = Path.GetFileNameWithoutExtension(imageFile.FileName) + "_" + Guid.NewGuid() + Path.GetExtension(imageFile.FileName);
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

                        // Копирование нового изображения
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(fileStream);
                        }

                        // Обновление пути к изображению
                        existingArtwork.ImagePath = "/images/" + fileName;

                        // Обновление размеров изображения
                        using (var img = Image.Load(filePath))
                        {
                            existingArtwork.Width = img.Width;
                            existingArtwork.Height = img.Height;
                        }

                        // Обновляем дату изменения
                        existingArtwork.DateCreated = DateTime.Now;
                    }
                    else
                    {
                        // Если изображение не загружается, сохраняем старые значения
                        artwork.ImagePath = existingArtwork.ImagePath;
                        artwork.Width = existingArtwork.Width;
                        artwork.Height = existingArtwork.Height;
                        artwork.DateCreated = existingArtwork.DateCreated;
                    }

                    // Обновляем запись в базе данных
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

            // Если модель не валидна, повторно передаем список категорий
            ViewData["CategoryList"] = new SelectList(_context.Categories, "Id", "Name", artwork.CategoryId);
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
            if (artwork == null)
            {
                return NotFound();
            }

            _context.Artworks.Remove(artwork);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        private bool ArtworkModelExists(int id)
        {
            return _context.Artworks.Any(e => e.Id == id);
        }

    }
}