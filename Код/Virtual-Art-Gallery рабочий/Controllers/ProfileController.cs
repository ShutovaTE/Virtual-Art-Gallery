using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Virtual_Art_Gallery.Data;
using Virtual_Art_Gallery.Models;
using System.Threading.Tasks;
using System.Linq;

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


}
