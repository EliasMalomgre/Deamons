using System.Collections.Generic;
using Prototype_DAL;
using Prototype_Domain.Test;

namespace Prototype_BL
{
    public class PartijManager: IPartijManager
    {
        private IPartijRepository repo;

        public PartijManager()
        {
            repo = new PartijRepoHC();
        }
        
        public void Create(Partij leerkrachtSessie)
        {
            repo.Create(leerkrachtSessie);
        }

        public Partij Read(string id)
        {
            return repo.Read(id);
        }

        public void Update(Partij partij)
        {
            repo.Update(partij);
        }

        public void Delete(string naam)
        {
            repo.Delete(naam);
        }

        public List<Partij> GetAll()
        {
            return repo.ReadAllePartijen();
        }
    }
}