using Stemtest.BL.Domain.Identity;
using Stemtest.BL.Domain.Sessie;

namespace Stemtest.BL
{
    public interface IIdentityManager: IManager<int, Leerkracht>
    {
        /*Leerkracht ReadUser(string id);
        void CreateUser(Leerkracht user);
        void UpdateUser(Leerkracht user);
        void DeleteUser(string id);*/

        Klas ReadKlas(int userId, string klasNaam);
    }
}