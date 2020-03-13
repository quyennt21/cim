using CIM.Common;
using CIM.Data.Infrastructure;
using CIM.Data.Repositories;
using CIM.Model.Models;
using System.Collections.Generic;
using System.Linq;

namespace CIM.Service
{
    public interface IAssetLogService
    {
        void Add(AssetLog log);

        void Update(AssetLog log);

        AssetLog GetById(int id, string[] includes = null);

        IEnumerable<AssetLog> GetAll(string[] includes = null);

        IEnumerable<AssetLog> GetAllPaging(out int totalRow, int page = 1, int pageSize = 10, string[] includes = null);

        IEnumerable<AssetLog> Search(string assetSearch, string userSearch, out int totalRow, int page = 1, int pageSize = 10, string[] includes = null);

        void SaveChanges();
    }

    public class AssetLogService : IAssetLogService
    {
        private IUnitOfWork _unitOfWork;
        private IAssetLogRepository _assetLogRepository;

        public AssetLogService(IUnitOfWork unitOfWork, IAssetLogRepository assetLogRepository)
        {
            this._unitOfWork = unitOfWork;
            this._assetLogRepository = assetLogRepository;
        }

        public void Add(AssetLog assetLog)
        {
            _assetLogRepository.Add(assetLog);
        }

        public IEnumerable<AssetLog> GetAll(string[] includes = null)
        {
            return _assetLogRepository.GetAll(includes);
        }

        public IEnumerable<AssetLog> GetAllPaging(out int totalRow, int page = 1, int pageSize = 10, string[] includes = null)
        {
            var query = _assetLogRepository.GetByConditions(x => x.Active, includes);

            totalRow = query.Count();

            return query.OrderBy(x => x.CreatedAt).Skip((page - 1) * pageSize).Take(pageSize);
        }

        public AssetLog GetById(int id, string[] includes = null)
        {
            return _assetLogRepository.GetSigleByConditions(x => x.ID == id, includes);
        }

        public void SaveChanges()
        {
            _unitOfWork.Commit();
        }

        public IEnumerable<AssetLog> Search(string assetSearch, string userSearch, out int totalRow, int page = 1, int pageSize = 10, string[] includes = null)
        {
            var predicate = PredicateBuilder.Create<AssetLog>(a => a.Active);

            if (!string.IsNullOrEmpty(assetSearch))
            {
                var isContainAssetSearch = PredicateBuilder.False<AssetLog>();
                isContainAssetSearch = isContainAssetSearch.Or(x => x.Asset.AssetCode.Contains(assetSearch.Trim()));
                isContainAssetSearch = isContainAssetSearch.Or(x => x.Asset.Name.Contains(assetSearch.Trim()));
                predicate = predicate.And(isContainAssetSearch);
            }

            if (!string.IsNullOrEmpty(userSearch))
            {
                predicate = predicate.And(x => x.ApplicationUser.UserName.Contains(userSearch.Trim()));
            }

            var query = _assetLogRepository.GetByConditions(predicate, includes);

            totalRow = query.Count();

            return query.OrderBy(x => x.CreatedAt).Skip((page - 1) * pageSize).Take(pageSize);
        }

        public void Update(AssetLog assetLog)
        {
            _assetLogRepository.Update(assetLog);
        }
    }
}