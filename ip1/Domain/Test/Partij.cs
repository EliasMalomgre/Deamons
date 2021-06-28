using System.Collections.Generic;

namespace Stemtest.BL.Domain.Test
{
    public class Partij
    {
        public string naam { get; set; }
        public string oriëntatie { get; set; }
        public string kleur { get; set; }
        public string partijLeider { get; set; }
        public string logo { get; set; }
        public List<Antwoord> opinies { get; set; }

        public Partij()
        {
            opinies = new List<Antwoord>(); //TODO opinies ophalen
        }

        public Partij(string naam)
        {
            this.naam = naam;
            opinies = new List<Antwoord>(); //TODO opinies ophalen
        }

        public Partij(string naam, string oriëntatie, string kleur, string partijLeider, string logo)
        {
            this.naam = naam;
            this.oriëntatie = oriëntatie;
            this.kleur = kleur;
            this.partijLeider = partijLeider;
            this.logo = logo;
            opinies = new List<Antwoord>(); //TODO opinies ophalen
        }
    }
}
