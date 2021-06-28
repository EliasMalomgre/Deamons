using System;
using System.Collections.Generic;
using System.Threading;
using Prototype_Domain.Test;

namespace Prototype_Domain.Sessie
{
    public class LeerkrachtSessie
    {
        public int SessieId { get; set; }
        public int MaxAantalDeelnemers { get; set; }
        public int HuidigAantalDeelnemers { get; set; }
        public Klas Klas { get; set; }
        public Test.Test test { get; set; }
        public ISpelStrategy SpelStrategy { get; set; }
        public int UserId { get; set; }

        public Dictionary<int, LeerlingSessie> LeerlingSessies;
        public Dictionary<string, List<Antwoord>> PartijAntwoorden;

        //Simple constructor purely for testing purposes
       

        //ctor voor startSession uit sessieService
        public LeerkrachtSessie(Test.Test test, ISpelStrategy spelStrategy, Klas klas, int userId)
        {
            LeerlingSessies = new Dictionary<int, LeerlingSessie>();
            PartijAntwoorden = new Dictionary<string, List<Antwoord>>();
            this.test = test;
            this.Klas = klas;
            this.SpelStrategy = spelStrategy;
            this.UserId = userId;
            PartijAntwoorden = new Dictionary<string, List<Antwoord>>();
            MaxAantalDeelnemers = Klas.AantalLln;
        }

        public void MakeNewLeerlingSessie(int leerlingId)
        {
            if (HuidigAantalDeelnemers==MaxAantalDeelnemers)
            {
                //TODO:Exception maken 
            }
            else
            {
                var lls = new LeerlingSessie(leerlingId);
                LeerlingSessies.Add(leerlingId,lls);
                HuidigAantalDeelnemers++;
            }
            
        }
        

        public string VerklaarWoord(string woord, int stellingId)
        {
            return test.VerklaarWoord(woord, stellingId);
        }
    }
}