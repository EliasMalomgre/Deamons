using Stemtest.BL.Domain.Identity;
using Stemtest.BL.Domain.Sessie;
using Stemtest.DAL;

namespace Stemtest.BL
{
    public class IdentityManager: IIdentityManager
    {
        private IIdentityRepository _repo;

        public IdentityManager()
        {
            //Swap dit wanneer je de DB gaat gebruiken
            _repo = new IdentityRepoHC();
        }
        
        public Leerkracht Read(int id)
        {
            return _repo.Read(id);
        }

        public void Create(Leerkracht leerkrachtSessie)
        {
            _repo.Create(leerkrachtSessie);
        }

        public void Update(Leerkracht user)
        {
            _repo.Update(user);
        }

        public void Delete(int id)
        {
            _repo.Delete(id);
        }

        public Klas ReadKlas(int userId, string klasNaam)
        {
            return _repo.Read(userId).Klassen.Find(k => k.Naam == klasNaam);
        }
    }
}