using CIM.Common;
using CIM.Data.Infrastructure;
using CIM.Data.Repositories;
using CIM.Model.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace CIM.Service
{
    public interface IMaintenanceDiaryService
    {
        void Add(MaintenanceDiary maintenanceDiary);

        void Update(MaintenanceDiary maintenanceDiary);

        MaintenanceDiary GetById(int id, string[] includes = null);

        IEnumerable<MaintenanceDiary> GetAll(string[] includes = null);

        IEnumerable<MaintenanceDiary> GetAllPaging(out int totalRow, int page = 1, int pageSize = 10, string[] includes = null);

        IEnumerable<MaintenanceDiary> Search(string assetSearch, string fromDateStr, string toDateStr, out int totalRow, int page = 1, int pageSize = 10, string[] includes = null);

        void SaveChanges();
    }

    public class MaintenanceDiaryService : IMaintenanceDiaryService
    {
        private IUnitOfWork _unitOfWork;
        private IMaintenanceDiaryRepository _maintenanceDiaryRepository;

        public MaintenanceDiaryService(IUnitOfWork unitOfWork, IMaintenanceDiaryRepository maintenanceDiaryRepository)
        {
            this._unitOfWork = unitOfWork;
            this._maintenanceDiaryRepository = maintenanceDiaryRepository;
        }

        public void Add(MaintenanceDiary maintenanceDiary)
        {
            _maintenanceDiaryRepository.Add(maintenanceDiary);
        }

        public IEnumerable<MaintenanceDiary> GetAll(string[] includes = null)
        {
            return _maintenanceDiaryRepository.GetAll(includes);
        }

        public IEnumerable<MaintenanceDiary> GetAllPaging(out int totalRow, int page = 1, int pageSize = 10, string[] includes = null)
        {
            var query = _maintenanceDiaryRepository.GetByConditions(x => x.Active, includes);

            totalRow = query.Count();

            return query.OrderBy(x => x.MaintenanceDate).Skip((page - 1) * pageSize).Take(pageSize);
        }

        public MaintenanceDiary GetById(int id, string[] includes = null)
        {
            return _maintenanceDiaryRepository.GetSigleByConditions(x => x.ID == id, includes);
        }

        public void SaveChanges()
        {
            _unitOfWork.Commit();
        }

        public IEnumerable<MaintenanceDiary> Search(string assetSearch, string fromDateStr, string toDateStr, out int totalRow, int page = 1, int pageSize = 10, string[] includes = null)
        {
            var predicate = PredicateBuilder.Create<MaintenanceDiary>(a => a.Active);

            if (!string.IsNullOrEmpty(assetSearch))
            {
                var isContainAssetSearch = PredicateBuilder.False<MaintenanceDiary>();
                isContainAssetSearch = isContainAssetSearch.Or(x => x.Asset.Name.Contains(assetSearch.Trim()));
                isContainAssetSearch = isContainAssetSearch.Or(x => x.Asset.AssetCode.Contains(assetSearch.Trim()));
                predicate = predicate.And(isContainAssetSearch);
            }

            if (!string.IsNullOrEmpty(fromDateStr))
            {
                var fromDate = Convert.ToDateTime(fromDateStr);
                predicate = predicate.And(x => DbFunctions.TruncateTime(x.MaintenanceDate) >= fromDate);
            }

            if (!string.IsNullOrEmpty(toDateStr))
            {
                var toDate = Convert.ToDateTime(toDateStr);
                predicate = predicate.And(x => DbFunctions.TruncateTime(x.MaintenanceDate) <= toDate);
            }

            var query = _maintenanceDiaryRepository.GetByConditions(predicate, includes);

            totalRow = query.Count();

            return query.OrderBy(x => x.MaintenanceDate).Skip((page - 1) * pageSize).Take(pageSize);
        }

        public void Update(MaintenanceDiary maintenanceDiary)
        {
            _maintenanceDiaryRepository.Update(maintenanceDiary);
        }
    }
}