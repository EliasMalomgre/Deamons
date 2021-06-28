using Prototype_DAL;
using Prototype_Domain.Sessie;

namespace Prototype_BL
{
    public class SessieManager: ISessieManager
    {
        private ISessieRepository repo;

        public SessieManager()
        {
            repo = new SessieRepoHC();
        }


        public int CreateSessie(int userId, LeerkrachtSessie sessie)
        {
            return repo.CreateSessie(userId, sessie);
        }

        public LeerkrachtSessie ReadSessie(int userId, int sessionId)
        {
            return repo.ReadSessie(userId, sessionId);
        }

        public void UpdateSessie(int userId, LeerkrachtSessie sessie)
        {
            repo.UpdateSessie(userId, sessie);
        }

        public void DeleteSessie(int userId, int sessionId)
        {
            repo.DeleteSessie(userId,sessionId);
        }
    }
}