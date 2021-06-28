using System;
using System.Collections.Generic;
using Prototype_Domain.Test;

namespace Prototype_Domain.Sessie
{
    public class LeerlingSessie
    {
        public int Id { get; set; }
        public string TestId { get; set; }
        public int Score { get; set; }
        public int LaatstBeantwoordeStelling { get; set; }

        private Dictionary<int, Antwoord> antwoorden;
        //Partijspel
        public string GeselecteerdePartij { get; set; }



        public LeerlingSessie(int llnId)
        {
            this.Id = llnId;
            antwoorden =new Dictionary<int, Antwoord>();
        }

        public void AddAntwoord(string argumentering, AntwoordMogelijkheid antwoordMogelijkheid)
        {
            Antwoord antwoord= new Antwoord(antwoordMogelijkheid,argumentering);
            LaatstBeantwoordeStelling++;
            antwoorden[LaatstBeantwoordeStelling] = antwoord;
        }

        public Antwoord GetAntwoord(int antwoordId)
        {
            Antwoord antwoord =  antwoorden[antwoordId];
            return antwoord;
        }

        public List<Antwoord> GetAlleAntwoorden()
        {
            List<Antwoord> antwoordList = new List<Antwoord>();
            foreach (var antwoord in antwoorden.Values)
            {
                antwoordList.Add(antwoord);
            }

            return antwoordList;
        }

        public void addScore()
        {
            Score++;
        }

        public void updateAntwoorden(List<Antwoord> antwoorden)
        {
            if (antwoorden.Count!=this.antwoorden.Count)
            {
                throw new Exception("Antwoordenlijst is niet even groot");
            }
            else
            {
                for (int i = 0; i < antwoorden.Count; i++)
                {
                    this.antwoorden[i] = antwoorden[i];
                }
            }
            
        }
    }
}