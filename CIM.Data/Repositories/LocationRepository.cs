using CIM.Data.Infrastructure;
using CIM.Model.Models;

namespace CIM.Data.Repositories
{
    public interface ILocationRepository : IRepository<Location>
    {
    }

    public class LocationRepository : RepositoryBase<Location>, ILocationRepository
    {
        public LocationRepository(IDbFactory dbFactory) : base(dbFactory)
        {
        }
    }
}