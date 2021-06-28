using System.Collections.Generic;
using Stemtest.BL.Domain.Sessie;

namespace Stemtest.BL.Domain.Identity
{
    public class Leerkracht
    {
        public int UserId { get; set; }
        public string naam { get; set; }

        public List<Klas> Klassen { get; set; }

        
        public void VoegVraagToe(int vanTestId, int stellingId, int naarTestId) //interpretatie: kopieer stelling van test A naar test B
        {
            //TODO dal services nodig voor uitwerking
        }

        public Leerkracht(string naam)
        {
            this.naam = naam;
            Klassen = new List<Klas>();
        }
    }
}