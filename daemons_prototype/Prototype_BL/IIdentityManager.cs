using Prototype_Domain.Identity;
using Prototype_Domain.Sessie;

namespace Prototype_BL
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