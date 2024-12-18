using Microsoft.AspNetCore.Identity;
using Virtual_Art_Gallery.Data;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<GalleryContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("GalleryContext")));

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<GalleryContext>()
    .AddDefaultUI()
    .AddDefaultTokenProviders(); 

builder.Services.AddControllersWithViews();

var app = builder.Build();

var scope = app.Services.CreateScope();
var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

await CreateRoles(roleManager);

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

async Task AssignAdminRoleToFirstUser(UserManager<IdentityUser> userManager)
{
    var users = userManager.Users.ToList();
    if (users.Count == 0)
    {
        return;
    }

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

var cultureInfo = new CultureInfo("ru-RU");
cultureInfo.NumberFormat.CurrencyDecimalSeparator = ",";
cultureInfo.NumberFormat.NumberDecimalSeparator = ",";

CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
