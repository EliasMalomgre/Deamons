using System.ComponentModel.DataAnnotations;

namespace UI.MVC.Models
{
    public class SchoolRequestEmailViewModel
    {
        [Required] public string SchoolName { get; set; }

        [Required] [EmailAddress] public string Email { get; set; }

        [Required] public string City { get; set; }

        [Required] public string PostalCode { get; set; }

        [Required] public string StreetAndNumber { get; set; }
    }
}