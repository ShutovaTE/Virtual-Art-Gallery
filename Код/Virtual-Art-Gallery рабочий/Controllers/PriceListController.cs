using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text.RegularExpressions;
using Virtual_Art_Gallery.Data;
using Virtual_Art_Gallery.Models;

namespace Virtual_Art_Gallery.Controllers
{
    public class PriceListController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly GalleryContext _context;

        public PriceListController(GalleryContext context, UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
            _context = context;
        }

        // GET: PriceList
        public async Task<IActionResult> Index()
        {
            var galleryContext = _context.Prices.Include(p => p.Creator);
            return View(await galleryContext.ToListAsync());
        }

        // GET: PriceList/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var priceListModel = await _context.Prices
                .Include(p => p.Creator)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (priceListModel == null)
            {
                return NotFound();
            }

            return View(priceListModel);
        }

        // GET: PriceList/Create
        [Authorize]
        public IActionResult Create()
        {
            ViewData["CreatorId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: PriceList/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,ImagePath,Price,CreatorId")] PriceListModel priceListModel, IFormFile imageFile)
        {
            if (ModelState.IsValid)
            {
                if (!User.Identity.IsAuthenticated)
                {
                    return Unauthorized("Вы должны быть авторизованы для создания прайса.");
                }

                if (!Regex.IsMatch(priceListModel.Price.ToString(CultureInfo.InvariantCulture), @"^\d+(\.\d{1,2})?$"))
                {
                    ModelState.AddModelError("Price", "Цена должна содержать не более двух цифр после запятой.");
                }

                if (imageFile != null && imageFile.Length > 0)
                {
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                    var fileExtension = Path.GetExtension(imageFile.FileName).ToLower();
                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        ModelState.AddModelError("ImageFile", "Недопустимый формат файла.");
                        return View(priceListModel);
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

                    priceListModel.ImagePath = "/images/" + fileName;
                }

                _context.Add(priceListModel);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", priceListModel);
            }
            ViewData["CreatorId"] = new SelectList(_context.Users, "Id", "Id", priceListModel.CreatorId);
            return View(priceListModel);
        }

        // GET: PriceList/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var price = await _context.Prices.FindAsync(id);
            if (price.CreatorId != _userManager.GetUserId(User))
            {
                return Unauthorized("Вы не имеете права редактировать этот прайс.");
            }

            if (price == null)
            {
                return NotFound();
            }

            price.Price = Math.Round(price.Price, 2);

            ViewData["CreatorId"] = new SelectList(_context.Users, "Id", "Id", price.CreatorId);
            return View(price);
        }

        // POST: PriceList/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Price,CreatorId,ImagePath")] PriceListModel priceListModel, IFormFile? imageFile)
        {
            if (id != priceListModel.Id)
            {
                return NotFound();
            }

            if (!Regex.IsMatch(priceListModel.Price.ToString(CultureInfo.InvariantCulture), @"^\d+(\.\d{1,2})?$"))
            {
                ModelState.AddModelError("Price", "Цена должна содержать не более двух цифр после запятой.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingPriceList = await _context.Prices.FindAsync(id);
                    if (existingPriceList == null)
                    {
                        return NotFound();
                    }

                    existingPriceList.Name = priceListModel.Name;
                    existingPriceList.Description = priceListModel.Description;
                    existingPriceList.Price = priceListModel.Price;

                    if (imageFile != null && imageFile.Length > 0)
                    {
                        var fileName = Path.GetFileNameWithoutExtension(imageFile.FileName) + "_" + Guid.NewGuid() + Path.GetExtension(imageFile.FileName);
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(fileStream);
                        }

                        existingPriceList.ImagePath = "/images/" + fileName;
                    }

                    _context.Update(existingPriceList);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PriceListModelExists(priceListModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction("Details", priceListModel);
            }

            ViewData["CreatorId"] = new SelectList(_context.Users, "Id", "Id", priceListModel.CreatorId);
            return View(priceListModel);
        }



        // GET: PriceList/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var priceListModel = await _context.Prices
                .Include(p => p.Creator)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (priceListModel == null)
            {
                return NotFound();
            }

            return View(priceListModel);
        }

        // POST: PriceList/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var priceListModel = await _context.Prices.FindAsync(id);
            var userId = _userManager.GetUserId(User);
            var creatorId = priceListModel.CreatorId;
            if (priceListModel != null)
            {
                _context.Prices.Remove(priceListModel);
            }

            await _context.SaveChangesAsync();
            if (userId == creatorId)
            {
                return RedirectToAction("IndexPrices", "Profile");
            }
            else
            {
                return RedirectToAction("ProfilePrices", "Profile", new {userId = creatorId});
            }
        }

        private bool PriceListModelExists(int id)
        {
            return _context.Prices.Any(e => e.Id == id);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Order(int id)
        {
            var price = await _context.Prices.FindAsync(id);
            if (price == null)
            {
                return NotFound("Прайс не найден.");
            }

            TempData["OrderMessage"] = $"Вы заказали работу \"{price.Name}\". Вы сможете списаться с художником по почте, когда он получит уведомление о заказе.";

            return RedirectToAction("Details", price);
        }

    }
}
