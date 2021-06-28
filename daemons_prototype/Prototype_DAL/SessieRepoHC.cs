using System;
using System.Collections.Generic;
using System.Linq;
using Prototype_Domain.Sessie;

namespace Prototype_DAL
{
    public class SessieRepoHC : ISessieRepository
    {
        private int currentKey;
        private Dictionary<int, List<LeerkrachtSessie>> repo;

        public SessieRepoHC()
        {
            repo = new Dictionary<int, List<LeerkrachtSessie>>();
            currentKey = 0;
            initialise();
        }



        private void initialise()
        {
            //geen voorgaande sessies beschikbaar binnen de POC
        }

        public int CreateSessie(int userId, LeerkrachtSessie sessie)
        {
            int sessionId = currentKey;
            currentKey++;
            sessie.SessieId = sessionId;
            if (!(repo.ContainsKey(userId)))
            {
                repo[userId] = new List<LeerkrachtSessie>();
            }

            repo[userId].Add(sessie);
            //WriteToFile(sessie);
            return sessionId;
        }

        public LeerkrachtSessie ReadSessie(int userId, int sessionId)
        {
            List<LeerkrachtSessie> sessies = repo[userId];
            return sessies.Find(s => s.SessieId == sessionId);
        }

        public void UpdateSessie(int userId, LeerkrachtSessie sessie)
        {
            repo[userId].Remove(sessie);
            repo[userId].Add(sessie);
        }

        public void DeleteSessie(int userId, int sessionId)
        {
            LeerkrachtSessie sessie = repo[userId].Find(s => s.SessieId == sessionId);
            if (sessie != null)
            {
                repo[userId].Remove(sessie);
            }
        }

        private void WriteToFile(LeerkrachtSessie sessie)
        {
            string[] lines = System.IO.File.ReadAllLines(@"../../../../Prototype_DAL/SessionReport.txt");
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"../../../../Prototype_DAL/SessionReport.txt"))
            {
                foreach (var line in lines)
                {
                    file.WriteLine(line);
                }

                file.WriteLine(DateTime.Now.ToString());
                file.WriteLine("Sessie: " + sessie.SessieId);
                file.WriteLine("User: " + sessie.UserId);
                file.WriteLine("Klas: " + sessie.Klas.Naam);
                file.WriteLine(sessie.SpelStrategy.ToString());
                file.WriteLine("\n");
            }
        }
    }
}