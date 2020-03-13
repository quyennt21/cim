using CIM.Data.Infrastructure;
using CIM.Model.Models;

namespace CIM.Data.Repositories
{
    public interface IMaintenanceDiaryRepository : IRepository<MaintenanceDiary>
    {
    }

    public class MaintenanceDiaryRepository : RepositoryBase<MaintenanceDiary>, IMaintenanceDiaryRepository
    {
        public MaintenanceDiaryRepository(IDbFactory dbFactory) : base(dbFactory)
        {
        }
    }
}