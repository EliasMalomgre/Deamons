using System;
using System.Collections.Generic;
using System.Text;
using Stemtest.BL;
using Stemtest.BL.Domain.Sessie;
using Stemtest.BL.Domain.Test;

namespace Prototype_Testing.Drivers
{
     public class AntwoordDriver
    {

        private static readonly GameManager _gameManager = new GameManager();
        public int userId ;
        public int leerlingId;
        public string antwoord;
        public string argument;

        
        public Boolean BeginSpel(int userId, int leerlingId, string antwoord, string argument)
        {
            _gameManager.StartSessie("PTest", SoortSpel.PARTIJSPEL, new List<string> { "Vlaams Belang"}, "203A", userId);
            _gameManager.BeginSpel(userId, leerlingId);
            if (argument.Equals(""))
            {
                argument = "geen argumentatie meegegeven";
            }
            Antwoord[] list1 = _gameManager.GetAntwoorden(userId, leerlingId).ToArray();

            switch (antwoord)
             {
                        case "ja":
                            _gameManager.BeantwoordStelling(argument, leerlingId, userId, 0);
                            break;
                        case "nee":
                            _gameManager.BeantwoordStelling(argument, leerlingId, userId, 1);
                            break;
            }
            Antwoord[] list2 = _gameManager.GetAntwoorden(userId, leerlingId).ToArray();
            return list2.Length >= list1.Length;
        }



            
    }
}
