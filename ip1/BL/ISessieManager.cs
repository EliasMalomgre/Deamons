using Stemtest.BL.Domain.Sessie;

namespace Stemtest.BL
{
    public interface ISessieManager
    {
        int CreateSessie(int userId, LeerkrachtSessie sessie);
        LeerkrachtSessie ReadSessie(int userId, int sessionId);
        void UpdateSessie(int userId, LeerkrachtSessie sessie);
        void DeleteSessie(int userId, int sessionId);
    }
}