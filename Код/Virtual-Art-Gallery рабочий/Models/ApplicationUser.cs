using Microsoft.AspNetCore.Identity;

namespace Virtual_Art_Gallery.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Contact { get; set; }
        public string AboutMe { get; set; }

    }
}
