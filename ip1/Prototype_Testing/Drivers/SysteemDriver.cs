using System;
using System.Collections.Generic;
using System.Text;
using Stemtest.BL;
using Stemtest.BL.Domain.Sessie;
using Stemtest.BL.Domain.Test;

namespace Prototype_Testing.Drivers
{
    public class SysteemDriver
    {
        private static GameManager _gameManager = new GameManager();
        public int userId;
        public string partij;
        public string klas;
        public string test;


        public GameManager GetGameManager()
        {
            return _gameManager;
        }
    }
}
