using CIM.Common;
using CIM.Data.Infrastructure;
using CIM.Data.Repositories;
using CIM.Model.Models;
using System.Collections.Generic;
using System.Linq;

namespace CIM.Service
{
    public interface ICampusService
    {
        void Add(Campus campus);

        void Update(Campus campus);

        Campus GetById(int id, string[] includes = null);

        Campus GetCampusDuplicate(int id, string name, string[] includes = null);

        IEnumerable<Campus> GetAll(string[] includes = null);

        IEnumerable<Campus> GetAllPaging(out int totalRow, int page = 1, int pageSize = 10, string[] includes = null);

        IEnumerable<Campus> Search(string campusSearch, out int totalRow, int page = 1, int pageSize = 10, string[] includes = null);

        Campus GetCampusCode(string name);

        void SaveChanges();
    }

    public class CampusService : ICampusService
    {
        private IUnitOfWork _unitOfWork;
        private ICampusRepository _campusRepository;

        public CampusService(IUnitOfWork unitOfWork, ICampusRepository campusRepository)
        {
            this._unitOfWork = unitOfWork;
            this._campusRepository = campusRepository;
        }

        public CampusService()
        {
        }

        public void Add(Campus campus)
        {
            _campusRepository.Add(campus);
        }

        public IEnumerable<Campus> GetAll(string[] includes = null)
        {
            return _campusRepository.GetAll(includes);
        }

        public Campus GetById(int id, string[] includes = null)
        {
            return _campusRepository.GetSigleByConditions(x => x.ID == id, includes);
        }

        public void SaveChanges()
        {
            _unitOfWork.Commit();
        }

        public Campus GetCampusCode(string name)
        {
            Campus campus = null;

            campus = _campusRepository.GetSigleByConditions(a => a.Name.Equals(name));

            return campus;
        }

        public void Update(Campus campus)
        {
            _campusRepository.Update(campus);
        }

        public IEnumerable<Campus> GetAllPaging(out int totalRow, int page = 1, int pageSize = 10, string[] includes = null)
        {
            var predicate = PredicateBuilder.True<Campus>();

            var query = _campusRepository.GetByConditions(predicate, includes);

            totalRow = query.Count();

            return query.OrderBy(x => x.CreatedAt).Skip((page - 1) * pageSize).Take(pageSize);
        }

        public IEnumerable<Campus> Search(string campusSearch, out int totalRow, int page = 1, int pageSize = 10, string[] includes = null)
        {
            var predicate = PredicateBuilder.True<Campus>();

            if (!string.IsNullOrEmpty(campusSearch))
            {
                predicate = predicate.And(x => x.Name.Contains(campusSearch.Trim()));
            }

            var query = _campusRepository.GetByConditions(predicate, includes);

            totalRow = query.Count();

            return query.OrderBy(x => x.CreatedAt).Skip((page - 1) * pageSize).Take(pageSize);
        }

        public Campus GetCampusDuplicate(int id, string name, string[] includes = null)
        {
            return _campusRepository.GetByConditions(x => x.Name.Equals(name) && x.ID != id, includes).FirstOrDefault();
        }
    }
}