using System;
using System.Collections.Generic;
using System.Text;
using Prototype_BL;

namespace Prototype_Testing.Drivers
{
    class SpelDriver
    {
        private static GameManager _gameManager = new GameManager();
        public int userId;
        public int leerlingId;
        public string antwoord;
        public string argument;
        public string partij;
        public string klas;
        public string test;
        public string score;
        public int huidigestelling;


        public void BeginSpel()
        {

            _gameManager.StartSessie(test, Prototype_Domain.Sessie.SoortSpel.PARTIJSPEL, new List<string> { partij }, klas, userId);
            _gameManager.BeginSpel(userId, leerlingId);


        }


        public GameManager GetGameManager()
        {
            return _gameManager;
        }
    }
}
