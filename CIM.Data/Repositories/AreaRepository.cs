using CIM.Data.Infrastructure;
using CIM.Model.Models;

namespace CIM.Data.Repositories
{
    public interface IAreaRepository : IRepository<Area>
    {
    }

    public class AreaRepository : RepositoryBase<Area>, IAreaRepository
    {
        public AreaRepository(IDbFactory dbFactory) : base(dbFactory)
        {
        }
    }
}