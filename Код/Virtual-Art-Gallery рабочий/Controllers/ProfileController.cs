using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Virtual_Art_Gallery.Data;
using Virtual_Art_Gallery.Models;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Authentication;

[Authorize]
public class ProfileController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly GalleryContext _context;

    public ProfileController(UserManager<IdentityUser> userManager, GalleryContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var userId = _userManager.GetUserId(User); 
        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            return NotFound();
        }

        var artworks = await _context.Artworks
            .Where(a => a.CreatorId == userId) 
            .ToListAsync();

        var model = new ProfileViewModel
        {
            Username = user.UserName,
            Email = user.Email,
            Artworks = artworks
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SubmitForApproval(int id)
    {
        var artwork = await _context.Artworks.FindAsync(id);
        if (artwork == null || artwork.CreatorId != _userManager.GetUserId(User))
        {
            return NotFound();
        }

        if (artwork.Status == ArtworkStatus.Draft)
        {
            artwork.Status = ArtworkStatus.Submitted;
            _context.Update(artwork);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction("Index", "Profile");
    }

    [AllowAnonymous]
    public async Task<IActionResult> AllProfiles()
    {
        var profiles = await _userManager.Users
            .Where(user => _context.Artworks.Any(a => a.CreatorId == user.Id && a.Status == ArtworkStatus.Approved))
            .Select(user => new ProfileViewModel
            {
                Username = user.UserName,
                Email = user.Email,
                Artworks = _context.Artworks
                    .Where(a => a.CreatorId == user.Id && a.Status == ArtworkStatus.Approved)
                    .OrderByDescending(a => a.DateCreated) 
                    .Take(3) 
                    .ToList()
            })
            .ToListAsync();

        return View(profiles);
    }


    [AllowAnonymous]
    public async Task<IActionResult> ViewProfile(string userId)
    {
        if (string.IsNullOrEmpty(userId))
        {
            return NotFound();
        }

        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            return NotFound();
        }

        var artworks = await _context.Artworks
            .Where(a => a.CreatorId == userId && a.Status == ArtworkStatus.Approved) 
            .ToListAsync();

        var model = new ProfileViewModel
        {
            Username = user.UserName,
            Email = user.Email,
            Artworks = artworks
        };

        return View(model);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteProfile()
    {
        var userId = _userManager.GetUserId(User);
        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            return NotFound();
        }

        var artworks = _context.Artworks.Where(a => a.CreatorId == userId);
        _context.Artworks.RemoveRange(artworks);

        var result = await _userManager.DeleteAsync(user);

        if (!result.Succeeded)
        {
            ModelState.AddModelError("", "Не удалось удалить профиль. Пожалуйста, попробуйте еще раз.");
            return RedirectToAction("Index");
        }

        await _context.SaveChangesAsync();

        await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);

        return RedirectToAction("Index", "Gallery"); 
    }

    [Authorize(Roles = "Administrator")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteArtistProfile(string userId)
    {
        if (string.IsNullOrEmpty(userId))
        {
            return NotFound();
        }

        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            return NotFound();
        }

        var artworks = _context.Artworks.Where(a => a.CreatorId == userId);
        _context.Artworks.RemoveRange(artworks);

        var result = await _userManager.DeleteAsync(user);

        if (!result.Succeeded)
        {
            ModelState.AddModelError("", "Не удалось удалить профиль. Пожалуйста, попробуйте снова.");
            return RedirectToAction("AllProfiles");
        }

        await _context.SaveChangesAsync();

        return RedirectToAction("AllProfiles");
    }

    public async Task<IActionResult> ProfileExhibitions(string userId)
    {
        var user = string.IsNullOrEmpty(userId)
            ? await _userManager.Users.FirstOrDefaultAsync(u => u.Id == _userManager.GetUserId(User))
            : await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            return NotFound();
        }

        var exhibitions = await _context.Exhibitions
            .Where(e => e.CreatorId == user.Id)
            .ToListAsync();

        var model = new ProfileViewModel
        {
            Username = user.UserName,
            Email = user.Email,
            Exhibitions = exhibitions
        };

        if (!exhibitions.Any())
        {
            ViewBag.NoExhibitionsMessage = "Нет выставок";
        }

        return View(model);
    }
}
