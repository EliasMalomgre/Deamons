using Microsoft.AspNetCore.Identity;

namespace BL.Domain.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public User User { get; set; }
        public bool Blocked { get; set; }
    }
}