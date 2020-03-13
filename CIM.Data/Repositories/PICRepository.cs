using CIM.Data.Infrastructure;
using CIM.Model.Models;

namespace CIM.Data.Repositories
{
    public interface IPICRepository : IRepository<PIC>
    {
    }

    public class PICRepository : RepositoryBase<PIC>, IPICRepository
    {
        public PICRepository(IDbFactory dbFactory) : base(dbFactory)
        {
        }
    }
}