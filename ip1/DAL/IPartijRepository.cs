using System.Collections.Generic;
using Stemtest.BL.Domain.Test;

namespace Stemtest.DAL
{
    public interface IPartijRepository: IRepository<string, Partij>
    {
        /*//CRUD methodes
        Partij ReadPartij(string naam);
        void CreatePartij(Partij partij);
        void UpdatePartij(Partij partij);
        void DeletePartij(string naam);*/
        
        //Extension
        List<Partij> ReadAllePartijen();
    }
}