namespace Prototype_BL
{
    public interface IManager<I, R>
    {
        //CRUD
        void Create(R leerkrachtSessie);
        R Read(I id);
        void Update(R returnType);
        void Delete(I idType);
    }
}