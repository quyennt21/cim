using CIM.Common;
using CIM.Data.Infrastructure;
using CIM.Data.Repositories;
using CIM.Model.Models;
using System.Collections.Generic;
using System.Linq;

namespace CIM.Service
{
    public interface IMailConfigService
    {
        void Add(MailConfig mailConfig);

        void Update(MailConfig mailConfig);

        MailConfig GetById(int id, string[] includes = null);

        IEnumerable<MailConfig> GetAll(string[] includes = null);

        IEnumerable<MailConfig> GetAllPaging(out int totalRow, int page = 1, int pageSize = 10, string[] includes = null);

        IEnumerable<MailConfig> Search(string stringSearch, out int totalRow, int page = 1, int pageSize = 10, string[] includes = null);

        void SaveChanges();
    }

    public class EmailConfigService : IMailConfigService
    {
        private IUnitOfWork _unitOfWork;
        private IMailConfigRepository _mailConfigRepository;

        public EmailConfigService(IUnitOfWork unitOfWork, IMailConfigRepository mailConfigRepository)
        {
            this._unitOfWork = unitOfWork;
            this._mailConfigRepository = mailConfigRepository;
        }

        public void Add(MailConfig mailConfig)
        {
            _mailConfigRepository.Add(mailConfig);
        }

        public IEnumerable<MailConfig> GetAll(string[] includes = null)
        {
            return _mailConfigRepository.GetAll(includes);
        }

        public IEnumerable<MailConfig> GetAllPaging(out int totalRow, int page = 1, int pageSize = 10, string[] includes = null)
        {
            var query = _mailConfigRepository.GetAll(includes);

            totalRow = query.Count();

            return query.OrderBy(x => x.CreatedAt).Skip((page - 1) * pageSize).Take(pageSize);
        }

        public MailConfig GetById(int id, string[] includes = null)
        {
            return _mailConfigRepository.GetSigleByConditions(x => x.ID == id, includes);
        }

        public void SaveChanges()
        {
            _unitOfWork.Commit();
        }

        public IEnumerable<MailConfig> Search(string stringSearch, out int totalRow, int page = 1, int pageSize = 10, string[] includes = null)
        {
            var predicate = PredicateBuilder.Create<MailConfig>(a => a.Active);

            if (!string.IsNullOrEmpty(stringSearch))
            {
                var isContainLocationSearch = PredicateBuilder.False<MailConfig>();
                isContainLocationSearch = isContainLocationSearch.Or(x => x.EmailAddress.ToLower().Contains(stringSearch.ToLower().Trim()));
                predicate = predicate.And(isContainLocationSearch);
            }

            var query = _mailConfigRepository.GetByConditions(predicate, includes);

            totalRow = query.Count();

            return query.OrderBy(x => x.CreatedAt).Skip((page - 1) * pageSize).Take(pageSize);
        }

        public void Update(MailConfig mailConfig)
        {
            _mailConfigRepository.Update(mailConfig);
        }
    }
}