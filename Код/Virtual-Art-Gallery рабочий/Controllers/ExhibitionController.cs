using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Virtual_Art_Gallery.Data;
using Virtual_Art_Gallery.Models;

namespace Virtual_Art_Gallery.Controllers
{
    public class ExhibitionController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly GalleryContext _context;

        public ExhibitionController(GalleryContext context, UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
            _context = context;
        }

        // GET: Exhibition
        public async Task<IActionResult> Index()
        {
            var exhibitions = await _context.Exhibitions
                .Include(e => e.Creator)
                .ToListAsync();

            return View(exhibitions);
        }

        // GET: Exhibition/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var userId = _userManager.GetUserId(User);
            var isAdmin = User.IsInRole("Administrator");

            var exhibition = _context.Exhibitions
                .Include(e => e.Artworks)
                .ThenInclude(a => a.Creator)
                .Include(c => c.Creator)
                .FirstOrDefault(e => e.Id == id);

            if (exhibition == null)
            {
                return NotFound();
            }

            exhibition.Artworks = exhibition.Artworks
                .Where(a =>
                    a.Status == ArtworkStatus.Approved ||
                    (a.CreatorId == userId || isAdmin))
                .ToList();

            return View(exhibition);
        }


        // GET: Exhibition/Create
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Exhibition/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,IsClosed,CreatorId")] ExhibitionModel exhibitionModel)
        {
            if (ModelState.IsValid)
            {
                if (!User.Identity.IsAuthenticated)
                {
                    return Unauthorized("Вы должны быть авторизованы для создания выставки.");
                }

                _context.Add(exhibitionModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(exhibitionModel);
        }

        // GET: Exhibition/Edit/5
        [Authorize]
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
            if (exhibitionModel.CreatorId != _userManager.GetUserId(User))
            {
                return Unauthorized("Редактировать выставку может только её создатель.");
            }
            return View(exhibitionModel);
        }

        // POST: Exhibition/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,IsClosed,CreatorId")] ExhibitionModel exhibitionModel)
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
        [Authorize]
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
            if (exhibitionModel.CreatorId != _userManager.GetUserId(User) && !User.IsInRole("Administrator"))
            {
                return Unauthorized("Удалять выставку может только её создатель или администратор.");
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
