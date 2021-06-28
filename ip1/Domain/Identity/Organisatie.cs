namespace Stemtest.BL.Domain.Identity
{
    public class Organisatie
    {
        public string naam { get; set; }
        public string straatEnNummer { get; set; }
        public string gemeente { get; set; }
        public string postcode { get; set; }
        public string kleur { get; set; } //later nodig voor interface customization
        public string logo { get; set; } //idem aan /\

        public Organisatie(string naam)
        {
            this.naam = naam;
        }
    }
}