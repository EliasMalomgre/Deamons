using System;
﻿using System.Collections;
using System.Collections.Generic;
using Prototype_Domain.Sessie;
using Prototype_Domain.Sessie.SpelStrategies;
using Prototype_Domain.Test;

namespace Prototype_BL
{
    public class GameManager
    {
        private ITestManager _testManager;
        private IIdentityManager _identityManager;
        private IPartijManager _partijManager;
        private ISessieManager _sessieManager;
        private Dictionary<int, LeerkrachtSessie> leerkrachtSessies;

        public GameManager()
        {
            _testManager = new TestManager();
            _identityManager = new IdentityManager();
            _partijManager = new PartijManager();
            _sessieManager = new SessieManager();
            leerkrachtSessies = new Dictionary<int, LeerkrachtSessie>();
        }

        public void StartSessie(string testId, SoortSpel soortSpel, List<string> gekozenPartijen, string klasNaam, int userId)
        {
            Test test = _testManager.Read(testId);
            Klas klas = _identityManager.ReadKlas(userId, klasNaam);
            ISpelStrategy spelStrategy;
            switch (soortSpel)
            {
                case SoortSpel.DEBATSPEL:
                    spelStrategy = new DebatSpel();
                    break;
                case SoortSpel.PARTIJSPEL:
                    spelStrategy = new PartijSpel();
                    break;
                case SoortSpel.EIGENSPEL:
                default:
                    spelStrategy = new EigenSpel();
                    break;
            }
            LeerkrachtSessie lks = new LeerkrachtSessie(test,spelStrategy,klas, userId);

            foreach (string partijnaam in gekozenPartijen)
            {
                Partij partij = _partijManager.Read(partijnaam);
                lks.PartijAntwoorden[partij.naam] =  partij.opinies;
            }
            leerkrachtSessies[userId] = lks;
            _sessieManager.CreateSessie(lks.UserId, lks);
        }

        public void BeginSpel(int userId, int leerlingId)
        {
            LeerkrachtSessie lks = leerkrachtSessies[userId];
            lks.MakeNewLeerlingSessie(leerlingId);
            _sessieManager.UpdateSessie(lks.UserId, lks);
        }

        public void SelecteerPartij(int leerlingId, int userId, string geselecteerdePartij)
        {
            LeerkrachtSessie lks = leerkrachtSessies[userId];
            lks.LeerlingSessies[leerlingId].GeselecteerdePartij = geselecteerdePartij;
            _sessieManager.UpdateSessie(lks.UserId,lks);
        }

        public void BeantwoordStelling(string argumentering, int leerlingId, int userId, int gekozenAntwoord)
        {
            LeerkrachtSessie lks = leerkrachtSessies[userId];
            var test = lks.test;
            var lls = lks.LeerlingSessies[leerlingId];
            int stellingId = lls.LaatstBeantwoordeStelling ;
            AntwoordMogelijkheid antwoordMogelijkheid = test.stellingen[stellingId].antwoorden[gekozenAntwoord];
            lls.AddAntwoord(argumentering,antwoordMogelijkheid);
            _sessieManager.UpdateSessie(lks.UserId, lks);
        }

        public string VerklaarWoord(int userId, int stellingId, string woord)
        {
            LeerkrachtSessie lks = leerkrachtSessies[userId];
            return  lks.VerklaarWoord(woord, stellingId);
        }

        public string berekenScore(int userId, int leerlingId)
        {
            LeerkrachtSessie lks = leerkrachtSessies[userId];
            LeerlingSessie lls = lks.LeerlingSessies[leerlingId];
            string gekozenPartij = lls.GeselecteerdePartij;
            List<Antwoord> juisteAntwoorden = lks.PartijAntwoorden[gekozenPartij];
            List<Antwoord> leerlingAntwoorden = lls.GetAlleAntwoorden();

            for (int i = 0 ; i < juisteAntwoorden.Count; i++)
            {
                if (juisteAntwoorden[i].mening.ToLower().Equals(leerlingAntwoorden[i].mening.ToLower()))
                {
                    lls.addScore();
                    leerlingAntwoorden[i].Correct = true;
                }
                else
                {
                    leerlingAntwoorden[i].Correct = false;
                }
            }
            leerkrachtSessies[userId].LeerlingSessies[leerlingId].updateAntwoorden(leerlingAntwoorden);
            string score = lls.Score + "/" + juisteAntwoorden.Count;
            return score;
        }

        public List<Woordverklaring> GetVerklaringen(int userId,int stellingId)
        {
            return leerkrachtSessies[userId].test.GetVerklaringen(stellingId);
        }

        public Test GetTest(int userId)
        {
            return leerkrachtSessies[userId].test;
        }

        public List<Antwoord> GetFouteAntwoorden( int userId,  int leerlingId)
        {
            List<Antwoord> antwoorden = new List<Antwoord>();
            LeerkrachtSessie lks = leerkrachtSessies[userId];
            LeerlingSessie lls = lks.LeerlingSessies[leerlingId];

            foreach (var antwoord in lls.GetAlleAntwoorden())
            {
                if (!antwoord.Correct)
                {
                    antwoorden.Add(antwoord);
                }
            }

            return antwoorden;
        }

        public Antwoord GetPartijAntwoord(int userId, int stellingId, string gekozenPartij)
        {
            LeerkrachtSessie lks =leerkrachtSessies[userId];
            return lks.PartijAntwoorden[gekozenPartij].Find(s=>s.stellingId==stellingId);
        }

        public List<Antwoord> GetAntwoorden(int userId, int leerlingId)
        {
            LeerkrachtSessie ls = leerkrachtSessies[userId];
            var lls = ls.LeerlingSessies[leerlingId];
            return lls.GetAlleAntwoorden();
        }

        public LeerkrachtSessie GetLeerkrachtSessie(int userId)
        {
            return leerkrachtSessies[userId];
        }
        public LeerlingSessie GetLeerlingSessie(int userId, int leerlingId)
        {
            return leerkrachtSessies[userId].LeerlingSessies[leerlingId];
        }
    }
}