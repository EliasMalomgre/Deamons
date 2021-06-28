using System;
using System.Collections.Generic;
using TechTalk.SpecFlow;
using Xunit;
using Prototype_Domain.Sessie;

namespace Prototype_Testing
{
    [Binding]
    public class InitialiseerSysteemSteps
    {
        Drivers.SysteemDriver _driver;

        public InitialiseerSysteemSteps()
        {
            _driver = new Drivers.SysteemDriver();
        }

        [Given(@"ik leerkracht (.*) wil de test ""(.*)"" met klas ""(.*)"" doen over ""(.*)""")]
        public void GivenIkLeerkrachtWilDeTestMetKlasDoenOver(int lkrId, string test, string klas, string partij)
        {
            _driver.userId = lkrId;
            _driver.test = test;
            _driver.klas = klas;
            _driver.partij = partij;
        }
        
        [When(@"ik mijn systeem initialiseer")]
        public void WhenIkMijnSysteemInitialiseer()
        {
            _driver.GetGameManager().StartSessie(_driver.test, Prototype_Domain.Sessie.SoortSpel.PARTIJSPEL, new List<string> { _driver.partij }, _driver.klas, _driver.userId);
        }
        
        [Then(@"moet mijn sessie aangemaakt zijn")]
        public void ThenMoetMijnSessieAangemaaktZijn()
        {
            LeerkrachtSessie lkrs = _driver.GetGameManager().GetLeerkrachtSessie(_driver.userId);
            Assert.True(lkrs.PartijAntwoorden.ContainsKey(_driver.partij));
            Assert.True(lkrs.Klas.Naam.Equals(_driver.klas));
            Assert.True(lkrs.UserId == _driver.userId);
            Assert.True(lkrs.test.id.Equals(_driver.test));
        }
    }
}
