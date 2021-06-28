using System;
using TechTalk.SpecFlow;
using Xunit;
using Prototype_Domain.Sessie;

namespace Prototype_Testing
{
    
    [Binding]
    public class BeginSpelSteps
    {
        Drivers.SpelDriver _driver;

        public BeginSpelSteps()
        {
            _driver = new Drivers.SpelDriver();
        }

        [Given(@"Ik leerling met id (.*)  wil de test ""(.*)"" met klas ""(.*)"" doen over ""(.*)"" van leerkracht (.*)")]
        public void GivenIkLeerlingMetIdWilDeTestMetKlasDoenOverVanLeerkracht(int leerlingId, string test, string klas, string partij, int lkrId)
        {
            _driver.userId = lkrId;
            _driver.test = test;
            _driver.klas = klas;
            _driver.partij = partij;
            _driver.leerlingId = leerlingId;
        }
       


        [When(@"Ik mijn systeem initialiseer")]
        public void WhenIkMijnSysteemInitialiseer()
        {
            _driver.BeginSpel();
        }
        
        [Then(@"Moet mijn sessie aangemaakt zijn")]
        public void ThenMoetMijnSessieAangemaaktZijn()
        {
            LeerlingSessie lls = _driver.GetGameManager().GetLeerlingSessie(_driver.userId,_driver.leerlingId);
            Assert.True(lls.Id == (_driver.leerlingId));
        }
    }
}
