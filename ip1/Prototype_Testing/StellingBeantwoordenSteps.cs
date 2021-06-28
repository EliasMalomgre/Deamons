using System;
using TechTalk.SpecFlow;
using Xunit;
using System.Collections.Generic;


namespace Prototype_Testing
{
    [Binding]
    public class StellingBeantwoordenSteps
    {
        private Drivers.AntwoordDriver _driver;
        public StellingBeantwoordenSteps(Drivers.AntwoordDriver driver)
        {
            _driver = driver; //?? throw new ArgumentNullException(nameof(driver));
        }

        [Given(@"ik op stelling (.*) zit als leerling (.*) in de sessie van leerkracht (.*)")]
        public void GivenIkOpStellingZitAlsLeerlingInDeSessieVanLeerkracht(int StellingId, int LeerlingId, int LeerkrachtId)
        {
            _driver.leerlingId = LeerlingId;
            _driver.userId = LeerkrachtId;


        }
        
        [When(@"ik ""(.*)"" aanduid")]
        public void WhenIkAanduid(string antwoord)
        {
            _driver.antwoord = antwoord;
        }

        [When(@"argumentering ""(.*)"" ingeef")]
        public void WhenArgumenteringIngeef(string argumentering)
        {
            _driver.argument = argumentering;
        }


        [Then(@"word mijn antwoord geregistreerd")]
        public void ThenWordMijnAntwoordGeregistreerd()
        {
            Assert.True(_driver.BeginSpel(_driver.userId, _driver.leerlingId, _driver.antwoord, _driver.argument));
        }
    }
}
