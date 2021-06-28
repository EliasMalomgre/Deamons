using System.ComponentModel.DataAnnotations;

namespace BL.Domain.Identity
{
    public class Organisation
    {
        public Organisation(string name)
        {
            Name = name;
        }

        [Key] public int Id { get; set; }

        public bool Blocked { get; set; }
        public string Name { get; set; }
        public string StreetAndNumber { get; set; }
        public string City { get; set; }
        public string Postalcode { get; set; }
        public string Colour { get; set; } //later nodig voor interface customization
        public string Logo { get; set; } //idem aan /\
    }
}