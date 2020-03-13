using CIM.Data.Infrastructure;
using CIM.Model.Models;

namespace CIM.Data.Repositories
{
    public interface IAssetAttributeRepository : IRepository<AssetTypeAttribute>
    {
    }

    public class AssetAttributeRepository : RepositoryBase<AssetTypeAttribute>, IAssetAttributeRepository
    {
        public AssetAttributeRepository(IDbFactory dbFactory) : base(dbFactory)
        {
        }
    }
}