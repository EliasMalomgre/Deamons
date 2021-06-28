using System;
using System.Collections.Generic;
using Prototype_Domain.Identity;
using Prototype_Domain.Sessie;

namespace Prototype_DAL
{
    public class IdentityRepoHC: IIdentityRepository
    {
        private Dictionary<int, Leerkracht> _repo;

        public IdentityRepoHC()
        {
            _repo = new Dictionary<int, Leerkracht>();
            initialiseRepo();
        }
        public void Create(Leerkracht leerkracht)
        {
            _repo.Add(leerkracht.UserId, leerkracht);
        }

        public Leerkracht Read(int id)
        {
            Leerkracht user = _repo[id];
            if (user == null)
            {
                throw new Exception("Gebruiker niet gevonden");
            }

            return user;
        }

        public void Update(Leerkracht leerkracht)
        {
            if (Read(leerkracht.UserId) != null)
            {
                _repo[leerkracht.UserId] = leerkracht;
            }
            else
            {
                throw new Exception("Gebruiker niet gevonden");
            }
        }

        public void Delete(int id)
        {
            bool isGelukt = _repo.Remove(id);
            if (!isGelukt)
            {
                throw new Exception("Gebruiker niet gevonden");
            }
        }

        private void initialiseRepo()
        {
            //hier kun je gebruikers toevoegen
            //voorbeeld:
            SuperAdmin bigBoss = new SuperAdmin("Giovanni", "Team Rocket");
            bigBoss.UserId = 69;
            bigBoss.Klassen.Add(new Klas("203A",20));
            _repo.Add(bigBoss.UserId, bigBoss);
        }
    }
}