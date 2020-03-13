using CIM.Common;
using CIM.Data.Infrastructure;
using CIM.Data.Repositories;
using CIM.Model.Models;
using System.Collections.Generic;
using System.Linq;

namespace CIM.Service
{
    public interface IPICService
    {
        void Add(PIC pic);

        void Update(PIC pic);

        PIC GetById(int id, string[] includes = null);

        IEnumerable<PIC> GetAll(string[] includes = null);

        IEnumerable<PIC> GetAllPaging(out int totalRow, int page = 1, int pageSize = 10, string[] includes = null);

        IEnumerable<PIC> Search(int locationId, int assetTypeId, string userSearch, out int totalRow, int page = 1, int pageSize = 10, string[] includes = null);

        PIC GetByAreaIdAndAssetTypeId(int areaId, int assetTypeId, string[] includes = null);

        void SaveChanges();
    }

    public class PICService : IPICService
    {
        private IUnitOfWork _unitOfWork;
        private IPICRepository _PICRepository;
        private IAreaRepository _areaRepository;

        public PICService(IUnitOfWork unitOfWork, IPICRepository PICRepository, IAreaRepository areaRepository)
        {
            this._unitOfWork = unitOfWork;
            this._PICRepository = PICRepository;
            this._areaRepository = areaRepository;
        }

        public void Add(PIC pic)
        {
            _PICRepository.Add(pic);
        }

        public IEnumerable<PIC> GetAll(string[] includes = null)
        {
            return _PICRepository.GetAll(includes);
        }

        public IEnumerable<PIC> GetAllPaging(out int totalRow, int page = 1, int pageSize = 10, string[] includes = null)
        {
            var query = _PICRepository.GetByConditions(x => x.Active, includes);

            totalRow = query.Count();

            return query.OrderBy(x => x.CreatedAt).Skip((page - 1) * pageSize).Take(pageSize);
        }

        public PIC GetByAreaIdAndAssetTypeId(int areaId, int assetTypeId, string[] includes = null)
        {
            return _PICRepository.GetSigleByConditions(x => x.AreaID == areaId && x.AssetTypeID == assetTypeId, includes);
        }

        public PIC GetById(int id, string[] includes = null)
        {
            return _PICRepository.GetSigleByConditions(x => x.ID == id, includes);
        }

        public void SaveChanges()
        {
            _unitOfWork.Commit();
        }

        public IEnumerable<PIC> Search(int locationId, int assetTypeId, string userSearch, out int totalRow, int page = 1, int pageSize = 10, string[] includes = null)
        {
            var predicate = PredicateBuilder.Create<PIC>(a => a.Active);

            //search by location id
            if (locationId > 0)
            {
                var listArea = _areaRepository.GetByConditions(x => x.Location.ID == locationId);
                var isArea = PredicateBuilder.False<PIC>();
                foreach (var area in listArea)
                {
                    isArea = isArea.Or(x => x.Area.ID == area.ID);
                }
                predicate = predicate.And(isArea);
            }

            //search by asset type id
            if (assetTypeId > 0)
            {
                predicate = predicate.And(x => x.AssetType.ID == assetTypeId);
            }

            if (!string.IsNullOrEmpty(userSearch))
            {
                predicate = predicate.And(x => x.ApplicationUser.UserName.Contains(userSearch.Trim()));
            }

            var query = _PICRepository.GetByConditions(predicate, includes);

            totalRow = query.Count();

            return query.OrderBy(x => x.CreatedAt).Skip((page - 1) * pageSize).Take(pageSize);
        }

        public void Update(PIC pic)
        {
            _PICRepository.Update(pic);
        }
    }
}