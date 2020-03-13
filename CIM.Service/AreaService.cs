using CIM.Common;
using CIM.Data.Infrastructure;
using CIM.Data.Repositories;
using CIM.Model.Models;
using System.Collections.Generic;
using System.Linq;

namespace CIM.Service
{
    public interface IAreaService
    {
        void Add(Area area);

        void Update(Area area);

        Area GetById(int id, string[] includes = null);

        Area GetAreaDuplicateName(int id, string name, string[] includes = null);

        Area GetAreaDuplicateCode(int id, string code, string[] includes = null);

        IEnumerable<Area> GetAll(string[] includes = null);

        IEnumerable<Area> GetAllPaging(out int totalRow, int page = 1, int pageSize = 10, string[] includes = null);

        IEnumerable<Area> Search(string areaSearch, int locationId, out int totalRow, int page = 1, int pageSize = 10, string[] includes = null);

        Area GetAreaByCode(string code);

        IEnumerable<Area> GetAllByLocationId(int locationId, string[] includes = null);

        void SaveChanges();
    }

    public class AreaService : IAreaService
    {
        private IUnitOfWork _unitOfWork;
        private IAreaRepository _areaRepository;

        public AreaService(IUnitOfWork unitOfWork, IAreaRepository areaRepository)
        {
            this._unitOfWork = unitOfWork;
            this._areaRepository = areaRepository;
        }

        public void Add(Area area)
        {
            _areaRepository.Add(area);
        }

        public IEnumerable<Area> GetAll(string[] includes = null)
        {
            return _areaRepository.GetAll(includes);
        }

        public IEnumerable<Area> GetAllPaging(out int totalRow, int page = 1, int pageSize = 10, string[] includes = null)
        {
            var predicate = PredicateBuilder.True<Area>();

            var query = _areaRepository.GetByConditions(x => x.Active, includes);

            totalRow = query.Count();

            return query.OrderBy(x => x.CreatedAt).Skip((page - 1) * pageSize).Take(pageSize);
        }

        public Area GetById(int id, string[] includes = null)
        {
            return _areaRepository.GetSigleByConditions(x => x.ID == id, includes);
        }

        public void SaveChanges()
        {
            _unitOfWork.Commit();
        }

        public IEnumerable<Area> Search(string areaSearch, int locationId, out int totalRow, int page = 1, int pageSize = 10, string[] includes = null)
        {
            var predicate = PredicateBuilder.True<Area>();

            if (!string.IsNullOrEmpty(areaSearch))
            {
                var isContainAreaSearch = PredicateBuilder.False<Area>();
                isContainAreaSearch = isContainAreaSearch.Or(x => x.Name.Contains(areaSearch.Trim()));
                isContainAreaSearch = isContainAreaSearch.Or(x => x.AreaCode.Contains(areaSearch.Trim()));
                predicate = predicate.And(isContainAreaSearch);
            }

            if (locationId != 0)
            {
                predicate = predicate.And(a => a.Location.ID == locationId);
            }

            var query = _areaRepository.GetByConditions(predicate, includes);

            totalRow = query.Count();

            return query.OrderBy(x => x.CreatedAt).Skip((page - 1) * pageSize).Take(pageSize);
        }

        public void Update(Area area)
        {
            _areaRepository.Update(area);
        }

        public Area GetAreaByCode(string code)
        {
            Area area = null;

            area = _areaRepository.GetSigleByConditions(a => a.AreaCode.Equals(code), new string[] { "Location" });

            return area;
        }

        public IEnumerable<Area> GetAllByLocationId(int locationId, string[] includes = null)
        {
            return _areaRepository.GetByConditions(x => x.Location.ID == locationId);
        }

        public Area GetAreaDuplicateName(int id, string name, string[] includes = null)
        {
            return _areaRepository.GetSigleByConditions(x => x.Name.ToLower().Trim().Equals (name.ToLower().Trim()) && x.ID != id);
        }

        public Area GetAreaDuplicateCode(int id, string code, string[] includes = null)
        {
            return _areaRepository.GetSigleByConditions(x => x.AreaCode.ToLower().Trim() .Equals(code.ToLower().Trim()) && x.ID != id);
        }
    }
}