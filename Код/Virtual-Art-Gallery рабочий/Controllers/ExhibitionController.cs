using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Virtual_Art_Gallery.Data;
using Virtual_Art_Gallery.Models;

namespace Virtual_Art_Gallery.Controllers
{
    public class ExhibitionController : Controller
    {
        private readonly GalleryContext _context;

        public ExhibitionController(GalleryContext context)
        {
            _context = context;
        }

        // GET: Exhibition
        public async Task<IActionResult> Index()
        {
            return View(await _context.Exhibitions.ToListAsync());
        }

        // GET: Exhibition/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var exhibition = _context.Exhibitions
                .Include(e => e.Artworks)
                .ThenInclude(a => a.Category)
                .FirstOrDefault(e => e.Id == id);

            if (exhibition == null)
            {
                return NotFound();
            }

            return View(exhibition);
        }

        // GET: Exhibition/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Exhibition/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,IsClosed")] ExhibitionModel exhibitionModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(exhibitionModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(exhibitionModel);
        }

        // GET: Exhibition/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var exhibitionModel = await _context.Exhibitions.FindAsync(id);
            if (exhibitionModel == null)
            {
                return NotFound();
            }
            return View(exhibitionModel);
        }

        // POST: Exhibition/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,IsClosed")] ExhibitionModel exhibitionModel)
        {
            if (id != exhibitionModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(exhibitionModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ExhibitionModelExists(exhibitionModel.Id))
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
            return View(exhibitionModel);
        }

        // GET: Exhibition/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var exhibitionModel = await _context.Exhibitions
                .FirstOrDefaultAsync(m => m.Id == id);
            if (exhibitionModel == null)
            {
                return NotFound();
            }

            return View(exhibitionModel);
        }

        // POST: Exhibition/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var exhibitionModel = await _context.Exhibitions.FindAsync(id);
            if (exhibitionModel != null)
            {
                _context.Exhibitions.Remove(exhibitionModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ExhibitionModelExists(int id)
        {
            return _context.Exhibitions.Any(e => e.Id == id);
        }
    }
}
