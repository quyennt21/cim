using CIM.Common;
using CIM.Data.Infrastructure;
using CIM.Data.Repositories;
using CIM.Model.Models;
using System.Collections.Generic;
using System.Linq;

namespace CIM.Service
{
    public interface IWarrantyService
    {
        void Add(Warranty warranty);

        void Update(Warranty warranty);

        Warranty GetById(int id, string[] includes = null);

        IEnumerable<Warranty> GetAll(string[] includes = null);

        IEnumerable<Warranty> GetAllPaging(out int totalRow, int page = 1, int pageSize = 10, string[] includes = null);

        IEnumerable<Warranty> Search(string assetSearch, out int totalRow, int page = 1, int pageSize = 10, string[] includes = null);

        void SaveChanges();
    }

    public class WarrantyService : IWarrantyService
    {
        private IUnitOfWork _unitOfWork;
        private IWarrantyRepository _warrantyRepository;

        public WarrantyService(IUnitOfWork unitOfWork, IWarrantyRepository warrantyServiceRepository)
        {
            this._unitOfWork = unitOfWork;
            this._warrantyRepository = warrantyServiceRepository;
        }

        public void Add(Warranty warranty)
        {
            _warrantyRepository.Add(warranty);
        }

        public IEnumerable<Warranty> GetAll(string[] includes = null)
        {
            return _warrantyRepository.GetAll(includes);
        }

        public IEnumerable<Warranty> GetAllPaging(out int totalRow, int page = 1, int pageSize = 10, string[] includes = null)
        {
            var query = _warrantyRepository.GetByConditions(x => x.Active, includes);

            totalRow = query.Count();

            return query.OrderByDescending(x => x.EndDate).Skip((page - 1) * pageSize).Take(pageSize);
        }

        public Warranty GetById(int id, string[] includes = null)
        {
            return _warrantyRepository.GetSigleByConditions(x => x.ID == id, includes);
        }

        public void SaveChanges()
        {
            _unitOfWork.Commit();
        }

        public IEnumerable<Warranty> Search(string assetSearch, out int totalRow, int page = 1, int pageSize = 10, string[] includes = null)
        {
            var predicate = PredicateBuilder.Create<Warranty>(a => a.Active);

            if (!string.IsNullOrEmpty(assetSearch))
            {
                var isContainAssetSearch = PredicateBuilder.False<Warranty>();
                isContainAssetSearch = isContainAssetSearch.Or(x => x.Asset.Name.Contains(assetSearch.Trim()));
                isContainAssetSearch = isContainAssetSearch.Or(x => x.Asset.AssetCode.Contains(assetSearch.Trim()));
                predicate = predicate.And(isContainAssetSearch);
            }

            var query = _warrantyRepository.GetByConditions(predicate, includes);

            totalRow = query.Count();

            return query.OrderByDescending(x => x.EndDate).Skip((page - 1) * pageSize).Take(pageSize);
        }

        public void Update(Warranty warranty)
        {
            _warrantyRepository.Update(warranty);
        }
    }
}