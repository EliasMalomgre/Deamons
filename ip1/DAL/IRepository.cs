namespace Stemtest.DAL
{
    public interface IRepository<I,R>
    {
        //CRUD
        void Create(R returnType);
        R Read(I idType);
        void Update(R returnType);
        void Delete(I idType);
    }
}