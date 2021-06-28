using Stemtest.BL.Domain.Test;
using Stemtest.DAL;

namespace Stemtest.BL
{
    public class TestManager: ITestManager
    {
        private ITestRepository repo;

        public TestManager()
        {
            repo = new TestRepoHC();
        }
        
        public Test Read(string id)
        {
            return repo.Read(id);
        }

        public void Create(Test leerkrachtSessie)
        {
            repo.Create(leerkrachtSessie);
        }

        public void Update(Test test)
        {
            repo.Update(test);
        }

        public void Delete(string id)
        {
            repo.Delete(id);
        }
    }
}