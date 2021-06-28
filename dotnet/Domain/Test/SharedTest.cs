using System.ComponentModel.DataAnnotations;
using BL.Domain.Identity;

namespace BL.Domain.Test
{
    public class SharedTest
    {
        [Key] public int Id { get; set; }

        public Test Test { get; set; }
        public Organisation Organisation { get; set; }
        public User Creator { get; set; }
        public bool PublicShared { get; set; }
        public bool OrganisationShared { get; set; }
    }
}