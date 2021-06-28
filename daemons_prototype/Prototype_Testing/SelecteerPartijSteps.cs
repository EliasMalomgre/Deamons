using System;
using TechTalk.SpecFlow;
using Prototype_BL;
using Xunit;

namespace Prototype_Testing
{
    [Binding]
    public class SelecteerPartijSteps
    {
        Drivers.PartijSel _driver;

        public SelecteerPartijSteps()
        {
            _driver = new Drivers.PartijSel();
        }

        [Given(@"ik leerlingId (.*)  van klas ""(.*)"" in de sessie ""(.*)"" van leerkracht (.*)")]
        public void GivenIkLeerlingIdVanKlasInDeSessieVanLeerkracht(int leerlingId, string klas, string test, int LkrId)
        {
            _driver.leerlingId = leerlingId;
            _driver.klas = klas;
            _driver.test = test;
            _driver.userId = LkrId;
        }


        [When(@"Ik mijn partij ""(.*)"" selecteer")]
        public void WhenIkMijnPartijSelecteer(string partij)
        {
            _driver.partij = partij;
        }

        [When(@"het spel is gestart")]
        public void GivenHetSpelIsGestart()
        {
            _driver.BeginSpel();
        }

        [Then(@"Dan zou ik hiervan het partijspel moeten krijgen")]
        public void ThenDanZouIkHiervanHetPartijspelMoetenKrijgen()
        {
            Assert.True(_driver.GetGameManager().GetLeerlingSessie(_driver.userId, _driver.leerlingId).GeselecteerdePartij.Equals("Vlaams Belang"));
        }
    }
}
