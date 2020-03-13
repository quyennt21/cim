using CIM.Data.Infrastructure;
using CIM.Model.Models;

namespace CIM.Data.Repositories
{
    public interface IWarrantyRepository : IRepository<Warranty>
    {
    }

    public class WarrantyRepository : RepositoryBase<Warranty>, IWarrantyRepository
    {
        public WarrantyRepository(IDbFactory dbFactory) : base(dbFactory)
        {
        }
    }
}