using System.ComponentModel.DataAnnotations;

namespace BL.Domain.Sessie
{
    public class SessionSettings
    {
        [Key] public int Id { get; set; }

        public bool ArgumentsAllowed { get; set; }
        public bool SkipAllowed { get; set; }
        public bool DefinitionsGiven { get; set; }
        public bool ForceWaiting { get; set; }
        public string Colour1 { get; set; }
        public string Colour2 { get; set; }
        public string Colour3 { get; set; }
        public string Colour4 { get; set; }
        public string Colour5 { get; set; }
        public string Colour6 { get; set; }
        public string ColourSkip { get; set; }
    }
}