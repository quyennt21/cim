using CIM.Common;
using CIM.Data.Infrastructure;
using CIM.Data.Repositories;
using CIM.Model.Models;
using System.Collections.Generic;
using System.Linq;

namespace CIM.Service
{
    public interface ILocationService
    {
        void Add(Location location);

        void Update(Location location);

        Location GetById(int id, string[] includes = null);

        Location GetNameLocationDuplicate(int id, string name, string[] includes = null);

        Location GetCodeLocationDuplicate(int id, string code, string[] includes = null);

        IEnumerable<Location> GetAll(string[] includes = null);

        IEnumerable<Location> GetAllPaging(out int totalRow, int page = 1, int pageSize = 10, string[] includes = null);

        IEnumerable<Location> Search(string locationSearch, out int totalRow, int page = 1, int pageSize = 10, string[] includes = null);

        Location GetLocationByCode(string code);

        Location GetLocationByName(string name);

        void SaveChanges();
    }

    public class LocationService : ILocationService
    {
        private IUnitOfWork _unitOfWork;
        private ILocationRepository _locationRepository;

        public LocationService(IUnitOfWork unitOfWork, ILocationRepository locationRepository)
        {
            this._unitOfWork = unitOfWork;
            this._locationRepository = locationRepository;
        }

        public void Add(Location location)
        {
            _locationRepository.Add(location);
        }

        public IEnumerable<Location> GetAll(string[] includes = null)
        {
            return _locationRepository.GetAll(includes);
        }

        public IEnumerable<Location> GetAllPaging(out int totalRow, int page = 1, int pageSize = 10, string[] includes = null)
        {
            var predicate = PredicateBuilder.True<Location>();

            var query = _locationRepository.GetByConditions(predicate, includes);

            totalRow = query.Count();

            return query.OrderBy(x => x.CreatedAt).Skip((page - 1) * pageSize).Take(pageSize);
        }

        public Location GetById(int id, string[] includes = null)
        {
            return _locationRepository.GetSigleByConditions(x => x.ID == id, includes);
        }

        public void SaveChanges()
        {
            _unitOfWork.Commit();
        }

        public IEnumerable<Location> Search(string locationSearch, out int totalRow, int page = 1, int pageSize = 10, string[] includes = null)
        {
            var predicate = PredicateBuilder.True<Location>();

            if (!string.IsNullOrEmpty(locationSearch))
            {
                var isContainLocationSearch = PredicateBuilder.False<Location>();
                isContainLocationSearch = isContainLocationSearch.Or(x => x.Name.Contains(locationSearch.Trim()));
                isContainLocationSearch = isContainLocationSearch.Or(x => x.LocationCode.Contains(locationSearch.Trim()));
                predicate = predicate.And(isContainLocationSearch);
            }

            var query = _locationRepository.GetByConditions(predicate, includes);

            totalRow = query.Count();

            return query.OrderBy(x => x.CreatedAt).Skip((page - 1) * pageSize).Take(pageSize);
        }

        public Location GetLocationByCode(string code)
        {
            Location location = null;

            location = _locationRepository.GetSigleByConditions(a => a.LocationCode.Equals(code), new string[] { "Campus" });

            return location;
        }

        public Location GetLocationByName(string name)
        {
            Location location = null;

            location = _locationRepository.GetSigleByConditions(a => a.Name.Equals(name), new string[] { "Campus" });

            return location;
        }

        public void Update(Location location)
        {
            _locationRepository.Update(location);
        }

        public Location GetNameLocationDuplicate(int id, string name, string[] includes = null)
        {
            return _locationRepository.GetSigleByConditions(a => a.Name.ToLower().Trim().Equals( name.ToLower().Trim()) && a.ID != id, includes);
        }

        public Location GetCodeLocationDuplicate(int id, string code, string[] includes = null)
        {
            return _locationRepository.GetSigleByConditions(a => a.LocationCode.ToLower().Trim().Equals( code.ToLower().Trim() )&& a.ID != id, includes);
        }
    }
}