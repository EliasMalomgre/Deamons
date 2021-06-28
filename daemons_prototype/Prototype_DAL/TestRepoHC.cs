using System;
using System.Collections.Generic;
using System.Linq;
using Prototype_Domain.Identity;
using Prototype_Domain.Test;

namespace Prototype_DAL
{
    public class TestRepoHC : ITestRepository
    {
        private Dictionary<string, Test> _repo;

        public TestRepoHC()
        {
            _repo = new Dictionary<string, Test>();
            initialiseRepo();
        }
        
        public Test Read(string id)
        {
            Test deTest = _repo[id];
            if (deTest == null)
            {
                throw new Exception("Test niet gevonden");
            }

            return deTest;
        }

        public void Create(Test test)
        {
            _repo.Add(test.id, test);
        }

        public void Update(Test test)
        {
            if (Read(test.id) != null)
            {
                _repo[test.id] = test;
            }
            else
            {
                throw new Exception("Test niet gevonden");
            }
        }

        public void Delete(string id)
        {
            bool isGelukt = _repo.Remove(id);
            if (!isGelukt)
            {
                throw new Exception("Test niet gevonden");
            }
        }

        private void initialiseRepo()
        {
            //vul hier de repo met testdata
            Test test = new Test("PTest","PartijTest",new SuperAdmin("Michiel","The Tree Company"));
            test.stellingen.Add(new Stelling(1, "Moet er meer ingezet worden op windenergie?"));
            test.stellingen.Add(new Stelling(2,"Gemeentebesturen moeten minder mogelijkheden krijgen om de plaatsing van windmolens in hun gemeente tegen te houden?"));
            test.stellingen.Add(new Stelling(3,"Er mag geen statiegeld ingevoerd worden op drankblikjes"));
            test.stellingen.Add(new Stelling(4,"Moet de kernenergie behouden worden?"));
            test.stellingen.Add(new Stelling(5, "Moet je een taaltest afleggen vooraleer je de belgische nationaliteit kan krijgen?"));
            test.stellingen.Add(new Stelling(6,"In crisistijden, mag de staat dan in het rood gaan?"));
            test.stellingen.Add(new Stelling(7,"Moet de pensioenleeftijd naar 67 opgetrokken worden?"));
            test.stellingen.Add(new Stelling(8,"Moet het mobiliteitsbudget verhoogd worden."));
            test.stellingen.Add(new Stelling(9,"Huurwaarborg naar 3 maand inplaats van 2 maand verhogen."));
            test.stellingen.Add(new Stelling(10,"De erfenisbelastingen moeten verder dalen."));
            test.stellingen.Find(s => s.stellingID == 1).woordverklaringen.Add(new Woordverklaring(){woord = "windenergie",verklaring = "Energie die opgewekt wordt door gebruik van wind."});
            test.stellingen.Find(s => s.stellingID == 3).woordverklaringen.Add(new Woordverklaring(){woord = "statiegeld",verklaring = "een klein bedrag dat wordt geheven bij de aankoop van een product en dat wordt terugbetaald als de koper de verpakking van het product na gebruik weer inlevert"});
            _repo.Add(test.id,test);
        }
        
    }
}