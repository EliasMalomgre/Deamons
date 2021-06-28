using System;
using System.Collections.Generic;
using System.Text;
using Stemtest.BL;
using Stemtest.BL.Domain.Sessie;

namespace Prototype_Testing.Drivers
{
    class SpelDriver
    {
        private static GameManager _gameManager = new GameManager();
        public int userId;
        public int leerlingId;
        public string partij;
        public string klas;
        public string test;


        public void BeginSpel()
        {
            _gameManager.StartSessie(test, SoortSpel.PARTIJSPEL, new List<string> { partij }, klas, userId);
            _gameManager.BeginSpel(userId, leerlingId);
        }


        public GameManager GetGameManager()
        {
            return _gameManager;
        }
    }
}
