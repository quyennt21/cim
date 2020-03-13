using CIM.Data.Infrastructure;
using CIM.Model.Models;

namespace CIM.Data.Repositories
{
    public interface IRateRepository : IRepository<Rate>
    {
    }

    public class RateRepository : RepositoryBase<Rate>, IRateRepository
    {
        public RateRepository(IDbFactory dbFactory) : base(dbFactory)
        {
        }
    }
}