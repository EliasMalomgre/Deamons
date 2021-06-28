using System.Collections.Generic;

namespace UI.MVC.Models
{
    public class StartSessionViewModel
    {
        public int Test { get; set; }
        public string Type { get; set; }
        public string ClassName { get; set; }
        public string UserId { get; set; }
        public bool Arguments { get; set; }
        public bool Skip { get; set; }
        public bool Definitions { get; set; }
        public bool ForceWaiting { get; set; }
        public List<int> SelectedStatementsId { get; set; }
        public string Colour1 { get; set; }
        public string Colour2 { get; set; }
        public string Colour3 { get; set; }
        public string Colour4 { get; set; }
        public string Colour5 { get; set; }
        public string Colour6 { get; set; }
        public string SkipColour { get; set; }
        public bool PreparingSession { get; set; }
    }
}