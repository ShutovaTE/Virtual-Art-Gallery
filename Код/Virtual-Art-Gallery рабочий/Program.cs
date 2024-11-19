using Microsoft.AspNetCore.Identity;
using Virtual_Art_Gallery.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Registering Identity with RoleManager and Entity Framework
builder.Services.AddDbContext<GalleryContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("GalleryContext")));

// Add Identity services, including role management
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<GalleryContext>()
    .AddDefaultUI()
    .AddDefaultTokenProviders();  // Adding token providers is important if you're using password reset, etc.

// Add services to the container
builder.Services.AddControllersWithViews();

var app = builder.Build();

var scope = app.Services.CreateScope();
var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

// Create roles on startup if they don't exist
await CreateRoles(roleManager);

// Assign the "Administrator" role to the first registered user (optional)
await AssignAdminRoleToFirstUser(userManager);

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.MapRazorPages();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Gallery}/{action=Index}/{id?}");

app.Run();

// Method to create roles if they don't exist
async Task CreateRoles(RoleManager<IdentityRole> roleManager)
{
    string[] roleNames = { "User", "Administrator" };

    foreach (var roleName in roleNames)
    {
        var roleExist = await roleManager.RoleExistsAsync(roleName);
        if (!roleExist)
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }
}

// Method to assign the "Administrator" role to the first user (if desired)
async Task AssignAdminRoleToFirstUser(UserManager<IdentityUser> userManager)
{
    var users = userManager.Users.ToList();
    if (users.Count == 0)
    {
        // If there are no users, don't proceed
        return;
    }

    // Assuming the first user should be an administrator
    var firstUser = users.FirstOrDefault();
    if (firstUser != null)
    {
        var isUserInRole = await userManager.IsInRoleAsync(firstUser, "Administrator");
        if (!isUserInRole)
        {
            await userManager.AddToRoleAsync(firstUser, "Administrator");
        }
    }
}
