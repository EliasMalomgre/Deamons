using System;
using System.Collections.Generic;

namespace Stemtest.BL.Domain.Test
{
    public class Stelling
    {
        public int stellingID { get; set; }
        public String tekst { get; set; }
        public String uitleg { get; set; }
        
        public List<Woordverklaring> woordverklaringen { get; set; }

        public List<AntwoordMogelijkheid> antwoorden { get; set; }

        public Stelling(int stellingId)
        {
            woordverklaringen = new List<Woordverklaring>(); //TODO ergens die verklaringen ophalen (OOS)
            antwoorden = new List<AntwoordMogelijkheid>(); //TODO ergens die antwoorden ophalen
            stellingID = stellingId;
        }

        public Stelling(int stellingId, string tekst, string uitleg = "geen uitleg")
        {
            woordverklaringen = new List<Woordverklaring>(); //TODO ergens die verklaringen ophalen (OOS)
            antwoorden = new List<AntwoordMogelijkheid>{new AntwoordMogelijkheid(0,stellingId,"Ja"),new AntwoordMogelijkheid(1,stellingId,"Nee")}; //TODO ergens die antwoorden ophalen PAS DIT AAN
            this.stellingID = stellingId;
            this.tekst = tekst;
            this.uitleg = uitleg;
        }

        public string VindWoordverklaring(string woord)
        {
            Woordverklaring wv = woordverklaringen.Find(w => w.woord == woord);
            if (wv != null)
            {
                return wv.verklaring;
            }

            return "Geen verklaring gevonden";
        }
        
    }
}
