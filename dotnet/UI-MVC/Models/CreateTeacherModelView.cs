using System.ComponentModel.DataAnnotations;

namespace UI.MVC.Models
{
    public class CreateTeacherModelView
    {
        [EmailAddress] public string Email { get; set; }
    }
}