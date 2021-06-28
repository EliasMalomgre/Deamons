using Stemtest.BL.Domain.Test;

namespace Stemtest.DAL
{
    public interface ITestRepository: IRepository<string, Test>
    {
        /*//CRUD methodes
        Test Read(int id);
        void Create(Test test);
        void Update(Test test);
        void Delete(int id);*/
    }
}