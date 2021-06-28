using BL.Domain.Sessie;

namespace UI.MVC.Models
{
    public class PastSessionModel
    {
        public int Id { get; set; }
        public string ClassName { get; set; }
        public GameType GameType { get; set; }
    }
}