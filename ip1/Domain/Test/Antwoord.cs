namespace Stemtest.BL.Domain.Test
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
        
    }
}
