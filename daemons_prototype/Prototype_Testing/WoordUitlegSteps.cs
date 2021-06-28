using System;
using TechTalk.SpecFlow;
using Prototype_BL;
using Xunit;

namespace Prototype_Testing
{
    [Binding]
    public class WoordUitlegSteps
    {
      
        Drivers.WoordUitleg _driver;

        public WoordUitlegSteps()
        {
            _driver = new Drivers.WoordUitleg();
        }
        [Given(@"Ik ben leerling (.*)")]
        public void GivenIkBenLeerling(int leerlingId)
        {
            _driver.leerlingId = leerlingId;
        }

        [Given(@"Ik zit in klas ""(.*)""")]
        public void GivenIkZitInKlas(string klas)
        {
            _driver.klas = klas;
        }

        [Given(@"Ik neem deel aan de ""(.*)"" sessie van leerkracht (.*)")]
        public void GivenIkNeemDeelAanDeSessieVanLeerkracht(string test, int LkrId)
        {
            _driver.test = test;
            _driver.userId = LkrId;
        }

        [Given(@"Ik heb ""(.*)"" gekozen")]
        public void GivenIkHebGekozen(string partij)
        {
            _driver.partij = partij;
        }

        [Given(@"De sessie is gestart")]
        public void GivenDeSessieIsGestart()
        {
            _driver.BeginSpel();
        }

        [Given(@"Ik heb (.*) stellingen beantwoord")]
        public void GivenIkHebStellingenBeantwoord(int aantal)
        {
            _driver.huidigestelling = aantal + 1;
        }
        
        [When(@"Ik moeilijke woorden opvraag")]
        public void WhenIkMoeilijkeWoordenOpvraag()
        {
           _driver.woordverklaringen =  _driver.GetGameManager().GetVerklaringen(_driver.userId, _driver.huidigestelling);
        }
        
        [Then(@"Moet ik een uitleg krijgen")]
        public void ThenMoetIkEenUitlegKrijgen()
        {
            Assert.True(_driver.woordverklaringen.Count > 0);
        }
    }
}
