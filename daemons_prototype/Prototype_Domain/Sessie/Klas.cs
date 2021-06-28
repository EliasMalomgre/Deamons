namespace Prototype_Domain.Sessie
{
    public class Klas
    {
        public string Naam { get; set; }
        public int AantalLln { get; set; }

        public Klas(string naam, int aantalLln)
        {
            this.Naam = naam;
            this.AantalLln = aantalLln;
        }
    }
}