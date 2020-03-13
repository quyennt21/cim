using CIM.Data.Infrastructure;
using CIM.Model.Models;

namespace CIM.Data.Repositories
{
    public interface IAssetAttributeValueRepository : IRepository<AssetAttributeValue>
    {
    }

    public class AssetAttributeValueRepository : RepositoryBase<AssetAttributeValue>, IAssetAttributeValueRepository
    {
        public AssetAttributeValueRepository(IDbFactory dbFactory) : base(dbFactory)
        {
        }
    }
}