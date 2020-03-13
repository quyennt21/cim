using CIM.Data.Infrastructure;
using CIM.Model.Models;

namespace CIM.Data.Repositories
{
    public interface ICampusRepository : IRepository<Campus>
    {
    }

    public class CampusRepository : RepositoryBase<Campus>, ICampusRepository
    {
        public CampusRepository(IDbFactory dbFactory) : base(dbFactory)
        {
        }
    }
}