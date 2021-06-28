using System;
using System.Collections.Generic;
using System.Text;
using Stemtest.BL;
using Stemtest.BL.Domain.Sessie;
using Stemtest.BL.Domain.Test;

namespace Prototype_Testing.Drivers
{
    public class ResultaatDriver
    {
        private static GameManager _gameManager = new GameManager();
        public int userId;
        public int leerlingId;
        public string partij;
        public string klas;
        public string test;
        public string score;



        public void BeginSpel()
        {

            _gameManager.StartSessie(test, SoortSpel.PARTIJSPEL, new List<string> { partij }, klas, userId);
            _gameManager.BeginSpel(userId, leerlingId);
            _gameManager.SelecteerPartij(leerlingId, userId, partij);


        }

        public void BeantwoordVragen()
        {

            List<int> antwoorden = new List<int>();
            antwoorden.Add(1);
            antwoorden.Add(0);
            antwoorden.Add(0);
            antwoorden.Add(0);
            antwoorden.Add(0);
            antwoorden.Add(0);
            antwoorden.Add(0);
            antwoorden.Add(0);
            antwoorden.Add(1);
            antwoorden.Add(1);


            for (int i = 0; i < 10; i++)
            {
                _gameManager.BeantwoordStelling("Geen argument", leerlingId, userId, antwoorden[i]);
            }
        }

        public GameManager GetGameManager()
        {
            return _gameManager;
        }


    }
}
