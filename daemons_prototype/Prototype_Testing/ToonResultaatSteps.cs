using System;
using TechTalk.SpecFlow;
using Prototype_BL;
using Xunit;

namespace Prototype_Testing
{
    [Binding]
    public class ToonResultaatSteps
    {
        Drivers.ResultaatDriver _driver;

        public ToonResultaatSteps()
        {
            _driver = new Drivers.ResultaatDriver();
        }

        [Given(@"ik ben leerling (.*)")]
        public void GivenIkBenLeerling(int leerlingId)
        {
            _driver.leerlingId = leerlingId;
        }
        
        [Given(@"ik zit in klas ""(.*)""")]
        public void GivenIkZitInKlas(string klas)
        {
            _driver.klas = klas;
        }
        
        [Given(@"ik neem deel aan de ""(.*)"" sessie van leerkracht (.*)")]
        public void GivenIkNeemDeelAanDeSessieVanLeerkracht(string test, int LkrId)
        {
            _driver.test = test;
            _driver.userId = LkrId;
        }
        
        [Given(@"ik heb ""(.*)"" gekozen")]
        public void GivenIkHebGekozen(string partij)
        {
            _driver.partij = partij;
        }
        
        [Given(@"de sessie is gestart")]
        public void GivenDeSessieIsGestart()
        {
            _driver.BeginSpel();
        }
        
        [Given(@"ik (.*) juiste antwoorden heb ingevuld")]
        public void GivenIkJuisteAntwoordenHebIngevuld(int juist)
        {
            _driver.BeantwoordVragen();
        }
        
        [When(@"ik mijn resultaat wilt bekijken")]
        public void WhenIkMijnResultaatWiltBekijken()
        {
           _driver.score = _driver.GetGameManager().berekenScore(_driver.userId,_driver.leerlingId);
        }
        
        [Then(@"dan zal mijn score (.*) zijn op de test")]
        public void ThenDanZalMijnScoreZijnOpDeTest(int p0)
        {
            Assert.True(_driver.score.Equals(p0 + "/10"));
        }
    }
}
