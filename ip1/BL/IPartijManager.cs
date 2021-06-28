using System.Collections.Generic;
using Stemtest.BL.Domain.Test;

namespace Stemtest.BL
{
    public interface IPartijManager: IManager<string, Partij>
    {
        /*//CRUD
        void CreatePartij(Partij partij);
        Partij ReadPartij(string naam);
        void UpdatePartij(Partij partij);
        void DeletePartij(string naam);*/
        
        //Extension
        List<Partij> GetAll();
    }
}