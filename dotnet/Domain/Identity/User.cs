using System.Collections.Generic;
using BL.Domain.Sessie;

namespace BL.Domain.Identity
{
    public class User
    {
        public int UserId { get; set; }
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public Organisation Organisation { get; set; }
        public List<Class> Classes { get; set; }
    }
}