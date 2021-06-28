using System;

namespace Prototype_Domain.Test
{
    public class AntwoordMogelijkheid
    {
        public int antwoordId { get; set; }
        public string mening { get; set; }
        public int stellingId { get; set; }

        public AntwoordMogelijkheid(int antwoordId, int stellingId, string mening)
        {
            this.antwoordId = antwoordId;
            this.mening = mening;
            this.stellingId = stellingId;
        }
    }
}
