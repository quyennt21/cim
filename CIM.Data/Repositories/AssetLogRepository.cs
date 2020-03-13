using CIM.Data.Infrastructure;
using CIM.Model.Models;

namespace CIM.Data.Repositories
{
    public interface IAssetLogRepository : IRepository<AssetLog>
    {
    }

    public class AssetLogRepository : RepositoryBase<AssetLog>, IAssetLogRepository
    {
        public AssetLogRepository(IDbFactory dbFactory) : base(dbFactory)
        {
        }
    }
}