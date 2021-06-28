namespace Prototype_Domain.Test
{
    public class Antwoord : AntwoordMogelijkheid
    {
        public string Argumentering { get; set; }
        public bool Correct { set; get; }

        public Antwoord(int antwoordId, int stellingId, string mening, string argumentering="Geen mening") : base(antwoordId, stellingId, mening)
        {
            Argumentering = argumentering;
        }
        
        //convert antwoordmogelijkheid naar antwoord
        public Antwoord(AntwoordMogelijkheid antwoordMogelijkheid, string argumentering) : base(antwoordMogelijkheid.antwoordId, antwoordMogelijkheid.stellingId, antwoordMogelijkheid.mening)
        {
            Argumentering = argumentering;
        }


        public static bool operator ==(Antwoord a1, Antwoord a2)
        {
            if (a1.mening.Equals(a2.mening))
            {
                return true;
            }

            return false;
        }
        
        public static bool operator !=(Antwoord a1, Antwoord a2)
        {
            if (a1.mening.Equals(a2.mening))
            {
                return true;
            }
            return false;
        }
    }
}
