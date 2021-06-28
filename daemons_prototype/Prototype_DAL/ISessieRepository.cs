using System.Collections.Generic;
using Prototype_Domain.Sessie;

namespace Prototype_DAL
{
    public interface ISessieRepository
    {
        /*void CreateKlas(int userId, Klas klas);
        Klas ReadKlas(int userId, string klasNaam);
        void UpdateKlas(int userId, Klas klas);
        void DeleteKlas(int userId, string klasNaam);*/

        int CreateSessie(int userId, LeerkrachtSessie sessie);
        LeerkrachtSessie ReadSessie(int userId, int sessionId);
        void UpdateSessie(int userId, LeerkrachtSessie sessie);
        void DeleteSessie(int userId, int sessionId);
    }
}