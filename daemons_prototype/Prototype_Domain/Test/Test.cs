using System;
using System.Collections.Generic;
using Prototype_Domain.Identity;

namespace Prototype_Domain.Test
{
    public class Test
    {
        public string id { get; set; }
        public string naam { get; set; }
        public Leerkracht maker { get; set; }

        public List<Stelling> stellingen;
        public List<Tag> tags; //TODO (OOS)


        public void VoegVraagToe(string vraag, string uitleg)
        {
            Stelling stelling = new Stelling(stellingen.Count+1,vraag,uitleg);
        }

        public void VoegVraagToe(string vraag)
        {
            VoegVraagToe(vraag, null);
        }
        
        public Test(string id, string naam, Leerkracht maker)
        {
            this.stellingen = new List<Stelling>(); //TODO stellingen ophalen met een methode uit DAL of zo (OOS)
            this.id = id;
            this.naam = naam;
            this.maker = maker;
        }

        public string VerklaarWoord(string woord, int stellingId)
        {
            return stellingen[stellingId].VindWoordverklaring(woord);
        }

        public List<Woordverklaring> GetVerklaringen(int stellingId)
        {
            List<Woordverklaring> verklaringen = new List<Woordverklaring>();
            Stelling stelling = stellingen.Find(s => s.stellingID == stellingId);
            foreach (var woordverklaring in stelling.woordverklaringen)
            {
                verklaringen.Add(woordverklaring);
            }

            return verklaringen;
        }
    }
}