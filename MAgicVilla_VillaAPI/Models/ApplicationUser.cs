using Microsoft.AspNetCore.Identity;

namespace MAgicVilla_VillaAPI.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
    }
}
