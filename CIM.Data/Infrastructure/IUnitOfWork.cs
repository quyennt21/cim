namespace CIM.Data.Infrastructure
{
    public interface IUnitOfWork
    {
        void Commit();
    }
}